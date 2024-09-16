#if UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityWebSocket;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hakoniwa.AR.Core;
using UnityEngine;
using Newtonsoft.Json;

namespace Hakoniwa.Web.Core
{

    public class HakoniwaAvatarWebclient : MonoBehaviour
    {
        [Serializable]
        public class HakoAvatarPdu
        {
            public string name;
            public string type;
        }
        [Serializable]
        public class HakoAvatar
        {
            public HakoAvatarPdu[] pdu;
            public HakoAvatorObject avatar;
            public int state;
        }


        public class HakoPacket
        {
            [JsonProperty("pdu_type")]
            public string PduType { get; set; }

            [JsonProperty("pdu_name")]
            public string PduName { get; set; }

            [JsonProperty("pdu_data")]
            public string PduData { get; set; }

            private static string FixJsonBrackets(string jsonData)
            {
                // JSON文字列内の括弧の対応をチェック
                Stack<char> stack = new Stack<char>();
                int lastIndexToKeep = jsonData.Length - 1;

                for (int i = 0; i < jsonData.Length; i++)
                {
                    char c = jsonData[i];
                    if (c == '{')
                    {
                        stack.Push(c);
                    }
                    else if (c == '}')
                    {
                        if (stack.Count > 0 && stack.Peek() == '{')
                        {
                            stack.Pop();
                        }
                        else
                        {
                            // 余分な閉じ括弧を検出したら、その位置を記録
                            lastIndexToKeep = i - 1;
                            break;
                        }
                    }
                }

                // 必要以上の閉じ括弧がある場合、それを削除
                return jsonData.Substring(0, lastIndexToKeep + 1);
            }

            public static HakoPacket FromJson(string jsonData)
            {
                //Debug.Log("HakoPacket.FromJson: " + jsonData);
                HakoPacket packet = new HakoPacket();

                try
                {
                    // pdu_typeの抽出
                    int typeStart = jsonData.IndexOf("\"pdu_type\":") + "\"pdu_type\":".Length;
                    int typeEnd = jsonData.IndexOf(",", typeStart);
                    packet.PduType = jsonData.Substring(typeStart, typeEnd - typeStart).Trim(' ', '"');

                    // pdu_nameの抽出
                    int nameStart = jsonData.IndexOf("\"pdu_name\":") + "\"pdu_name\":".Length;
                    int nameEnd = jsonData.IndexOf("}", nameStart);  // pdu_nameの終了位置を"}"の位置で探す
                    if (nameEnd == -1)  // 万が一見つからない場合は例外を投げる
                    {
                        throw new Exception("pdu_nameの終了位置が見つかりませんでした");
                    }
                    packet.PduName = jsonData.Substring(nameStart, nameEnd - nameStart).Trim(' ', '"');

                    // pdu_dataの抽出（pdu_dataはJSONオブジェクト全体）
                    int dataStart = jsonData.IndexOf("\"pdu_data\":") + "\"pdu_data\":".Length;
                    int dataEnd = jsonData.LastIndexOf("}");
                    if (dataEnd == -1)
                    {
                        throw new Exception("pdu_dataの終了位置が見つかりませんでした");
                    }
                    packet.PduData = jsonData.Substring(dataStart, dataEnd - dataStart + 1).Trim();  // "{"から"}"まで含めて抽出
                    packet.PduData = FixJsonBrackets(packet.PduData);


                    //Debug.Log("pduName: " + packet.PduName);
                    //Debug.Log("PduType: " + packet.PduType);
                    //Debug.Log("PduData: " + packet.PduData);

                }
                catch (Exception ex)
                {
                    Debug.LogError("Error parsing JSON: " + ex.Message);
                }

                return packet;
            }

            public T GetPduData<T>()
            {
                try
                {
                    // PduDataは既にJSON形式の文字列なので、そのままデシリアライズする
                    return JsonConvert.DeserializeObject<T>(PduData);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error converting PDU data to type: " + typeof(T) + ". " + ex.Message);
                    return default;
                }
            }
        }

        public class Vector3d
        {
            [JsonProperty("x")]
            public double X { get; set; }

            [JsonProperty("y")]
            public double Y { get; set; }

            [JsonProperty("z")]
            public double Z { get; set; }

            public Vector3d() { }

            public Vector3d(double x, double y, double z)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }
        }

        public class Twist
        {
            [JsonProperty("linear")]
            public Vector3d Linear { get; set; }

            [JsonProperty("angular")]
            public Vector3d Angular { get; set; }

            public Twist()
            {
                this.Linear = new Vector3d();
                this.Angular = new Vector3d();
            }

            public Twist(Vector3d linear, Vector3d angular)
            {
                this.Linear = linear;
                this.Angular = angular;
            }
        }
        public class HakoHilActuatorControls
        {
            [JsonProperty("time_usec")]
            public ulong TimeUsec { get; set; }

            [JsonProperty("controls")]
            public float[] Controls { get; set; }

            [JsonProperty("mode")]
            public byte Mode { get; set; }

            [JsonProperty("flags")]
            public ulong Flags { get; set; }

            public HakoHilActuatorControls()
            {
                Controls = new float[16];  // controls配列は16個のfloatを持つ
            }

            public HakoHilActuatorControls(ulong timeUsec, float[] controls, byte mode, ulong flags)
            {
                TimeUsec = timeUsec;
                Controls = controls.Length == 16 ? controls : new float[16];
                Mode = mode;
                Flags = flags;
            }
        }

