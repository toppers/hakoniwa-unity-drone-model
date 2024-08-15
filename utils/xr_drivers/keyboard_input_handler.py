from pynput import keyboard
from input_handler import InputHandler
import json

class KeyboardInputHandler(InputHandler):
    def __init__(self, position, rotation, send_udp_message, save_to_json, ip, port):
        self.position = position
        self.rotation = rotation
        self.send_udp_message = send_udp_message
        self.ip = ip
        self.port = port
        self.save_to_json = save_to_json

    def handle_input(self):
        def on_press(key):
            temp_position = [0, 0, 0]
            temp_rotation = [0, 0, 0]

            if hasattr(key, 'char'):
                if key.char == 'w':
                    temp_position[1] = 0.01  # y: 上昇
                    self.position[1] += 0.01
                elif key.char == 's':
                    temp_position[1] = -0.01  # y: 下降
                    self.position[1] -= 0.01
                elif key.char == 'q':
                    temp_rotation[1] = 1.0  # yaw: 左回転
                    self.rotation[1] += 1.0
                elif key.char == 'e':
                    temp_rotation[1] = -1.0  # yaw: 右回転
                    self.rotation[1] -= 1.0
            else:
                if key == keyboard.Key.up:
                    temp_position[2] = 0.01  # z: 前進
                    self.position[2] += 0.01
                elif key == keyboard.Key.down:
                    temp_position[2] = -0.01  # z: 後退
                    self.position[2] -= 0.01
                elif key == keyboard.Key.left:
                    temp_position[0] = -0.01  # x: 左移動
                    self.position[0] -= 0.01
                elif key == keyboard.Key.right:
                    temp_position[0] = 0.01  # x: 右移動
                    self.position[0] += 0.01
                elif key == keyboard.Key.esc:
                    print("Exiting...")
                    self.save_to_json(self.position, self.rotation)
                    return False  # Stop listener

            updated_parameters = {"position": temp_position, "rotation": temp_rotation}
            message = json.dumps(updated_parameters)
            self.send_udp_message(message, self.ip, self.port)
            print(f"Updated position: {temp_position}, rotation: {temp_rotation}")
            self.save_to_json(self.position, self.rotation)

        with keyboard.Listener(on_press=on_press) as listener:
            listener.join()
