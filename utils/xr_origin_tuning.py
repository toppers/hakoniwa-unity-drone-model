import socket
import json
import argparse
import sys

try:
    from utils.xr_drivers.keyboard_input_handler import KeyboardInputHandler
    from utils.xr_drivers.joystick_input_handler import JoystickInputHandler
except ImportError as e:
    print(f"ERROR: {e}")
    sys.exit(1)  # インポートに失敗した場合は、プログラムを終了します

def send_udp_message(message, ip, port):
    try:
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
    except Exception as e:
        print(f"ERROR: Failed to send UDP message: {e}")
    finally:
        sock.close()

def validate_address(address):
    try:
        ip, port = address.split(':')
        port = int(port)
        return ip, port
    except ValueError:
        print("ERROR: Address must be in the format IP:PORT")
        sys.exit(1)

def save_to_json(position, rotation):
    data = {"position": position, "rotation": rotation}
    with open('output.json', 'w') as f:
        json.dump(data, f, indent=4)
    print(f"Saved current state to output.json")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Send UDP message with parameters from JSON file.')
    parser.add_argument('json_file', help='Path to the JSON file with parameters.')
    parser.add_argument('address', help='Destination IP address and port in the format IP:PORT.')
    parser.add_argument('--input', choices=['keyboard', 'joystick'], default='keyboard',
                        help='Choose input method: keyboard or joystick')

    args = parser.parse_args()

    # JSONファイルのロード
    try:
        with open(args.json_file, 'r') as f:
            parameters = json.load(f)
    except FileNotFoundError:
        print(f"ERROR: The file {args.json_file} was not found.")
        sys.exit(1)
    except json.JSONDecodeError:
        print(f"ERROR: Failed to decode JSON from the file {args.json_file}.")
        sys.exit(1)

    initial_position = parameters.get("position", [0, 0, 0])
    initial_rotation = parameters.get("rotation", [0, 0, 0])

    ip, port = validate_address(args.address)

    message = json.dumps(parameters)
    send_udp_message(message, ip, port)
    print(f"Initial message sent: {message} to {ip}:{port}")

    try:
        if args.input == 'keyboard':
            input_handler = KeyboardInputHandler(initial_position, initial_rotation, send_udp_message, save_to_json, ip, port)
        else:
            input_handler = JoystickInputHandler(initial_position, initial_rotation, send_udp_message, save_to_json, ip, port)

        input_handler.handle_input()
    except Exception as e:
        print(f"ERROR: An unexpected error occurred during input handling: {e}")
        sys.exit(1)
