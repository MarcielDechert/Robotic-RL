import os
import pip
import mlagents
from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel

channel = EngineConfigurationChannel()
env = UnityEnvironment(file_name="../Robotic-RL-Env/Build/Robotic-RL-Env", seed=1, side_channels=[channel])
channel.set_configuration_parameters(time_scale=1.0)

env.reset()
behavior_name = list(env.behavior_specs)[0]
print(f"Name of the behavior : {behavior_name}")
spec = env.behavior_specs[behavior_name]

# Ausgabe der Anzahl an Observations
print("Number of observations : ", spec.observation_shapes)

# How many actions are possible ?
print(f"There are {spec.action_spec.continuous_size} action(s)")