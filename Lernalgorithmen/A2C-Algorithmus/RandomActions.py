import os
import pip
import mlagents
from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel

channel = EngineConfigurationChannel()
env = UnityEnvironment(file_name="../../Robotic-RL-Env/Build/Robotic-RL-Env", seed=1, side_channels=[channel])
channel.set_configuration_parameters(time_scale = 1.0)

env.reset()
behavior_name = list(env.behavior_specs)[0]
print(f"Name of the behavior : {behavior_name}")
spec = env.behavior_specs[behavior_name]

# Ausgabe der Anzahl an Observations
print("Number of observations : ", len(spec.observation_shapes))

# Is the Action continuous or multi-discrete ?

if spec.action_spec.is_continuous():
  print("The action is continuous")
if spec.action_spec.is_discrete():
  print("The action is discrete")

  
# How many actions are possible ?
print(f"There are {spec.action_spec.continuous_size} action(s)")

decision_steps, terminal_steps = env.get_steps(behavior_name)
env.set_actions(behavior_name, spec.action_spec.empty_action(len(decision_steps)))
env.step()

for index, shape in enumerate(spec.observation_shapes):
  if len(shape) == 1:
    print("First vector observations : ", decision_steps.obs[index][0,:])

for episode in range(10000):
  env.reset()
  decision_steps, terminal_steps = env.get_steps(behavior_name)
  tracked_agent = -1 # -1 indicates not yet tracking
  done = False # For the tracked_agent
  episode_rewards = 0 # For the tracked_agent

  while not done:
    # Track the first agent we see if not tracking
    # Note : len(decision_steps) = [number of agents that requested a decision]
    if tracked_agent == -1 and len(decision_steps) >= 1:
      tracked_agent = decision_steps.agent_id[0]

    # Generate an action for all agents
    action = spec.action_spec.random_action(len(decision_steps))
    print(action)

    # Set the actions
    env.set_actions(behavior_name, action)

    # Move the simulation forward
    env.step()

    # Get the new simulation results
    decision_steps, terminal_steps = env.get_steps(behavior_name)
    if tracked_agent in decision_steps: # The agent requested a decision
      episode_rewards += decision_steps[tracked_agent].reward
    if tracked_agent in terminal_steps: # The agent terminated its episode
      episode_rewards += terminal_steps[tracked_agent].reward
      done = True
  print(f"Total rewards for episode {episode} is {episode_rewards}")

env.close()
print("Closed environment")