        public ulong max_buffer_size = 1024 * 1024 * 10;
        public HakoAvatar[] avatars;
        public string websocket_url;
        //private ClientWebSocket ws;
        private WebSocketClient ws;
        private bool is_ws_alive = false;
        private HakoAvatar get_avatar(string pdu_name)
        {
            foreach(var e in avatars) {
                foreach (var pdu in e.pdu)
                {
                    //Debug.Log("avatar: pdu_name: " + pdu.name);
                    if (pdu.name == pdu_name)
                    {
                        return e;
                    }
                }
            }
            return null;
        }
        async void Start()
        {
            ws = new WebSocketClient();

            try
            {
                // WebSocketの接続を開始
                var uri = new Uri(websocket_url);
                await ws.ConnectAsync(uri, CancellationToken.None);
                Debug.Log("WebSocket connected!");
                is_ws_alive = true;

                // メッセージ受信を開始
                await ReceiveMessages();
            }
            catch (Exception ex)
            {
                is_ws_alive = false;
                Debug.Log($"WebSocket connection closed: {ex.Message}");
            }
        }

        async Task ReceiveMessages()
        {
            while (is_ws_alive)
            {
                // まず、メッセージのサイズ（8バイト）を受信
                var headerBuffer = new byte[8];
                await ReceiveExactBytesAsync(headerBuffer);

                ulong messageLength = BitConverter.ToUInt64(headerBuffer, 0);
                //Debug.Log("Received raw header: " + BitConverter.ToString(headerBuffer) + ", interpreted length: " + messageLength);
                if (messageLength > max_buffer_size)
                {
                    Debug.LogError("Message length exceeds maximum buffer size. Disconnecting. messageLength=" + messageLength);
                    await ws.CloseAsync(WebSocketCloseStatus.MessageTooBig, "Message too big", CancellationToken.None);
                    return;
                }

                if (messageLength > 0)
                {
                    var payloadBuffer = new byte[messageLength];
                    await ReceiveExactBytesAsync(payloadBuffer);

                    var message = Encoding.UTF8.GetString(payloadBuffer);
                    //Debug.Log("WebSocket message received: " + message);

                    // メッセージを処理してアバターを更新
                    UpdateAvatar(message);
                }
                else
                {
                    Debug.LogWarning("Received invalid message length.");
                }
            }
        }

        async Task ReceiveExactBytesAsync(byte[] buffer)
        {
            int offset = 0;
            while (offset < buffer.Length)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer, offset, buffer.Length - offset), CancellationToken.None);
                offset += result.Count;

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                    break;
                }
            }

            if (offset != buffer.Length)
            {
                throw new InvalidOperationException("WebSocket connection closed before full message was received.");
            }
        }
        private Vector3 ConvertRos2Unity(Vector3 ros_data)
        {
            return new Vector3(
                -ros_data.y, // unity.x
                ros_data.z, // unity.y
                ros_data.x  // unity.z
                );
        }
        private void UpdateAvatar(string jsonData)
        {
            // jsonDataをパースして、対応するアバターを更新する
            HakoPacket packet = HakoPacket.FromJson(jsonData);
            if (packet == null)
            {
                Debug.LogError("packet is null...");
                return;
            }
            var e = get_avatar(packet.PduName.Trim());
            if (e != null)
            {
                if (packet.PduType == "geometry_msgs/Twist") {
                    Twist twistData = packet.GetPduData<Twist>();
                    if (twistData == null)
                    {
                        Debug.LogError("twistData is null...");
                        return;
                    }
                    //Debug.Log("linear.x: " + twistData.Linear.X);
                    //Debug.Log("linear.y: " + twistData.Linear.Y);
                    //Debug.Log("linear.z: " + twistData.Linear.Z);
                    //Debug.Log("angular.z: " + twistData.Angular.Z);
                    var pos_rot = new HakoPositionAndRotation();
                    pos_rot.position = new Vector3(
                        -(float)twistData.Linear.Y, 
                        (float)twistData.Linear.Z, 
                        (float)twistData.Linear.X 
                    );
                    pos_rot.rotation = new Vector3(
                        (180 / MathF.PI) * (float)twistData.Angular.Y, 
                        -(180 / MathF.PI) * (float)twistData.Angular.Z, 
                        -(180 / MathF.PI) * (float)twistData.Angular.X  
                    );
                    //Debug.Log("pos_rot.rotation.y: " + pos_rot.rotation.y);
                    pos_rot.state = e.state;
                    // アバターの位置や回転を更新
                    e.avatar.SetPosAndRot(pos_rot);
                }
                else if (packet.PduType == "hako_mavlink_msgs/HakoHilActuatorControls")
                {
                    HakoHilActuatorControls c = packet.GetPduData<HakoHilActuatorControls>();
                    float control_val = c.Controls[0];
                    Debug.Log("control_val: " + control_val);
                    e.state = (int)(control_val * 100.0);
                }
                else
                {
                    Debug.LogWarning("Avatar not found for topic type: " + packet.PduType);
                }
            }
            else
            {
                Debug.LogWarning("Avatar not found for topic: " + packet.PduName);
            }
        }

        // WebSocket接続終了時のクリーンアップ処理
        void CleanupWebSocket()
        {
            if (ws != null)
            {
                ws.Dispose();
                ws = null;  // WebSocketインスタンスをクリア
                Debug.Log("WebSocket closed and resources disposed.");
            }
        }

        void OnDestroy()
        {
            CleanupWebSocket();
        }
    }
}

#endif