#!/usr/bin/env python3
import os
import math
import ptan
import time
import argparse
from tensorboardX import SummaryWriter

from lib import model, common

import numpy as np
import torch
import torch.optim as optim
import torch.nn.functional as F

from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel
from gym_unity.envs import UnityToGymWrapper

"""
    Dies sind die Hyperparameter des Lernprozesses. Mit diesen kann der Lernprozess optimiert werden und ein schnelleres
    bzw. besseres Ergebnis ermöglicht werden.
"""
ENV_ID = "../../Robotic-RL-Env/Build/Robotic-RL-Env"
GAMMA = 0.99
BATCH_SIZE = 32
LR_ACTS = 1e-4
LR_VALS = 1e-4
REPLAY_SIZE = 5000
REPLAY_INITIAL = 500
SAC_ENTROPY_ALPHA = 0.2

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
    # Test wird 250 mal durchgeführt und der Mittelwert wird in rewards ausgegeben. Eine erhöhung des Counts führt zu einem
    # besseren vergleich zwischen den unterschiedlichen testzyklen
    for _ in range(count):
        obs = env.reset()
        while True:
            obs_v = ptan.agent.float32_preprocessor([obs]).to(device)
            mu_v = net(obs_v)
            action = mu_v.squeeze(dim=0).data.cpu().numpy()
            action = np.clip(action, -1, 1)
            obs, reward, done, _ = env.step(action)
            rewards += reward
            steps += 1
            if done:
                break
    return rewards / count, steps / count


