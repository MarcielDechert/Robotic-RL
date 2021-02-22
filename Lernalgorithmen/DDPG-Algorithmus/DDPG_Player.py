#!/usr/bin/env python3
import argparse
from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel
from gym_unity.envs import UnityToGymWrapper

from lib import model

import numpy as np
import torch


ENV_ID = "../../Robotic-RL-Env/Build/Robotic-RL-Env"
MODEL = "saves/ddpg-LinearHideSize256LearnRate1e4/best_+0.977_35000.dat"

"""
Programm zum Ausführen des optimierten Modells.
"""
if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-m", "--model", default=MODEL, help="Model file to load")
    parser.add_argument("-e", "--env", default=ENV_ID, help="Environment name to use, default=" + ENV_ID)
    args = parser.parse_args()

    channel = EngineConfigurationChannel()
    unity_env = UnityEnvironment(ENV_ID, seed=1, side_channels=[channel])
    channel.set_configuration_parameters(time_scale=1.0)
    env = UnityToGymWrapper(unity_env, allow_multiple_obs=True)

    net = model.DDPGActor(env.observation_space.shape[0], env.action_space.shape[0])
    net.load_state_dict(torch.load(args.model))

    obs = env.reset()
    total_reward = 0.0
    total_steps = 0
    while True:
        obs_v = torch.FloatTensor([obs])
        mu_v = net(obs_v)
        action = mu_v.squeeze(dim=0).data.numpy()
        action = np.clip(action, -1, 1)
        obs, reward, done, _ = env.step(action)
        total_reward += reward
        total_steps += 1
        if done:
            break
    action.data[0] = action.data[0] / 2 + 0.5
    action.data[1] = action.data[1] / 2 + 0.5
    action.data[0] = 10 + action.data[0] * 500
    action.data[1] = (action.data[1] * -150)
    print("The Target ist %.3f away and with the Actions %.3f and %.3f, we get %.3f Points" % (
    obs_v, action.data[0], action.data[1], reward))