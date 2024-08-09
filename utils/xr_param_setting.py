import socket
import json
import sys
import argparse
from pynput import keyboard

def send_udp_message(message, ip, port):
    print(f'INFO: send udp message: {message}')
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.sendto(message.encode('utf-8'), (ip, port))

    # 応答を待機
    sock.settimeout(5)  # タイムアウトを5秒に設定
    try:
        response, _ = sock.recvfrom(4096)
        print(f"OK: Received response: {response.decode('utf-8')}")
    except socket.timeout:
        print("ERROR: No response received within timeout period.")
    finally:
        sock.close()

def save_to_json(position, rotation):
    data = {"position": position, "rotation": rotation}
    with open('output.json', 'w') as f:
        json.dump(data, f, indent=4)
    print(f"Saved current state to output.json")

def adjust_position_and_rotation(ip, port, position, rotation):
    print("Use arrow keys to adjust position (x, z), 'w'/'s' for height (y), 'q'/'e' for yaw rotation.")
    print("Press 'Esc' to exit.")

    def on_press(key):
        # キー入力ごとにゼロ初期化された値で送信
        temp_position = [0, 0, 0]
        temp_rotation = [0, 0, 0]

        if hasattr(key, 'char'):
            if key.char == 'w':
                print("Key 'w' pressed - Moving up")
                temp_position[1] = 0.01  # y: 上昇
                position[1] += 0.01  # 累積は内部のみ
            elif key.char == 's':
                print("Key 's' pressed - Moving down")
                temp_position[1] = -0.01  # y: 下降
                position[1] -= 0.01  # 累積は内部のみ
            elif key.char == 'q':
                print("Key 'q' pressed - Rotating left")
                temp_rotation[1] = 1.0  # yaw: 左回転
                rotation[1] += 1.0  # 累積は内部のみ
            elif key.char == 'e':
                print("Key 'e' pressed - Rotating right")
                temp_rotation[1] = -1.0  # yaw: 右回転
                rotation[1] -= 1.0  # 累積は内部のみ
        else:
            if key == keyboard.Key.up:
                print("Arrow Up pressed - Moving forward")
                temp_position[2] = 0.01  # z: 前進
                position[2] += 0.01  # 累積は内部のみ
            elif key == keyboard.Key.down:
                print("Arrow Down pressed - Moving backward")
                temp_position[2] = -0.01  # z: 後退
                position[2] -= 0.01  # 累積は内部のみ
            elif key == keyboard.Key.left:
                print("Arrow Left pressed - Moving left")
                temp_position[0] = -0.01  # x: 左移動
                position[0] -= 0.01  # 累積は内部のみ
            elif key == keyboard.Key.right:
                print("Arrow Right pressed - Moving right")
                temp_position[0] = 0.01  # x: 右移動
                position[0] += 0.01  # 累積は内部のみ
            elif key == keyboard.Key.esc:
                print("Exiting...")
                save_to_json(position, rotation)  # 最後に状態を保存
                return False  # Stop listener

        updated_parameters = {"position": temp_position, "rotation": temp_rotation}
        message = json.dumps(updated_parameters)
        send_udp_message(message, ip, port)
        print(f"Updated position: {temp_position}, rotation: {temp_rotation}")
        save_to_json(position, rotation)  # 各更新後に現在の状態を保存

    with keyboard.Listener(on_press=on_press) as listener:
        listener.join()

if __name__ == "__main__":
    # コマンドライン引数のパーサーを設定
    parser = argparse.ArgumentParser(description='Send UDP message with parameters from JSON file.')
    parser.add_argument('json_file', help='Path to the JSON file with parameters.')
    parser.add_argument('address', help='Destination IP address and port in the format IP:PORT.')

    args = parser.parse_args()

    # JSONファイルを読み込み
    with open(args.json_file, 'r') as f:
        parameters = json.load(f)
    
    initial_position = parameters.get("position", [0, 0, 0])
    initial_rotation = parameters.get("rotation", [0, 0, 0])

    # 宛先IPアドレスとポートを分割
    ip, port = args.address.split(':')
    port = int(port)

    # JSON形式に変換して一度だけ送信
    message = json.dumps(parameters)
    send_udp_message(message, ip, port)
    print(f"Initial message sent: {message} to {ip}:{port}")

    # パラメータの微調整モードに移行
    adjust_position_and_rotation(ip, port, initial_position, initial_rotation)
