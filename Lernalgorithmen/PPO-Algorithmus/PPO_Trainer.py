#!/usr/bin/env python3
import os
import math
import ptan
import time
import argparse
from tensorboardX import SummaryWriter

from lib import model

import numpy as np
import torch
import torch.optim as optim
import torch.nn.functional as F

from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel
from gym_unity.envs import UnityToGymWrapper


ENV_ID = "../../Robotic-RL-Env/Build/Robotic-RL-Env"
GAMMA = 0.99
GAE_LAMBDA = 0.99

TRAJECTORY_SIZE = 1025
LEARNING_RATE_ACTOR = 1e-4
LEARNING_RATE_CRITIC = 1e-4

PPO_EPS = 0.05
PPO_EPOCHES = 10
PPO_BATCH_SIZE = 32

TEST_ITERS = 500


def test_net(net, env, count=128, device="cpu"):
    """
        :param net: Modell welches optimiert werden soll
        :param env: Umgebung in der das Modell optimiert werden soll
        :param count: Anzahl an Testwürfen
        :param device: Gerät auf denen die Berechnungen durchgeführt werden sollen
        :return Tupel von Belohnungen
    """
    rewards = 0.0
    steps = 0
    for _ in range(count):
        obs = env.reset()
        while True:
            obs_v = ptan.agent.float32_preprocessor([obs]).to(device)
            mu_v = net(obs_v)[0]
            action = mu_v.squeeze(dim=0).data.cpu().numpy()
            action = np.clip(action, -1, 1)
            obs, reward, done, _ = env.step(action)
            rewards += reward
            steps += 1
            if done:
                break
    return rewards / count, steps / count


def calc_logprob(mu_v, logstd_v, actions_v):
    """
        Berechnung der LogProb-Funktion zur Berechnung der neuen
        Policy-Verlustfunktion
    """
    p1 = - ((mu_v - actions_v) ** 2) / (2*torch.exp(logstd_v).clamp(min=1e-3))
    p2 = - torch.log(torch.sqrt(2 * math.pi * torch.exp(logstd_v)))
    return p1 + p2


def calc_adv_ref(trajectory, net_crt, states_v, device="cpu"):
    """
        By trajectory calculate advantage and 1-step ref value
        :param trajectory: Trajectorenliste
        :param net_crt: Critic-Netz, Netz zur Vorhersage des Zustandwertes
        :param states_v: Tensor mit gespeicherten Zuständen
        :return: Tupel von Advantagewerten bezogen auf die Zustandswerte
    """
    values_v = net_crt(states_v)
    values = values_v.squeeze().data.cpu().numpy()
    # generalized advantage estimator: smoothed version of the advantage
    last_gae = 0.0
    result_adv = []
    result_ref = []
    for val, next_val, (exp,) in zip(reversed(values[:-1]),
                                     reversed(values[1:]),
                                     reversed(trajectory[:-1])):
        if exp.done:
            delta = exp.reward - val
            last_gae = delta
        else:
            delta = exp.reward + GAMMA * next_val - val
            last_gae = delta + GAMMA * GAE_LAMBDA * last_gae
        result_adv.append(last_gae)
        result_ref.append(last_gae + val)

    adv_v = torch.FloatTensor(list(reversed(result_adv)))
    ref_v = torch.FloatTensor(list(reversed(result_ref)))
    return adv_v.to(device), ref_v.to(device)


