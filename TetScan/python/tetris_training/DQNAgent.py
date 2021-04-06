import keras
import numpy as np
import random
from collections import deque

class DQNAgent:
    def __init__(self, input_n, path_to_model=""):
        self.input_n = input_n
        self.epsilon = 1                                # Exploration probability, i.e. random values 
        self.epsilon_decay = self.epsilon / 1200        # Rate at which to decrease exploration chance (after 1500 episodes = no chance)
        self.discount = 0.95

        self.experience_memory = deque(maxlen=20000)    # Experience storage
        self.experience_memory_start = 2000

        if(len(path_to_model) > 0):
            self.model = keras.models.load_model(path_to_model)
        else:
            self.model = self._create_model()
        
    def _create_model(self):
        # Create Sequential neural network model of size 
        # 1. input_n size Input layer
        # 2. 32 node layer
        # 3. 32 node layer
        # 4. 1 node output layer corresponding to 1 action

        model = keras.models.Sequential()
        # Input + 2nd layer
        model.add(keras.layers.Dense(32, input_dim=self.input_n, activation="relu"))

        # 3rd layer
        model.add(keras.layers.Dense(32, activation="relu"))

        # Output layer
        model.add(keras.layers.Dense(1, activation='linear'))

        model.compile(loss='mse', optimizer='adam')

        return model

    def save_model(self, path):
        keras.models.save_model(self.model, path, overwrite=True)

    def load_model(self, path):
        if(len(path) > 0):
            self.model = keras.models.load_model(path)
            
    def add_experience(self, current_state, next_state, reward, done):
        self.experience_memory.append((current_state, next_state, reward, done))

    def get_best_state(self, states):
        # explroration if epsilon is still in play:
        if self.epsilon >= random.random():
            return random.choice(list(states))
        else:
            highest = None
            best_state = None

            for state in states:
                # Flatten the state values:
                flattened_state = np.reshape(state, [1, self.input_n])
                prediction = self.model.predict(flattened_state)[0]
                if not highest or prediction > highest:
                    highest = prediction
                    best_state = state

            return best_state

    def train(self, batch_size, epochs):
        # Number of available past experience
        available_experiences = len(self.experience_memory)
    
        # if there are enough experiences and are greater than the batch size of 32:
        if available_experiences >= self.experience_memory_start and available_experiences >= batch_size:

            # pick a random batch from previous experience
            batch = random.sample(self.experience_memory, batch_size)

            # Get a batch of new q values for next states
            new_current_states = np.array([sample[1] for sample in batch])
            future_qs_list = [x[0] for x in self.model.predict(new_current_states)]

            x = []
            y = []

            for index, (state, _, reward, done) in enumerate(batch):
                if not done:
                    new_q = reward + self.discount * future_qs_list[index]
                else:
                    new_q = reward

                x.append(state)
                y.append(new_q)

            self.model.fit(np.array(x), np.array(y), batch_size=batch_size, epochs=epochs, verbose=0)

            if self.epsilon > 0:
                self.epsilon -= self.epsilon_decay
    
