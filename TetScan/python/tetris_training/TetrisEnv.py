import random
import cv2
import numpy as np
from PIL import Image
from time import sleep
from copy import deepcopy

# Tetris game class
class TetrisEnv:

    '''Tetris game class'''

    # BOARD
    MAP_EMPTY = 0
    MAP_BLOCK = 1
    MAP_PLAYER = 2
    BOARD_WIDTH = 10
    BOARD_HEIGHT = 20

    TETROMINO_NAMES = {
        0: "LINE",
        1: "T",
        2: "L",
        3: "J",
        4: "Z",
        5: "S",
        6: "SQUARE"
    }

    TETROMINOS = {
        0: { # I
            0: [(0,0), (1,0), (2,0), (3,0)],
            90: [(1,0), (1,1), (1,2), (1,3)],
            180: [(3,0), (2,0), (1,0), (0,0)],
            270: [(1,3), (1,2), (1,1), (1,0)],
        },
        1: { # T
            0: [(1,0), (0,1), (1,1), (2,1)],
            90: [(0,1), (1,2), (1,1), (1,0)],
            180: [(1,2), (2,1), (1,1), (0,1)],
            270: [(2,1), (1,0), (1,1), (1,2)],
        },
        2: { # L
            0: [(1,0), (1,1), (1,2), (2,2)],
            90: [(0,1), (1,1), (2,1), (2,0)],
            180: [(1,2), (1,1), (1,0), (0,0)],
            270: [(2,1), (1,1), (0,1), (0,2)],
        },
        3: { # J
            0: [(1,0), (1,1), (1,2), (0,2)],
            90: [(0,1), (1,1), (2,1), (2,2)],
            180: [(1,2), (1,1), (1,0), (2,0)],
            270: [(2,1), (1,1), (0,1), (0,0)],
        },
        4: { # Z
            0: [(0,0), (1,0), (1,1), (2,1)],
            90: [(0,2), (0,1), (1,1), (1,0)],
            180: [(2,1), (1,1), (1,0), (0,0)],
            270: [(1,0), (1,1), (0,1), (0,2)],
        },
        5: { # S
            0: [(2,0), (1,0), (1,1), (0,1)],
            90: [(0,0), (0,1), (1,1), (1,2)],
            180: [(0,1), (1,1), (1,0), (2,0)],
            270: [(1,2), (1,1), (0,1), (0,0)],
        },
        6: { # O
            0: [(1,0), (2,0), (1,1), (2,1)],
            90: [(1,0), (2,0), (1,1), (2,1)],
            180: [(1,0), (2,0), (1,1), (2,1)],
            270: [(1,0), (2,0), (1,1), (2,1)],
        }
    }

    COLORS = {
        0: (255, 255, 255),
        1: (247, 64, 99),
        2: (0, 167, 247),
    }


    def __init__(self):
        self.reset()

    
    def reset(self):
        '''Resets the game, returning the current state'''
        self.board = [[0] * TetrisEnv.BOARD_WIDTH for _ in range(TetrisEnv.BOARD_HEIGHT)]
        self.last_board = self.board
        self.game_over = False
        self.bag = list(range(len(TetrisEnv.TETROMINOS)))
        random.shuffle(self.bag)
        self.next_piece = self.bag.pop()
        self._new_round()
        self.score = 0
        return self._get_board_props(self.board)

    def _set_last_board(self, state):
        self.last_board = state

    def _get_rotated_piece(self):
        '''Returns the current piece, including rotation'''
        return TetrisEnv.TETROMINOS[self.current_piece][self.current_rotation]


    def _get_complete_board(self):
        '''Returns the complete board, including the current piece'''
        piece = self._get_rotated_piece()
        piece = [np.add(x, self.current_pos) for x in piece]
        board = [x[:] for x in self.board]
        for x, y in piece:
            board[y][x] = TetrisEnv.MAP_PLAYER
        return board


    def get_game_score(self):
        '''Returns the current game score.

        Each block placed counts as one.
        For lines cleared, it is used BOARD_WIDTH * lines_cleared ^ 2.
        '''
        return self.score
    

    def _new_round(self):
        '''Starts a new round (new piece)'''
        # Generate new bag with the pieces
        if len(self.bag) == 0:
            self.bag = list(range(len(TetrisEnv.TETROMINOS)))
            random.shuffle(self.bag)
        
        self.current_piece = self.next_piece
        self.next_piece = self.bag.pop()
        self.current_pos = [3, 0]
        self.current_rotation = 0

        if self._check_collision(self._get_rotated_piece(), self.current_pos):
            self.game_over = True


    def _check_collision(self, piece, pos):
        '''Check if there is a collision between the current piece and the board'''
        for x, y in piece:
            x += pos[0]
            y += pos[1]
            if x < 0 or x >= TetrisEnv.BOARD_WIDTH \
                    or y < 0 or y >= TetrisEnv.BOARD_HEIGHT \
                    or self.board[y][x] == TetrisEnv.MAP_BLOCK:
                return True
        return False


    def _rotate(self, angle):
        '''Change the current rotation'''
        r = self.current_rotation + angle

        if r == 360:
            r = 0
        if r < 0:
            r += 360
        elif r > 360:
            r -= 360

        self.current_rotation = r


    def _add_piece_to_board(self, piece, pos):
        '''Place a piece in the board, returning the resulting board'''        
        board = [x[:] for x in self.board]
        for x, y in piece:
            board[y + pos[1]][x + pos[0]] = TetrisEnv.MAP_BLOCK
        return board


    def _clear_lines(self, board):
        '''Clears completed lines in a board'''
        # Check if lines can be cleared
        lines_to_clear = [index for index, row in enumerate(board) if sum(row) == TetrisEnv.BOARD_WIDTH]
        if lines_to_clear:
            board = [row for index, row in enumerate(board) if index not in lines_to_clear]
            # Add new lines at the top
            for _ in lines_to_clear:
                board.insert(0, [0 for _ in range(TetrisEnv.BOARD_WIDTH)])
        return len(lines_to_clear), board


    def _number_of_holes(self, board):
        '''Number of holes in the board (empty sqquare with at least one block above it)'''
        holes = 0

        for col in zip(*board):
            i = 0
            while i < TetrisEnv.BOARD_HEIGHT and col[i] != TetrisEnv.MAP_BLOCK:
                i += 1
            holes += len([x for x in col[i+1:] if x == TetrisEnv.MAP_EMPTY])

        return holes


    def _bumpiness(self, board):
        '''Sum of the differences of heights between pair of columns'''
        total_bumpiness = 0
        max_bumpiness = 0
        min_ys = []

        for col in zip(*board):
            i = 0
            while i < TetrisEnv.BOARD_HEIGHT and col[i] != TetrisEnv.MAP_BLOCK:
                i += 1
            min_ys.append(i)
        
        for i in range(len(min_ys) - 1):
            bumpiness = abs(min_ys[i] - min_ys[i+1])
            max_bumpiness = max(bumpiness, max_bumpiness)
            total_bumpiness += abs(min_ys[i] - min_ys[i+1])

        return total_bumpiness, max_bumpiness


    def _height(self, board):
        '''Sum and maximum height of the board'''
        sum_height = 0
        max_height = 0
        min_height = TetrisEnv.BOARD_HEIGHT

        for col in zip(*board):
            i = 0
            while i < TetrisEnv.BOARD_HEIGHT and col[i] == TetrisEnv.MAP_EMPTY:
                i += 1
            height = TetrisEnv.BOARD_HEIGHT - i
            sum_height += height
            if height > max_height:
                max_height = height
            elif height < min_height:
                min_height = height

        return sum_height, max_height, min_height


    def _get_board_props(self, board):
        '''Get properties of the board'''
        lines, board = self._clear_lines(board)
        holes = self._number_of_holes(board)
        total_bumpiness, max_bumpiness = self._bumpiness(board)
        sum_height, max_height, min_height = self._height(board)
        return [lines, holes, total_bumpiness, sum_height]


    def get_states_from_input(self, game_state, options):
        self.board = game_state
        piece_array, x = self.find_piece_from_state(self.board)
        piece_info = self.identify_piece(piece_array)

        if piece_info == None:
            return None    
        
        self.current_piece = piece_info["id"]

        if options['ghost'] == "True":
            self.board = self.remove_ghost_piece(game_state, piece_array, x)

        states = {}
        
        if self.current_piece == 6: 
            rotations = [0]
        elif self.current_piece == 0:
            rotations = [0, 90]
        else:
            rotations = [0, 90, 180, 270]

        # For all rotations
        for rotation in rotations:
            piece = TetrisEnv.TETROMINOS[self.current_piece][rotation]
            min_x = min([p[0] for p in piece])
            max_x = max([p[0] for p in piece])

            # For all positions
            for x in range(-min_x, TetrisEnv.BOARD_WIDTH - max_x):
                pos = [x, 0]

                # Drop piece
                while not self._check_collision(piece, pos):
                    pos[1] += 1
                pos[1] -= 1

                # Valid move
                if pos[1] >= 0:
                    board = self._add_piece_to_board(piece, pos)
                    states[(x, rotation)] = self._get_board_props(board)

        return states

    def find_piece_from_state(self, game_state): 
        search_board = np.array(game_state) - np.array(self.last_board)
        extracted_piece = []

        piece_x_min = len(self.last_board[0]) - 1
        piece_x_max = 0
        piece_y_min = -1
        piece_y_max = -1

        # First 4 rows
        for y in range(4):
            for x in range(len(search_board[y])):
                if search_board[y][x] == 1:
                    if piece_y_min != -1:
                        piece_y_max = y
                    else:
                        piece_y_min = y

                    if x < piece_x_min:    
                        piece_x_min = x
                    if x > piece_x_max:
                        piece_x_max = x
                    self.board[y][x] = 0 # clear the piece from the baord
        
        for y in range(piece_y_min, piece_y_max + 1):
            extracted_piece.append(list(search_board[y][piece_x_min : piece_x_max + 1]))

        return extracted_piece, piece_x_min

    
    def identify_piece(self, piece_array):

        match = 0

        for id, piece in TetrisEnv.TETROMINOS.items():
            for rotation, value in piece.items():
                if match >= 4:
                    break
                match = 0
                offset_x = min(value, key=lambda x : x[0])[0]
                offset_y = min(value, key=lambda x : x[1])[1]

                for x, y in value:
                    x = x - offset_x
                    y = y - offset_y

                    try:
                        if piece_array[y][x] == 1 :
                            match = match + 1
                        else:
                            break
                            
                    except IndexError:
                        break
                if match >= 4:
                    self.extracted_piece = id
                    self.extracted_rotation = rotation

                    return {"id": id, "rotation": rotation}

    def remove_ghost_piece(self, game_state, current_piece, x_pos):
        match = 0

        search_board = deepcopy(game_state)

        for board_y in range(TetrisEnv.BOARD_HEIGHT - (len(current_piece) - 1)):
            match = 0
            match_pos = 0

            for piece_y in range(len(current_piece)):
                for piece_x in range(len(current_piece[piece_y])):
                    if search_board[board_y + piece_y][x_pos + piece_x] == 1 and current_piece[piece_y][piece_x] == 1:
                        match = match + 1

                        if match_pos == 0:
                            match_pos = board_y

                        search_board[board_y + piece_y][x_pos + piece_x] = 0

            if match == 4:
                return search_board
            else:
                search_board = deepcopy(game_state)

        return game_state

    def get_end_state(self, x, rotation):
        self.current_pos = [x, 0]
        self.current_rotation = rotation
        piece = self._get_rotated_piece()

        while not self._check_collision(piece, self.current_pos):
            self.current_pos[1] += 1
        self.current_pos[1] -= 1
     
        self.board = self._add_piece_to_board(self._get_rotated_piece(), self.current_pos)
        self.last_board = self.board.copy()

        move_x = x - 3

        move_rot = 0

        for r in range(4):
            if self.extracted_rotation == self.current_rotation:
                move_rot = r
                break
            else:
                self.extracted_rotation = (self.extracted_rotation + 90) % 360 

        print(move_x, move_rot)

        return (self.board, move_x, move_rot)


    def get_next_states(self):
        '''Get all possible next states'''
        states = {}
        piece_id = self.current_piece

        print(self.board)
        
        if piece_id == 6: 
            rotations = [0]
        elif piece_id == 0:
            rotations = [0, 90]
        else:
            rotations = [0, 90, 180, 270]

        # For all rotations
        for rotation in rotations:
            piece = TetrisEnv.TETROMINOS[piece_id][rotation]
            min_x = min([p[0] for p in piece])
            max_x = max([p[0] for p in piece])

            # For all positions
            for x in range(-min_x, TetrisEnv.BOARD_WIDTH - max_x):
                pos = [x, 0]

                # Drop piece
                while not self._check_collision(piece, pos):
                    pos[1] += 1
                pos[1] -= 1

                # Valid move
                if pos[1] >= 0:
                    board = self._add_piece_to_board(piece, pos)
                    states[(x, rotation)] = self._get_board_props(board)

        return states


    def get_input_n(self):
        '''Size of the state'''
        return 4


    def play(self, x, rotation, render=False, render_delay=None):
        '''Makes a play given a position and a rotation, returning the reward and if the game is over'''
        self.current_pos = [x, 0]
        self.current_rotation = rotation

        # Drop piece
        while not self._check_collision(self._get_rotated_piece(), self.current_pos):
            if render:
                self.render()
                if render_delay:
                    sleep(render_delay)
            self.current_pos[1] += 1
        self.current_pos[1] -= 1

        # Update board and calculate score        
        self.board = self._add_piece_to_board(self._get_rotated_piece(), self.current_pos)
        lines_cleared, self.board = self._clear_lines(self.board)
        score = 1 + (lines_cleared ** 2) * TetrisEnv.BOARD_WIDTH
        self.score += score

        # Start new round
        self._new_round()
        if self.game_over:
            score -= 2

        return score, self.game_over


    def render(self):
        '''Renders the current board'''
        img = [TetrisEnv.COLORS[p] for row in self._get_complete_board() for p in row]
        img = np.array(img).reshape(TetrisEnv.BOARD_HEIGHT, TetrisEnv.BOARD_WIDTH, 3).astype(np.uint8)
        img = img[..., ::-1] # Convert RRG to BGR (used by cv2)
        img = Image.fromarray(img, 'RGB')
        img = img.resize((TetrisEnv.BOARD_WIDTH * 25, TetrisEnv.BOARD_HEIGHT * 25))
        img = np.array(img)
        cv2.putText(img, str(self.score), (22, 22), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 0), 1)
        cv2.imshow('image', np.array(img))
        cv2.waitKey(1)

