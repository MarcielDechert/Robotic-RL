import ptan
import numpy as np
import torch
import torch.nn as nn

HID_SIZE = 256


class ModelActor(nn.Module):
    """
        ModelActor definiert das Modell, welches das Policy-Netz repräsentiert. Hierzu werden die Module des PyTorch
        Frameworks genutzt um ein zweischichtes NN zu erstellen.
    """
    def __init__(self, obs_size, act_size):
        super(ModelActor, self).__init__()

        self.mu = nn.Sequential(
            nn.Linear(obs_size, HID_SIZE),
            nn.Tanh(),
            nn.Linear(HID_SIZE, HID_SIZE),
            nn.Tanh(),
            nn.Linear(HID_SIZE, act_size),
            nn.Tanh(),
        )
        self.logstd = nn.Parameter(torch.zeros(act_size))

    def forward(self, x):
        """
        Die Forward-Methode wird aufgerufen um das Modell mit dem Eingabewert x zu durchlaufen.
        :param x: Ist der Eingabewert für das Modell und hier die Entfernung des Bechers
        :return: Der Return ist der Mittelwert der Normalverteilung des Policy-Netzes aus dem die Aktionen berechnet werden
        """
        return self.mu(x)


class ModelCritic(nn.Module):
    """
        ModelCritic definiert das Modell, welches das Critic-Netz repräsentiert. Hierzu werden die Module des PyTorch
        Frameworks genutzt um ein zweischichtes NN zu erstellen.
    """
    def __init__(self, obs_size):
        super(ModelCritic, self).__init__()

        self.value = nn.Sequential(
            nn.Linear(obs_size, HID_SIZE),
            nn.ReLU(),
            nn.Linear(HID_SIZE, HID_SIZE),
            nn.ReLU(),
            nn.Linear(HID_SIZE, 1),
        )

    def forward(self, x):
        """
        Die Forward-Methode wird aufgerufen um das Modell mit dem Eingabewert x zu durchlaufen
        :param x: Ist der Eingabewert für das Modell und hier die Entfernung des Bechers
        :return: Als Ausgabe wird hier der Zustandswert V(s) direkt vorhergesagt
        """
        return self.value(x)


class ModelSACTwinQ(nn.Module):
    """
        ModellSACTwinQ repräsentiert das doppelte Q-Netz wobei hier zwei getrennte Modelle definiert werden.
        Hierzu werden die Module des PyTorch Frameworks genutzt um ein zweischichtes NN zu erstellen.
    """
    def __init__(self, obs_size, act_size):
        super(ModelSACTwinQ, self).__init__()

        self.q1 = nn.Sequential(
            nn.Linear(obs_size + act_size, HID_SIZE),
            nn.ReLU(),
            nn.Linear(HID_SIZE, HID_SIZE),
            nn.ReLU(),
            nn.Linear(HID_SIZE, 1),
        )

        self.q2 = nn.Sequential(
            nn.Linear(obs_size + act_size, HID_SIZE),
            nn.ReLU(),
            nn.Linear(HID_SIZE, HID_SIZE),
            nn.ReLU(),
            nn.Linear(HID_SIZE, 1),
        )

    def forward(self, obs, act):
        """
        Die Forward-Methode wird aufgerufen um die Modelle mit dem Eingabewert x zu durchlaufen
        :param obs: Ist die Beobachtungen, daher hier die Entfernung des Bechers zum Roboter
        :param act: Ist die Aktion, des Buffers die für ein gutes Ergebnis gesorgt hat.
        :return: Als Return werden hier die zwei getrennt voneinder berechneten Ergebnisse ausgegeben. Diese repräsentieren
        die Aktionswerte Q
        """
        x = torch.cat([obs, act], dim=1)
        return self.q1(x), self.q2(x)


class AgentSAC(ptan.agent.BaseAgent):
    """
    Die AgentSAC Klasse berechnet die unterschiedlichen Zustände mit dem eigentlichen Zielnetz. Die eigemtliche Brechnung
    findet in der Methode __call__ statt.
    """
    def __init__(self, net, device="cpu", ou_enabled=True,
                 ou_mu=0.0, ou_teta=0.15, ou_sigma=0.2,
                 ou_epsilon=1.0):
        self.net = net
        self.device = device
        self.ou_enabled = ou_enabled
        self.ou_mu = ou_mu
        self.ou_teta = ou_teta
        self.ou_sigma = ou_sigma
        self.ou_epsilon = ou_epsilon

    def initial_state(self):
        return None

    def __call__(self, states, agent_states):
        """
        Die Methode Call wird immer dann genutzt, wenn der Agent die neuen Zustände und Aktionen berechnen soll. Der Agent
        berechnet mit den aktuellen Netzen die Zustände und Aktion aus, die für den Wurf benötigt werden.
        :param states: Beobachtungen von Unity, daher Entfernung des Bechers
        :param agent_states: Hier werden ältere Zustände gespeichert, die jedoch hier keinen Einfluss haben.
        :return: Als Ausgabe werden die Aktionen ausgegeben die ausgeführt werden soll und der neue Zustand.
        """
        states_v = ptan.agent.float32_preprocessor(states)
        states_v = states_v.to(self.device)
        mu_v = self.net(states_v)
        actions = mu_v.data.cpu().numpy()

        # Vorhersage der Zukunftigen Zustände, wird jedoch nicht benötigt, da nur kein weiterer Folgezustand erhalten wird
        if self.ou_enabled and self.ou_epsilon > 0:
            new_a_states = []
            for a_state, action in zip(agent_states, actions):
                if a_state is None:
                    a_state = np.zeros(
                        shape=action.shape, dtype=np.float32)
                a_state += self.ou_teta * (self.ou_mu - a_state)
                a_state += self.ou_sigma * np.random.normal(
                    size=action.shape)

                action += self.ou_epsilon * a_state
                new_a_states.append(a_state)
        else:
            new_a_states = agent_states

        # Aktion wird im Intervall von -1 bis 1 geclippt. Hier wäre es auch möglich das spätere Intervall des Roboters
        # anzugeben, wird jedoch bei Änderung des Intervalls zu problemen führen.
        actions = np.clip(actions, -1, 1)
        return actions, new_a_states
