from DQNAgent import DQNAgent
from TetrisEnv import TetrisEnv
import json
import socket
from colorama import Fore, Style
import argparse

parser = argparse.ArgumentParser()
parser.add_argument("--path", dest="path")
args = parser.parse_args()

path_to_model = args.path or "trained_models/training1/model.hdf"

HOSTNAME = '127.0.0.1'
PORT = 49995

class AI(object):
    def __init__(self, modelPath=""):
    
        self.model = DQNAgent(env.get_input_n())
        self.model.epsilon = 0

    def load_model(self, path):
        self.model.load_model(path)
        return "success"

    def predict(self, state, options):
        next_states = []
        next_states = env.get_states_from_input(state, options)
        
        if next_states is None:
            return next_states
        else:
            best_state = self.model.get_best_state(next_states.values())
            best_action = None
            for action, state in next_states.items():
                if state == best_state:
                    best_action = action
                    break
            end_state, x, rot = env.get_end_state(best_action[0], best_action[1])
            return {"end_state": end_state, "x": x, "rot": rot}

    def print_state(self, state, state2=""):
        for row in range(len(state)):
            for grid in range(len(state[row])):
                if state[row][grid] == 1:
                    print(Fore.GREEN + " ■", end=""),
                elif state[row][grid] == 0:
                    print(Fore.WHITE + " ■", end=""),
            if state2 != "":
                print(Fore.BLUE + " --->", end="")
                for grid in range(len(state2[row])):
                    if state2[row][grid] == 1:
                        print(Fore.GREEN + " ■", end=""),
                    elif state2[row][grid] == 0:
                        print(Fore.WHITE + " ■", end=""),
            print()
        print(Style.RESET_ALL)


def request_handler(conn: socket.socket, req, last_payload):
    action = req['action']
    payload = req['payload']
    options = req['options']
    
    print('Received:', action)

    if action == 'predict':
        if(last_payload == payload):
            conn.send(str("none").encode())
            return

        best_state = model.predict(payload, options)
        if best_state is None:
            conn.send(str("none").encode())
            return
            
        ret_payload = str(best_state)
        ret_payload = ret_payload.encode()
        conn.send(ret_payload)
    elif action == 'ping':
        conn.send(str("ok").encode())
    elif action == 'load':
        model.load_model(payload)
    elif action == 'quit':
        exit(0)

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    isRunning = True

    s.setsockopt(socket.SOL_SOCKET, socket.SO_KEEPALIVE, 1)
    s.bind((HOSTNAME, PORT))
    s.listen()
    env = TetrisEnv()
    model = AI()
   
    model.load_model(path_to_model)

    while(1):
        try:
            conn, addr = s.accept()
            isRunning = True
            with conn:
                print('Connection with:', addr)
                env.reset()
                last_payload = {}
                while isRunning:
                    data = conn.recv(2048)
                    if data == b'':
                        print(f'Client {addr} disconnected.')
                        isRunning = False
                    else:
                        req = json.loads(data)
                        request_handler(conn, req, last_payload)              
        except Exception:
            print("exception thrown")

