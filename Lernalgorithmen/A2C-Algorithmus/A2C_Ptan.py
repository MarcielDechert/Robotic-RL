#!/usr/bin/env python3
import os
import time
import math
import ptan
import argparse
from tensorboardX import SummaryWriter

from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel
from gym_unity.envs import UnityToGymWrapper

from lib import model, common

import numpy as np
import torch
import torch.optim as optim
import torch.nn.functional as F


ENV_ID = "../../Robotic-RL-Env/Build/Robotic-RL-Env"
GAMMA = 0.99
REWARD_STEPS = 2
BATCH_SIZE = 32
LEARNING_RATE = 5e-3
ENTROPY_BETA = 1e-4

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
            obs_v = ptan.agent.float32_preprocessor([obs])
            obs_v = obs_v.to(device)
            mu_v = net(obs_v)[0]
            action = mu_v.squeeze(dim=0).data.cpu().numpy()
            action = np.clip(action, -1, 1)
            obs, reward, done, _ = env.step(action)
            rewards += reward
            steps += 1
            if done:
                break
    return rewards / count, steps / count


def calc_logprob(mu_v, var_v, actions_v):
    """
    Berechnung der LogProb-Funktion zur Berechnung der neuen
    Policy-Verlustfunktion
    """
    p1 = - ((mu_v - actions_v) ** 2) / (2*var_v.clamp(min=1e-3))
    p2 = - torch.log(torch.sqrt(2 * math.pi * var_v))
    return p1 + p2


if __name__ == "__main__":
    # Parsen der Parameterwerte bei Start des Programms
    parser = argparse.ArgumentParser()
    parser.add_argument("--cuda", default=False, action='store_true', help='Enable CUDA')
    parser.add_argument("-n", "--name", required=True, help="Name of the run")
    args = parser.parse_args()
    device = torch.device("cuda" if args.cuda else "cpu")

    save_path = os.path.join("saves", "a2c-" + args.name)
    os.makedirs(save_path, exist_ok=True)

    # Wrappen der Unity-Umgebung in eine Gym-Umgebung
    channel = EngineConfigurationChannel()
    unity_env = UnityEnvironment(ENV_ID, seed=1, side_channels=[channel])
    channel.set_configuration_parameters(time_scale=20.0)
    env = UnityToGymWrapper(unity_env)

    # Erstellen des Modells nach der A2C-Architektur
    net = model.ModelA2C(env.observation_space.shape[0], env.action_space.shape[0]).to(device)
    print(net)

    # Erstellen des Agenten mit der PTAN-Bibliothek
    writer = SummaryWriter(comment="-a2c_" + args.name)
    agent = model.AgentA2C(net, device=device)
    exp_source = ptan.experience.ExperienceSourceFirstLast(env, agent, GAMMA, steps_count=REWARD_STEPS)

    optimizer = optim.Adam(net.parameters(), lr=LEARNING_RATE)

    batch = []
    best_reward = None
    with ptan.common.utils.RewardTracker(writer) as tracker:
        with ptan.common.utils.TBMeanTracker(writer, batch_size=10) as tb_tracker:
            for step_idx, exp in enumerate(exp_source):
                rewards_steps = exp_source.pop_rewards_steps()
                if rewards_steps:
                    rewards, steps = zip(*rewards_steps)
                    tb_tracker.track("episode_steps", steps[0], step_idx)
                    tracker.reward(rewards[0], step_idx)

                # Nach jedem Durchlaufen der 500 Würfe wird die Testfunktion aufgerufen
                if step_idx % TEST_ITERS == 0:
                    ts = time.time()
                    rewards, steps = test_net(net, env, device=device)
                    print("Test done is %.2f sec, reward %.3f, steps %d" % (
                        time.time() - ts, rewards, steps))
                    writer.add_scalar("test_reward", rewards, step_idx)
                    writer.add_scalar("test_steps", steps, step_idx)
                    # Bei besserer Belohnung das neue Speichern.
                    if best_reward is None or best_reward <= rewards:
                        if best_reward is not None:
                            print("Best reward updated: %.3f -> %.3f" % (best_reward, rewards))
                            name = "best_%+.3f_%d.dat" % (rewards, step_idx)
                            fname = os.path.join(save_path, name)
                            torch.save(net.state_dict(), fname)
                        best_reward = rewards

                batch.append(exp)
                if len(batch) < BATCH_SIZE:
                    continue

                # Entnahme der Zustandswerte und Aktionen aus dem gespeicherten Batch
                states_v, actions_v, vals_ref_v = \
                    common.unpack_batch_a2c(
                        batch, net, device=device,
                        last_val_gamma=GAMMA ** REWARD_STEPS)
                batch.clear()

                # Vorhersage des aktuellen Zustandwertes mit dem aktuellen Modell
                optimizer.zero_grad()
                mu_v, var_v, value_v = net(states_v)

                # Berechnen der Verlustfunktion für den Zustandswert
                loss_value_v = F.mse_loss(
                    value_v.squeeze(-1), vals_ref_v)

                # Berechnen des Advantagewertes
                adv_v = vals_ref_v.unsqueeze(dim=-1) - \
                        value_v.detach()
                log_prob_v = adv_v * calc_logprob(
                    mu_v, var_v, actions_v)
                # Berechnen der Verlustfunktion der Policy
                loss_policy_v = -log_prob_v.mean()
                ent_v = -(torch.log(2*math.pi*var_v) + 1)/2
                entropy_loss_v = ENTROPY_BETA * ent_v.mean()

                # Berechnen der Gesamtverlustfunktion
                loss_v = loss_policy_v + entropy_loss_v + \
                         loss_value_v
                loss_v.backward()
                optimizer.step()

                tb_tracker.track("advantage", adv_v, step_idx)
                tb_tracker.track("values", value_v, step_idx)
                tb_tracker.track("batch_rewards", vals_ref_v, step_idx)
                tb_tracker.track("loss_entropy", entropy_loss_v, step_idx)
                tb_tracker.track("loss_policy", loss_policy_v, step_idx)
                tb_tracker.track("loss_value", loss_value_v, step_idx)
                tb_tracker.track("loss_total", loss_v, step_idx)