if __name__ == "__main__":
    # Parsen der Parameterwerte bei Start des Programms
    parser = argparse.ArgumentParser()
    parser.add_argument("--cuda", default=False, action='store_true', help='Enable CUDA')
    parser.add_argument("-n", "--name", required=True, help="Name of the run")
    parser.add_argument("-e", "--env", default=ENV_ID, help="Environment id, default=" + ENV_ID)
    parser.add_argument("--lrc", default=LEARNING_RATE_CRITIC, type=float, help="Critic learning rate")
    parser.add_argument("--lra", default=LEARNING_RATE_ACTOR, type=float, help="Actor learning rate")
    args = parser.parse_args()
    device = torch.device("cuda" if args.cuda else "cpu")

    save_path = os.path.join("saves", "ppo-" + args.name)
    os.makedirs(save_path, exist_ok=True)

    # Wrappen der Unity-Umgebung in eine Gym-Umgebung
    channel = EngineConfigurationChannel()
    unity_env = UnityEnvironment(ENV_ID, seed=1, side_channels=[channel])
    channel.set_configuration_parameters(time_scale=20.0)
    env = UnityToGymWrapper(unity_env)

    # Erstellen des Modells nach der PPO-Architektur
    net_act = model.ModelActor(env.observation_space.shape[0], env.action_space.shape[0]).to(device)
    net_crt = model.ModelCritic(env.observation_space.shape[0]).to(device)
    print(net_act)
    print(net_crt)

    # Erstellen des Agenten mit der PTAN-Bibliothek
    writer = SummaryWriter(comment="-ppo_" + args.name)
    agent = model.AgentA2C(net_act, device=device)
    exp_source = ptan.experience.ExperienceSource(env, agent, steps_count=1)

    opt_act = optim.Adam(net_act.parameters(), lr=args.lra)
    opt_crt = optim.Adam(net_crt.parameters(), lr=args.lrc)

    trajectory = []
    best_reward = None
    with ptan.common.utils.RewardTracker(writer) as tracker:
        for step_idx, exp in enumerate(exp_source):
            rewards_steps = exp_source.pop_rewards_steps()
            if rewards_steps:
                rewards, steps = zip(*rewards_steps)
                writer.add_scalar("episode_steps", np.mean(steps), step_idx)
                tracker.reward(np.mean(rewards), step_idx)

            # Nach jedem Durchlaufen der 500 Würfe wird die Testfunktion aufgerufen
            if step_idx % TEST_ITERS == 0:
                ts = time.time()
                rewards, steps = test_net(net_act, env, device=device)
                print("Test done in %.2f sec, reward %.3f, steps %d" % (
                    time.time() - ts, rewards, steps))
                writer.add_scalar("test_reward", rewards, step_idx)
                writer.add_scalar("test_steps", steps, step_idx)
                # Bei besserer Belohnung das neue Speichern.
                if best_reward is None or best_reward <= rewards:
                    if best_reward is not None:
                        print("Best reward updated: %.3f -> %.3f" % (best_reward, rewards))
                        name = "best_%+.3f_%d.dat" % (rewards, step_idx)
                        fname = os.path.join(save_path, name)
                        torch.save(net_act.state_dict(), fname)
                    best_reward = rewards

            trajectory.append(exp)
            if len(trajectory) < TRAJECTORY_SIZE:
                continue

            traj_states = [t[0].state for t in trajectory]
            traj_actions = [t[0].action for t in trajectory]
            traj_states_v = torch.FloatTensor(traj_states)
            traj_states_v = traj_states_v.to(device)
            traj_actions_v = torch.FloatTensor(traj_actions)
            traj_actions_v = traj_actions_v.to(device)
            traj_adv_v, traj_ref_v = calc_adv_ref(
                trajectory, net_crt, traj_states_v, device=device)
            mu_v = net_act(traj_states_v)
            old_logprob_v = calc_logprob(
                mu_v, net_act.logstd, traj_actions_v)

            # Normalisieren der Advantagewerte
            traj_adv_v = traj_adv_v - torch.mean(traj_adv_v)
            traj_adv_v /= torch.std(traj_adv_v)

            # Herausnahme des letzten Eintrages der Trajectory und berechne den neuen ohne ihn
            trajectory = trajectory[:-1]
            old_logprob_v = old_logprob_v[:-1].detach()

            sum_loss_value = 0.0
            sum_loss_policy = 0.0
            count_steps = 0

            for epoch in range(PPO_EPOCHES):
                for batch_ofs in range(0, len(trajectory),
                                       PPO_BATCH_SIZE):
                    batch_l = batch_ofs + PPO_BATCH_SIZE
                    states_v = traj_states_v[batch_ofs:batch_l]
                    actions_v = traj_actions_v[batch_ofs:batch_l]
                    batch_adv_v = traj_adv_v[batch_ofs:batch_l]
                    batch_adv_v = batch_adv_v.unsqueeze(-1)
                    batch_ref_v = traj_ref_v[batch_ofs:batch_l]
                    batch_old_logprob_v = \
                        old_logprob_v[batch_ofs:batch_l]

                    # Trainieren des Critic-Netzes
                    opt_crt.zero_grad()
                    value_v = net_crt(states_v)
                    # Berechnen der Verlustfunktion der Zustandswerte
                    loss_value_v = F.mse_loss(
                        value_v.squeeze(-1), batch_ref_v)
                    loss_value_v.backward()
                    opt_crt.step()

                    # Optimieren des Actor-Netzes
                    opt_act.zero_grad()
                    mu_v = net_act(states_v)
                    logprob_pi_v = calc_logprob(
                        mu_v, net_act.logstd, actions_v)
                    ratio_v = torch.exp(
                        logprob_pi_v - batch_old_logprob_v)
                    surr_obj_v = batch_adv_v * ratio_v
                    c_ratio_v = torch.clamp(ratio_v,
                                            1.0 - PPO_EPS,
                                            1.0 + PPO_EPS)
                    clipped_surr_v = batch_adv_v * c_ratio_v
                    # Berechnen der Verlustfunktion der Policywahrscheinlichkeitsverteilung
                    loss_policy_v = -torch.min(
                        surr_obj_v, clipped_surr_v).mean()
                    loss_policy_v.backward()
                    opt_act.step()

                    sum_loss_value += loss_value_v.item()
                    sum_loss_policy += loss_policy_v.item()
                    count_steps += 1

            trajectory.clear()
            writer.add_scalar("advantage", traj_adv_v.mean().item(), step_idx)
            writer.add_scalar("values", traj_ref_v.mean().item(), step_idx)
            writer.add_scalar("loss_policy", sum_loss_policy / count_steps, step_idx)
            writer.add_scalar("loss_value", sum_loss_value / count_steps, step_idx)