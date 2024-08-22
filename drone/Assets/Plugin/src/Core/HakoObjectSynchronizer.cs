using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hakoniwa.AR.Core
{
    public class HakoObjectSynchronizer : MonoBehaviour
    {
        public GameObject[] players;
        public GameObject[] avators;
        public string server_ipaddr;
        public int server_portno;
        public int timeout_sec;
        public float scale = 1;

        public string client_ipaddr;
        public int client_portno;

        private HakoUdpServer server;
        private HakoUdpClient client;


        private Dictionary<string, HakoAvatorObject> avatorMap = new Dictionary<string, HakoAvatorObject>();
        private Dictionary<string, HakoPlayerObject> playerMap = new Dictionary<string, HakoPlayerObject>();

        async void Start()
        {
            this.server = new HakoUdpServer(server_ipaddr, server_portno, timeout_sec);
            this.client = new HakoUdpClient(client_ipaddr, client_portno);

            if (this.avators.Length > 0)
            {
                foreach (var obj in this.avators)
                {
                    var avator = obj.GetComponentInChildren<HakoAvatorObject>();
                    if (avator == null)
                    {
                        throw new System.Exception("Not Found avator");
                    }
                    this.avatorMap.Add(obj.name, avator);
                }
            }
            if (this.players.Length > 0)
            {
                foreach (var obj in this.players)
                {
                    var player = obj.GetComponentInChildren<HakoPlayerObject>();
                    if (player == null)
                    {
                        throw new System.Exception("Not Found player");
                    }
                    this.playerMap.Add(obj.name, player);
                }
            }
            await this.server.StartServer();

        }
        void OnApplicationQuit()
        {
            this.server.StopServer();
        }
        int count = 0;
        public int send_cycle = 5;
        void FixedUpdate()
        {
            count++;
            if ((count % send_cycle) != 0)
            {
                return;
            }
            var data = this.server.RecvData();
            if (data != null)
            {
                Debug.Log("recv data len=" + data.Length);
                for (int off = 0; off < data.Length;)
                {
                    HakoPositionAndRotation pr = new HakoPositionAndRotation();
                    int size = pr.Decode(data, off, this.scale);
                    //Debug.Log("off=" + off + " size=" + size);
                    off += size;
                    Debug.Log("avator name=" + pr.name);
                    var obj = this.avatorMap[pr.name];
                    obj.SetPosAndRot(pr);
                }
            }
            List<byte[]> dataBytesList = new List<byte[]>();
            foreach (var name in this.playerMap.Keys)
            {
                var pr = this.playerMap[name].GetPosAndRot();
                {
                    var pr_data = pr.Encode(this.scale);
                    dataBytesList.Add(pr_data);
                }
                this.playerMap[name].SetPrevValue(pr);
            }
            byte[] combinedData = dataBytesList.SelectMany(bytes => bytes).ToArray();
            this.client.SendData(combinedData);
            //Debug.Log(" data send len=" + combinedData.Length);
        }
    }
}

