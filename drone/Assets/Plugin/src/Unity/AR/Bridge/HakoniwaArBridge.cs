using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hakoniwa.ar.bridge
{

    public class HakoniwaArBridge: IHakoniwaArBridge
    {
        private readonly int heartbeatTimeoutSeconds = 5;
        private DateTime lastHeartbeatTime = DateTime.Now; // 初期値として現在時刻を設定
        private IHakoniwaArBridgePlayer player;
        private UdpComm udp_service;
        private HakoniwaArBridgeStateManager state_manager;
        private bool isStartedWebSocket = false;
        private string serverUri;

        public HakoniwaArBridge()
        {
            state_manager = new HakoniwaArBridgeStateManager();
            udp_service = new UdpComm();
        }

        public bool Register(IHakoniwaArBridgePlayer player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (this.player != null)
            {
                Console.WriteLine("A player is already registered.");
                return false;
            }

            this.player = player;
            Console.WriteLine("Player registered successfully.");
            return true;
        }

        private void HeartBeatCheck()
        {
            var packet = udp_service.GetLatestPacket("heartbeat_request");

            if (packet != null && packet.Data != null && packet.Data.ContainsKey("ip_address"))
            {
                // ハートビートを受信したため、タイムスタンプを更新
                lastHeartbeatTime = DateTime.Now;

                var ipAddress = packet.Data["ip_address"] as string;
                serverUri = $"ws://{ipAddress}:8765";
                if (isStartedWebSocket == false)
                {
                    try
                    {
                        player.StartService(serverUri);
                        isStartedWebSocket = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error starting player service: {ex.Message}");
                    }
                }

                // 現在の状態を文字列として取得し、HeartBeatResponseに設定
                var reply = new HeartBeatResponse(state_manager.GetState().ToString());
                udp_service.SendPacket(reply);
                //Console.WriteLine($"Heartbeat response sent with state: {state_manager.GetState().ToString()} to {serverUri}");
            }
            else
            {
                if (packet != null) {
                    Console.WriteLine("Invalid or missing IP address in heartbeat_request packet.");
                }
            }

            // タイムアウトチェック: 最後のハートビートから5秒以上経過している場合
            if ((DateTime.Now - lastHeartbeatTime).TotalSeconds > heartbeatTimeoutSeconds)
            {
                //Console.WriteLine("Heartbeat timeout detected. Switching to POSITIONING state.");
                ResetEvent();
            }
        }
        void RunPositioning()
        {
            var event_packet = udp_service.GetLatestPacket("play_start");
            if (event_packet != null) {
                PlayStartEvent();
                return;
            }

            PositioningRequestData packet = udp_service.GetLatestPositioningPacket();
            if (packet == null) 
            {
                Console.WriteLine("No position data available in the packet.");
                return;
            }

            HakoVector3 pos = new HakoVector3(
                (float)packet.Position["x"],
                (float)packet.Position["y"],
                (float)packet.Position["z"]
            );

            HakoVector3 rot = new HakoVector3(
                (float)packet.Orientation["x"],
                (float)packet.Orientation["y"],
                (float)packet.Orientation["z"]
            );

            player.UpdatePosition(pos, rot);
            //Console.WriteLine("Position and orientation data have been updated.");
        }

        private void PlayStartEvent()
        {
            state_manager.EventPlayStart();
        }
        private void ResetEvent()
        {
            if (state_manager.GetState() == BridgeState.PLAYING) {
                player.ResetPostion();
                player.StopService();
                isStartedWebSocket = false;
            }
            else {
                //nothing to do
            }
            udp_service.ClearBuffers();
            state_manager.EventReset();
        }
        private void RunPlaying()
        {
            var event_packet = udp_service.GetLatestPacket("reset");
            if (event_packet != null) {
                ResetEvent();
            }
            else {
                player.UpdateAvatars();
            }
        }
        public void Run()
        {
            HeartBeatCheck();
            if (state_manager.GetState() == BridgeState.POSITIONING) {
                RunPositioning();
            }
            RunPlaying();
        }

        public bool Start()
        {
            if (player == null)
            {
                Console.WriteLine("No player registered. Cannot start service.");
                return false;
            }
            udp_service.Start();
            Console.WriteLine("Hakoniwa AR Bridge service starting...");
            return true;
        }

        public bool Stop()
        {
            if (player == null)
            {
                Console.WriteLine("No player registered. Nothing to stop.");
                return false;
            }

            udp_service.Stop();
            Console.WriteLine("Hakoniwa AR Bridge service stopping...");
            return player.StopService();
        }
        public BridgeState GetState()
        {
            return state_manager.GetState();
        }
    }
}
