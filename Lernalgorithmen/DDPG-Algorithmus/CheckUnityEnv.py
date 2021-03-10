from gym_unity.envs import UnityToGymWrapper
from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel


ENV_ID = "../../Robotic-RL-Env/Build/Robotic-RL-Env"

channel = EngineConfigurationChannel()
unity_env = UnityEnvironment(ENV_ID, seed=1, side_channels=[channel])
channel.set_configuration_parameters(time_scale=1.0)
env = UnityToGymWrapper(unity_env, allow_multiple_obs=True)

env.reset()
print(f"Name of the behavior : {env.name}")

# Ausgabe der Anzahl an Observations
print("Number of observations : ", env.observation_space)

# How many actions are possible ?
print(f"There are {env.action_size} action(s)")