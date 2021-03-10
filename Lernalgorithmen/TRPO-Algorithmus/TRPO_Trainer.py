#!/usr/bin/env python3
import os
import math
import ptan
import time
import argparse
from tensorboardX import SummaryWriter

from lib import model, trpo

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

TRAJECTORY_SIZE = 512
LEARNING_RATE_CRITIC = 5e-4

TRPO_MAX_KL = 0.01
TRPO_DAMPING = 0.05

TEST_ITERS = 500


def test_net(net, env, count=250, device="cpu"):
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
    for val, next_val, (exp,) in zip(reversed(values[:-1]), reversed(values[1:]),
                                     reversed(trajectory[:-1])):
        if exp.done:
            delta = exp.reward - val
            last_gae = delta
        else:
            delta = exp.reward + GAMMA * next_val - val
            last_gae = delta + GAMMA * GAE_LAMBDA * last_gae
        result_adv.append(last_gae)
        result_ref.append(last_gae + val)

    adv_v = torch.FloatTensor(list(reversed(result_adv))).to(device)
    ref_v = torch.FloatTensor(list(reversed(result_ref))).to(device)
    return adv_v, ref_v


if __name__ == "__main__":
    # Parsen der Parameterwerte bei Start des Programms
    parser = argparse.ArgumentParser()
    parser.add_argument("--cuda", default=False, action='store_true', help='Enable CUDA')
    parser.add_argument("-n", "--name", required=True, help="Name of the run")
    parser.add_argument("-e", "--env", default=ENV_ID, help="Environment id, default=" + ENV_ID)
    parser.add_argument("--lr", default=LEARNING_RATE_CRITIC, type=float, help="Critic learning rate")
    parser.add_argument("--maxkl", default=TRPO_MAX_KL, type=float, help="Maximum KL divergence")
    args = parser.parse_args()
    device = torch.device("cuda" if args.cuda else "cpu")

    save_path = os.path.join("saves", "trpo-" + args.name)
    os.makedirs(save_path, exist_ok=True)

    # Wrappen der Unity-Umgebung in eine Gym-Umgebung
    channel = EngineConfigurationChannel()
    unity_env = UnityEnvironment(ENV_ID, seed=1, side_channels=[channel])
    channel.set_configuration_parameters(time_scale=20.0)
    env = UnityToGymWrapper(unity_env)

    # Erstellen des Modells nach der TRPO-Architektur
    net_act = model.ModelActor(env.observation_space.shape[0], env.action_space.shape[0]).to(device)
    net_crt = model.ModelCritic(env.observation_space.shape[0]).to(device)
    print(net_act)
    print(net_crt)

    # Erstellen des Agenten mit der PTAN-Bibliothek
    writer = SummaryWriter(comment="-trpo_" + args.name)
    agent = model.AgentA2C(net_act, device=device)
    exp_source = ptan.experience.ExperienceSource(env, agent, steps_count=1)

    opt_crt = optim.Adam(net_crt.parameters(), lr=args.lr)

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
            traj_states_v = torch.FloatTensor(traj_states).to(device)
            traj_actions_v = torch.FloatTensor(traj_actions).to(device)
            traj_adv_v, traj_ref_v = calc_adv_ref(trajectory, net_crt, traj_states_v, device=device)
            mu_v = net_act(traj_states_v)
            old_logprob_v = calc_logprob(mu_v, net_act.logstd, traj_actions_v)

            # Normalisieren der Advantagewerte
            traj_adv_v = (traj_adv_v - torch.mean(traj_adv_v)) / torch.std(traj_adv_v)

            # Herausnahme des letzten Eintrages der Trajectory und berechne den neuen ohne ihn
            trajectory = trajectory[:-1]
            old_logprob_v = old_logprob_v[:-1].detach()
            traj_states_v = traj_states_v[:-1]
            traj_actions_v = traj_actions_v[:-1]
            sum_loss_value = 0.0
            sum_loss_policy = 0.0
            count_steps = 0

            # Optimieren des Critic-Netzes
            opt_crt.zero_grad()
            value_v = net_crt(traj_states_v)
            loss_value_v = F.mse_loss(
                value_v.squeeze(-1), traj_ref_v)
            loss_value_v.backward()
            opt_crt.step()

            def get_loss():
                """
                    Berechnen der Verlustfunktion der Policy / des Actor Netzes
                """
                mu_v = net_act(traj_states_v)
                logprob_v = calc_logprob(
                    mu_v, net_act.logstd, traj_actions_v)
                dp_v = torch.exp(logprob_v - old_logprob_v)
                action_loss_v = -traj_adv_v.unsqueeze(dim=-1)*dp_v
                return action_loss_v.mean()

            def get_kl():
                """
                    Berechnen des Clippingwertes
                """
                mu_v = net_act(traj_states_v)
                logstd_v = net_act.logstd
                mu0_v = mu_v.detach()
                logstd0_v = logstd_v.detach()
                std_v = torch.exp(logstd_v)
                std0_v = std_v.detach()
                v = (std0_v ** 2 + (mu0_v - mu_v) ** 2) / \
                    (2.0 * std_v ** 2)
                kl = logstd_v - logstd0_v + v - 0.5
                return kl.sum(1, keepdim=True)

            # Optimieren der Policy
            trpo.trpo_step(net_act, get_loss, get_kl, args.maxkl,
                           TRPO_DAMPING, device=device)

            trajectory.clear()
            writer.add_scalar("advantage", traj_adv_v.mean().item(), step_idx)
            writer.add_scalar("values", traj_ref_v.mean().item(), step_idx)
            writer.add_scalar("loss_value", loss_value_v.item(), step_idx)