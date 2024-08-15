import pygame
from input_handler import InputHandler
import json

class JoystickInputHandler(InputHandler):
    # 定数の定義（PS4コントローラの設定に基づく）
    STICK_TURN_LR = 0  # Turn Left/Right (Left Stick - LR)
    STICK_UP_DOWN = 1  # Up/Down (Left Stick - UD)
    STICK_MOVE_LR = 2  # Move Left/Right (Right Stick - LR)
    STICK_MOVE_FB = 3  # Move Forward/Back (Right Stick - UD)

    SWITCH_CROSS = 0
    SWITCH_CIRCLE = 1
    SWITCH_SQUARE = 2
    SWITCH_TRIANGLE = 3

    def __init__(self, position, rotation, send_udp_message, save_to_json, ip, port):
        self.position = position
        self.rotation = rotation
        self.send_udp_message = send_udp_message
        self.ip = ip
        self.port = port
        self.save_to_json = save_to_json

        pygame.init()
        pygame.joystick.init()
        self.joystick = pygame.joystick.Joystick(0)
        self.joystick.init()

        # スティック操作の履歴を保持するための辞書と履歴の長さを設定
        self.history_len = 5
        self.stick_history = {self.STICK_TURN_LR: [], self.STICK_UP_DOWN: [], self.STICK_MOVE_LR: [], self.STICK_MOVE_FB: []}


    def average_stick_value(self, index, new_value: float):
        """
        履歴を使用してスティックの平均値を計算する
        """
        history = self.stick_history[index]
        history.append(new_value)
        if len(history) > self.history_len:
            history.pop(0)
        return sum(history) / len(history)

    def cubic_stick_value(self, x: float, a_value: float, b_value: float, c_value: float = 0.0, d_value: float = 0.0) -> float:
        """
        ドローンのスティック操作を3次関数で計算し、正規化する関数。
        """
        # 3次関数の計算
        y = a_value * x**3 + b_value * x**2 + c_value * x + d_value

        # 出力を -1 から 1 の範囲に制限（クリッピング）
        y_clipped = max(min(y, 1.0), -1.0)

        return y_clipped

    def get_stick_value(self, index):
        v = self.joystick.get_axis(index)
        v = self.average_stick_value(index, v)
        v = self.cubic_stick_value(v, 0.9, 0.1)
        return v

    def handle_input(self):
        running = True
        while running:
            for event in pygame.event.get():
                if event.type == pygame.JOYAXISMOTION:
                    temp_position = [0, 0, 0]
                    temp_rotation = [0, 0, 0]

                    # 左スティック：Yaw角と上下調整
                    temp_rotation[1] = self.get_stick_value(self.STICK_TURN_LR) * 1.0  # yaw: 左右回転
                    temp_position[1] = self.get_stick_value(self.STICK_UP_DOWN) * 0.01  # y: 上下

                    # 右スティック：左右と前後調整
                    temp_position[0] = self.get_stick_value(self.STICK_MOVE_LR) * 0.01  # x: 左右移動
                    temp_position[2] = self.get_stick_value(self.STICK_MOVE_FB) * 0.01  # z: 前後移動

                    self.position[0] += temp_position[0]
                    self.position[1] += temp_position[1]
                    self.position[2] += temp_position[2]
                    self.rotation[1] += temp_rotation[1]

                    updated_parameters = {"position": temp_position, "rotation": temp_rotation}
                    message = json.dumps(updated_parameters)
                    self.send_udp_message(message, self.ip, self.port)
                    print(f"Updated position: {temp_position}, rotation: {temp_rotation}")
                    self.save_to_json(self.position, self.rotation)

                elif event.type == pygame.JOYBUTTONDOWN:
                    if self.joystick.get_button(self.SWITCH_CIRCLE):  # 確認ボタン
                        print("Confirmation button pressed.")
                        running = False

            pygame.time.wait(10)