def testbench():
    env = TetrisEnv()
    
    previous_state = [
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 1, 1, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 1, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 1, 0, 1, 1, 0, 1, 0, 0 ],
        [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0 ],
        [ 1, 1, 1, 0, 1, 1, 0, 0, 1, 0 ],
        [ 0, 0, 1, 1, 0, 1, 1, 0, 1, 1 ],
        [ 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 0, 1, 0, 1, 1, 1, 1, 0, 1, 0 ],
        [ 1, 1, 0, 0, 1, 1, 1, 1, 1, 1 ]
    ]

    state = [
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 1, 1, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 1, 1, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 1, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 1, 0, 1, 1, 0, 1, 0, 0 ],
        [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0 ],
        [ 1, 1, 1, 0, 1, 1, 0, 0, 1, 0 ],
        [ 0, 0, 1, 1, 0, 1, 1, 0, 1, 1 ],
        [ 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 0, 1, 0, 1, 1, 1, 1, 0, 1, 0 ],
        [ 1, 1, 0, 0, 1, 1, 1, 1, 1, 1 ]
    ]

    env._set_last_board(previous_state)

    piece_array = env.find_piece_from_state(state)
    piece_info = env.identify_piece(piece_array)
    print(piece_info)

if __name__ == "__main__":
    testbench()