if __name__ == "__main__":
    """
        Dies ist das Hauptprogramm in dem das Modell trainiert wird. Der Lernalgorithmus wird soalnge durchlaufen, bis
        das Prorgamm über die Konsole beendet wird.
    """
    # Parsen der Parameterwerte bei Start des Programms. Wichtig ist dass der Name unter -n übergeben wird.
    parser = argparse.ArgumentParser()
    parser.add_argument("--cuda", default=False, action='store_true', help='Enable CUDA')
    parser.add_argument("-n", "--name", required=True, help="Name of the run")
    parser.add_argument("-e", "--env", default=ENV_ID, help="Environment id, default=" + ENV_ID)
    args = parser.parse_args()
    device = torch.device("cuda" if args.cuda else "cpu")

    save_path = os.path.join("saves", "sac-" + args.name)
    os.makedirs(save_path, exist_ok=True)

    # Wrappen der Unity-Umgebung in eine Gym-Umgebung
    channel = EngineConfigurationChannel()
    unity_env = UnityEnvironment(ENV_ID, seed=1, side_channels=[channel])
    # Zeitfaktor muss je nach Computersystem angepasst werden und so eingestellt werden, dass das Programm noch flüssig läuft.
    channel.set_configuration_parameters(time_scale=20.0)
    env = UnityToGymWrapper(unity_env)

    # Erstellen des Modells nach der SAC-Architektur
    act_net = model.ModelActor(
        env.observation_space.shape[0],
        env.action_space.shape[0]).to(device)
    crt_net = model.ModelCritic(
        env.observation_space.shape[0]
    ).to(device)
    twinq_net = model.ModelSACTwinQ(
        env.observation_space.shape[0],
        env.action_space.shape[0]).to(device)
    print(act_net)
    print(crt_net)
    print(twinq_net)
    # Erstellen des Zielnetzes nachdem Optimiert werden soll
    tgt_crt_net = ptan.agent.TargetNet(crt_net)
    # Erstellen des Agenten mit der PTAN-Bibliothek
    writer = SummaryWriter(comment="-sac_" + args.name)
    agent = model.AgentSAC(act_net, device=device)
    # ptan ist hier der Player der die Würfe im laufe des Prozesses durchführt.
    exp_source = ptan.experience.ExperienceSourceFirstLast(
        env, agent, gamma=GAMMA, steps_count=1)
    buffer = ptan.experience.ExperienceReplayBuffer(
        exp_source, buffer_size=REPLAY_SIZE)
    act_opt = optim.Adam(act_net.parameters(), lr=LR_ACTS)
    crt_opt = optim.Adam(crt_net.parameters(), lr=LR_VALS)
    twinq_opt = optim.Adam(twinq_net.parameters(), lr=LR_VALS)

    frame_idx = 0
    best_reward = None
    with ptan.common.utils.RewardTracker(writer) as tracker:
        with ptan.common.utils.TBMeanTracker(
                writer, batch_size=10) as tb_tracker:
            while True:
                frame_idx += 1
                buffer.populate(1)
                # Entnahme eines Wurfes aus dem Expierence Buffer
                rewards_steps = exp_source.pop_rewards_steps()
                # Speichern des Wurfes in Tensorbaord
                if rewards_steps:
                    rewards, steps = zip(*rewards_steps)
                    tb_tracker.track("episode_steps", steps[0], frame_idx)
                    tracker.reward(rewards[0], frame_idx)

                # Zufällige Würfe durchführen bis die Mindestbuffergröße erreicht ist.
                if len(buffer) < REPLAY_INITIAL:
                    continue

                # Entnahme einer Stichprobes aus dem Buffer. Hier werden die Zustände, Aktionen und Referenzwerte aus einer Stichprobe erstellt.
                batch = buffer.sample(BATCH_SIZE)
                states_v, actions_v, ref_vals_v, ref_q_v = \
                    common.unpack_batch_sac(
                        batch, tgt_crt_net.target_model,
                        twinq_net, act_net, GAMMA,
                        SAC_ENTROPY_ALPHA, device)

                tb_tracker.track("ref_v", ref_vals_v.mean(), frame_idx)
                tb_tracker.track("ref_q", ref_q_v.mean(), frame_idx)

                # Optimieren der zwei Q-Netze mit den Verlustfunktionen
                twinq_opt.zero_grad()
                q1_v, q2_v = twinq_net(states_v, actions_v)
                q1_loss_v = F.mse_loss(q1_v.squeeze(),
                                       ref_q_v.detach())
                q2_loss_v = F.mse_loss(q2_v.squeeze(),
                                       ref_q_v.detach())
                # Addieren der Lossfunktionen um ein gemeinsames optimieren des TwinNetzes zu ermöglichen
                q_loss_v = q1_loss_v + q2_loss_v
                q_loss_v.backward()
                twinq_opt.step()
                tb_tracker.track("loss_q1", q1_loss_v, frame_idx)
                tb_tracker.track("loss_q2", q2_loss_v, frame_idx)

                # Optimieren des Critic-Netzes nach der Vorhersage des Zustandwertes
                crt_opt.zero_grad()
                val_v = crt_net(states_v)
                v_loss_v = F.mse_loss(val_v.squeeze(),
                                      ref_vals_v.detach())
                v_loss_v.backward()
                crt_opt.step()
                tb_tracker.track("loss_v", v_loss_v, frame_idx)

                # Optimieren des Actor-Netzes nach der Verwendung der Q-Netze
                act_opt.zero_grad()
                acts_v = act_net(states_v)
                q_out_v, _ = twinq_net(states_v, acts_v)
                act_loss = -q_out_v.mean()
                act_loss.backward()
                act_opt.step()
                tb_tracker.track("loss_act", act_loss, frame_idx)

                tgt_crt_net.alpha_sync(alpha=1 - 1e-3)

                # Nach jedem Durchlaufen der 500 Würfe wird die Testfunktion aufgerufen
                if frame_idx % TEST_ITERS == 0:
                    ts = time.time()
                    # Erhalten der Belohnung des Testzykluses
                    rewards, steps = test_net(act_net, env, device=device)
                    print("Test done in %.2f sec, reward %.3f, steps %d" % (
                        time.time() - ts, rewards, steps))
                    writer.add_scalar("test_reward", rewards, frame_idx)
                    writer.add_scalar("test_steps", steps, frame_idx)
                    # Ist die Belohnung besser als der bisherige Highscore wird das Modell gespeichert.
                    if best_reward is None or best_reward <= rewards:
                        if best_reward is not None:
                            print("Best reward updated: %.3f -> %.3f" % (best_reward, rewards))
                            name = "best_%+.3f_%d.dat" % (rewards, frame_idx)
                            fname = os.path.join(save_path, name)
                            torch.save(act_net.state_dict(), fname)
                        best_reward = rewards

    pass
