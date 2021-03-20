import numpy as np
import torch
import torch.distributions as distr

import ptan


def unpack_batch_a2c(batch, net, last_val_gamma, device="cpu"):
    """
    Entpacken des Batches und erstellen von Trainingsdaten mit denen der Optimierungsprozess durchlaufen werden kann.
    :param batch: Übergabe der Stichproben des Buffers
    :param net: Übergabe des Netzes mit dem die Tensoren erstellt werden.
    :return: Zustandswert states_v, Aktionswert actions_v und Referenzwert ref_vals_v
    """
    states = []
    actions = []
    rewards = []
    not_done_idx = []
    last_states = []
    for idx, exp in enumerate(batch):
        # Entpacken der einzelnen Zustände und Aktionen in einzelne Listen
        states.append(exp.state)
        actions.append(exp.action)
        rewards.append(exp.reward)
        if exp.last_state is not None:
            not_done_idx.append(idx)
            last_states.append(exp.last_state)
    # Berechnen der Zustände und Aktionen mit dem Agenten
    states_v = ptan.agent.float32_preprocessor(states).to(device)
    actions_v = torch.FloatTensor(actions).to(device)

    # Berechnen der Belohnung in den Referenzwert. (Maximieren der Belohnung ist die Folge)
    rewards_np = np.array(rewards, dtype=np.float32)
    if not_done_idx:
        last_states_v = ptan.agent.float32_preprocessor(last_states).to(device)
        last_vals_v = net(last_states_v)
        last_vals_np = last_vals_v.data.cpu().numpy()[:, 0]
        rewards_np[not_done_idx] += last_val_gamma * last_vals_np

    ref_vals_v = torch.FloatTensor(rewards_np).to(device)
    return states_v, actions_v, ref_vals_v


@torch.no_grad()
def unpack_batch_sac(batch, val_net, twinq_net, policy_net,
                     gamma: float, ent_alpha: float,
                     device="cpu"):
    """
        Entnahme des SAC-Batches und Brechnen der Q-Netze und Brechnen der Policy in Bezug auf die Ausgaben des
        A2C-Batch
    """
    states_v, actions_v, ref_q_v = \
        unpack_batch_a2c(batch, val_net, gamma, device)

    # Berechnen der Referenzwerte für das Policy-Netz
    mu_v = policy_net(states_v)
    act_dist = distr.Normal(mu_v, torch.exp(policy_net.logstd))
    acts_v = act_dist.sample() # Entnahme einer Beispielaktion
    q1_v, q2_v = twinq_net(states_v, acts_v)
    # Elementeweise das Minimum zur Berechnung des neuen Referenzwertes
    ref_vals_v = torch.min(q1_v, q2_v).squeeze() - \
                 ent_alpha * act_dist.log_prob(acts_v).sum(dim=1)
    return states_v, actions_v, ref_vals_v, ref_q_v

