from DQNAgent import DQNAgent
from TetrisEnv import TetrisEnv
from CustomBoard import CustomBoard
from tqdm import tqdm


from datetime import datetime
from statistics import mean

def start():
    # Tetris Game Environment
    env = TetrisEnv()

    # Train Props
    episodes = 2000
    batch_size = 512
    epochs = 1

    # Render Props
    render_every = 5
    log_every = 1
    save_every = 1
    save_after = 500

    # Deep Q Network agent:
    agent = DQNAgent(env.get_input_n(), "saved_states/20210203-000358-ep=1800")

    # Logging
    log_location = f'logs/TetScan-training-ep={str(episodes)}-batch={str(batch_size)}-{datetime.now().strftime("%Y%m%d-%H%M%S")}'
    model_save_location = f'saved_states/{datetime.now().strftime("%Y%m%d-%H%M%S")}'
    logger = CustomBoard(log_dir=log_location)

    scores = []

    for episode in tqdm(range(episodes)):
        current_state = env.reset()
        done = False

        if render_every and episode % render_every == 0:
            render = True
        else:
            render = False

        # Game
        while not done:
            next_states = env.get_next_states()
            best_state = agent.get_best_state(next_states.values())
            
            best_action = None
            for action, state in next_states.items():
                if state == best_state:
                    best_action = action
                    break

            reward, done = env.play(best_action[0], best_action[1], render=render)
            
            agent.add_experience(current_state, next_states[best_action], reward, done)
            current_state = next_states[best_action]

        scores.append(env.get_game_score())

        agent.train(batch_size=batch_size, epochs=epochs)

        # Log every 'log_every' episodes
        if episode % log_every == 0:
            avg_score = mean(scores[-log_every:])
            min_score = min(scores[-log_every:])
            max_score = max(scores[-log_every:])

            logger.log(episode, avg_score=avg_score, min_score=min_score, max_score=max_score)

        # Save model every 'save_every' episodes
        if episode >= save_after and episode % save_every == 0:
            agent.save_model(f'{model_save_location}-ep={episode}')

if __name__ == "__main__":
    start()
