import pygame
from input_handler import InputHandler
import json

class JoystickInputHandler(InputHandler):
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

    def handle_input(self):
        running = True
        while running:
            for event in pygame.event.get():
                if event.type == pygame.JOYAXISMOTION:
                    temp_position = [0, 0, 0]
                    temp_rotation = [0, 0, 0]

                    temp_position[0] = self.joystick.get_axis(0) * 0.01  # x: 左右
                    temp_position[2] = self.joystick.get_axis(1) * 0.01  # z: 前後
                    temp_position[1] = self.joystick.get_axis(3) * 0.01  # y: 上下
                    temp_rotation[1] = self.joystick.get_axis(2) * 1.0  # yaw: 左右回転

                    self.position[0] += temp_position[0]
                    self.position[1] += temp_position[1]
                    self.position[2] += temp_position[2]
                    self.rotation[1] += temp_rotation[1]

                    updated_parameters = {"position": temp_position, "rotation": temp_rotation}
                    message = json.dumps(updated_parameters)
                    self.send_udp_message(message, self.ip, self.port)
                    print(f"Updated position: {temp_position}, rotation: {temp_rotation}")
                    self.save_to_json(self.position, self.rotation)

            pygame.time.wait(10)
