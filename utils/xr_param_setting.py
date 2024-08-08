import socket
import json
import sys
import argparse

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

if __name__ == "__main__":
    # コマンドライン引数のパーサーを設定
    parser = argparse.ArgumentParser(description='Send UDP message with parameters from JSON file.')
    parser.add_argument('json_file', help='Path to the JSON file with parameters.')
    parser.add_argument('address', help='Destination IP address and port in the format IP:PORT.')

    args = parser.parse_args()

    # JSONファイルを読み込み
    with open(args.json_file, 'r') as f:
        parameters = json.load(f)
    
    # 宛先IPアドレスとポートを分割
    ip, port = args.address.split(':')
    port = int(port)

    # JSON形式に変換
    message = json.dumps(parameters)
    
    # UDPメッセージを送信
    send_udp_message(message, ip, port)
    print(f"Sent message: {message} to {ip}:{port}")
