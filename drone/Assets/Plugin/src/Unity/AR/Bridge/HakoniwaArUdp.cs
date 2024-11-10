using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace hakoniwa.ar.bridge
{
    public class UdpComm
    {
        private UdpClient udpClient;
        private IPEndPoint receiveEndPoint;
        private IPEndPoint sendEndPoint;
        private Dictionary<string, BasePacket> packetBuffer;
        private readonly object bufferLock = new object();
        private bool running = false;
        private Thread receiveThread;
        private DateTime lastReceiveTime;

        private readonly int recvPort = 38528;
        private readonly int sendPort = 48528; 

        public UdpComm()
        {
            // ローカルIPを自動的に取得
            string localIp = GetLocalIpAddress();
            receiveEndPoint = new IPEndPoint(IPAddress.Parse(localIp), recvPort);
        }

        public DateTime GetLastReceiveTime()
        {
            return lastReceiveTime;
        }

        public void Start()
        {
            if (running) {
                return;
            }
            udpClient = new UdpClient(recvPort);
            packetBuffer = new Dictionary<string, BasePacket>();
            receiveThread = new Thread(ReceiveLoop) { IsBackground = true };
            receiveThread.Start();
            Console.WriteLine("UdpComm receiving started.");
            running = true;
        }
        public void ClearBuffers()
        {
            lock (bufferLock) {
                packetBuffer.Clear();
            }
        }

        public void Stop()
        {
            if (running) {
                running = false;
                udpClient.Close();
                if (receiveThread != null && receiveThread.IsAlive)
                {
                    receiveThread.Join();
                }
                packetBuffer.Clear();
                packetBuffer = null;
                Console.WriteLine("UdpComm stopped.");
            }
        }

        public void SendPacket(BasePacket packet)
        {
            if ((sendEndPoint == null) || (running == false))
            {
                Console.WriteLine("No send endpoint available. Please receive a packet first to set the endpoint.");
                return;
            }

            try
            {
                string json = packet.ToJson();
                byte[] data = Encoding.UTF8.GetBytes(json);
                udpClient.Send(data, data.Length, sendEndPoint);
                //Console.WriteLine($"Sent packet: {json}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending packet: {ex.Message}");
            }
        }

        private void ReceiveLoop()
        {
            lastReceiveTime = DateTime.Now;
            while (running)
            {
                try
                {
                    byte[] data = udpClient.Receive(ref receiveEndPoint);
                    string json = Encoding.UTF8.GetString(data);
                    BasePacket packet = JsonConvert.DeserializeObject<BasePacket>(json);

                    lock (bufferLock)
                    {
                        lastReceiveTime = DateTime.Now;
                        sendEndPoint = new IPEndPoint(receiveEndPoint.Address, sendPort);
                        if (packet.PacketType != null)
                        {
                            if (packet.PacketType == "data") {
                                packetBuffer[packet.DataType] = packet;
                            }
                            else {
                                packetBuffer[packet.EventType] = packet;
                            }
                        }
                    }

                    //Console.WriteLine($"Received packet from {receiveEndPoint.Address}:{receiveEndPoint.Port}: {json}");
                }
                catch (SocketException ex)
                {
                    if (running)
                    {
                        Console.WriteLine($"Socket error: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving data: {ex.Message}");
                }
            }
        }

        public BasePacket GetLatestPacket(string packetType)
        {
            lock (bufferLock)
            {
                if (packetBuffer.TryGetValue(packetType, out var packet))
                {
                    packetBuffer.Remove(packetType); // 取得後に削除
                    return packet;
                }
                return null; // パケットが見つからなければ null を返す
            }
        }
        public PositioningRequestData GetLatestPositioningPacket()
        {
            lock (bufferLock)
            {
                if (packetBuffer.TryGetValue("position", out var packet))
                {
                    packetBuffer.Remove("position"); // 取得後に削除

                    return PositioningRequest.FromBasePacket(packet);
                }
                return null; // パケットが見つからなければ null を返す
            }
        }

        private string GetLocalIpAddress()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530); // 外部アドレスに接続してローカルIPを取得
                return ((IPEndPoint)socket.LocalEndPoint).Address.ToString();
            }
        }
    }
}
