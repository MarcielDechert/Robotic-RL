#!/usr/bin/env python3
import argparse
from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel
from gym_unity.envs import UnityToGymWrapper

from lib import model

import numpy as np
import torch


ENV_ID = "../../Robotic-RL-Env/Build/Robotic-RL-Env"
MODEL = "saves/a2c-LinearToleranzHideSize512Eta1e4/best_+1.000_22000.dat"

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-m", "--model", default=MODEL, help="Model file to load" + MODEL)
    parser.add_argument("-e", "--env", default=ENV_ID, help="Environment name to use, default=" + ENV_ID)
    args = parser.parse_args()

    channel = EngineConfigurationChannel()
    unity_env = UnityEnvironment(ENV_ID, seed=1, side_channels=[channel])
    channel.set_configuration_parameters(time_scale=1.0)
    env = UnityToGymWrapper(unity_env, allow_multiple_obs=True)

    net = model.ModelA2C(env.observation_space.shape[0], env.action_space.shape[0])
    net.load_state_dict(torch.load(args.model))

    obs = env.reset()
    total_reward = 0.0
    total_steps = 0
    while True:
        obs_v = torch.FloatTensor([obs])
        mu_v, var_v, val_v = net(obs_v)
        action = mu_v.squeeze(dim=0).data.numpy()
        action = np.clip(action, -1, 1)
        obs, reward, done, _ = env.step(action)
        total_reward += reward
        total_steps += 1
        if done:
            break
    print("In %d steps we got %.3f reward" % (total_steps, total_reward))