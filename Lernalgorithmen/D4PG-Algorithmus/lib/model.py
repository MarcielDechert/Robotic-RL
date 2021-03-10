import ptan
import numpy as np
import torch
import torch.nn as nn
import torch.nn.functional as F

HID_SIZE = 256


class D4PGActor(nn.Module):
    """
    Erstellung des Actor-Netzes
    """
    def __init__(self, obs_size, act_size):
        super(D4PGActor, self).__init__()

        self.net = nn.Sequential(
            nn.Linear(obs_size, HID_SIZE),
            nn.ReLU(),
            nn.Linear(HID_SIZE, HID_SIZE),
            nn.ReLU(),
            nn.Linear(HID_SIZE, act_size),
            nn.Tanh()
        )

    def forward(self, x):
        return self.net(x)


class D4PGCritic(nn.Module):
    """
    Erstellung des Critic-Netzes mit Berechnung der Vorhersage der Atoms
    """
    def __init__(self, obs_size, act_size, n_atoms, v_min, v_max):
        super(D4PGCritic, self).__init__()

        self.obs_net = nn.Sequential(
            nn.Linear(obs_size, HID_SIZE),
            nn.ReLU(),
        )

        self.out_net = nn.Sequential(
            nn.Linear(HID_SIZE + act_size, HID_SIZE),
            nn.ReLU(),
            nn.Linear(HID_SIZE, n_atoms)
        )

        delta = (v_max - v_min) / (n_atoms - 1)
        self.register_buffer("supports", torch.arange(v_min, v_max+delta, delta))

    def forward(self, x, a):
        obs = self.obs_net(x)
        return self.out_net(torch.cat([obs, a], dim=1))

    def distr_to_q(self, distr):
        weights = F.softmax(distr, dim=1) * self.supports
        res = weights.sum(dim=1)
        return res.unsqueeze(dim=-1)


class AgentD4PG(ptan.agent.BaseAgent):
    """
    Agent mit implementierung der zufälligen Aktion daher des Noizy-Netzes
    """
    def __init__(self, net, device="cpu", epsilon=0.3):
        self.net = net
        self.device = device
        self.epsilon = epsilon

    def __call__(self, states, agent_states):
        states_v = ptan.agent.float32_preprocessor(states)
        states_v = states_v.to(self.device)
        mu_v = self.net(states_v)
        actions = mu_v.data.cpu().numpy()
        actions += self.epsilon * np.random.normal(
            size=actions.shape)
        actions = np.clip(actions, -1, 1)
        return actions, agent_states
