using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Hakoniwa.AR.Core
{
    public class HakoObjectSynchronizerForXR : MonoBehaviour
    {
        public GameObject[] players;
        public GameObject[] avators;
        public int timeout_sec;
        public float scale = 1;

        private HakoXrParams param;
        private string server_ipaddr;
        private int server_portno;
        private string client_ipaddr;
        private int client_portno;

        private HakoUdpServer server;
        private HakoUdpClient client;

        private Dictionary<string, HakoAvatorObject> avatorMap = new Dictionary<string, HakoAvatorObject>();
        private Dictionary<string, HakoPlayerObject> playerMap = new Dictionary<string, HakoPlayerObject>();

        private HakoXrParamServer paramServer;
        private bool isInitialized = false;
        private bool startInitialization = false;
        public GameObject player;

        void Start()
        {
            // HakoXrParamServer‚Ì‰Šú‰»‚Æ‹N“®
            paramServer = new HakoXrParamServer();
            paramServer.Initialize();
        }

        async Task InitializeAsync()
        {
            string ipport = param.server_url;
            server_ipaddr = ipport.Split(":")[0].Trim();
            server_portno = int.Parse(ipport.Split(":")[1].Trim());
            Debug.Log("server ip : " + server_ipaddr + " port:" + server_portno);

            ipport = param.client_url;
            client_ipaddr = ipport.Split(":")[0].Trim();
            client_portno = int.Parse(ipport.Split(":")[1].Trim());
            Debug.Log("client ip : " + client_ipaddr + " port:" + client_portno);

            this.server = new HakoUdpServer(server_ipaddr, server_portno, timeout_sec);
            this.client = new HakoUdpClient(client_ipaddr, client_portno);

            /*
             * Position Setting
             */
            player.transform.position = new Vector3(param.position[0], param.position[1], param.position[2]);
            /*
             * Rotation Setting
             */
            Vector3 newRotation = player.transform.eulerAngles;
            newRotation.y = param.rotation[1];
            player.transform.rotation = Quaternion.Euler(newRotation);
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
            isInitialized = true;
            await server.StartServer();
        }

        void OnApplicationQuit()
        {
            if (isInitialized)
            {
                this.server.StopServer();
            }
            paramServer.DestroyObjects();
        }
        public int send_cycle = 5;
        int count;
        async void FixedUpdate()
        {
            count++;
            if ((count % send_cycle) != 0)
            {
                return;
            }
            if (!startInitialization)
            {
                this.param = paramServer.GetHakoXrParams();
                if (this.param != null)
                {
                    startInitialization = true;
                    await InitializeAsync();
                }
            }
            else if (isInitialized)
            {
                var ap = this.paramServer.GetAdjustmentParams();
                if (ap != null)
                {
                    /*
                     * Position Setting
                     */
                    var cp = player.transform.position;
                    player.transform.position = new Vector3(cp.x + ap.position[0], cp.y + ap.position[1], cp.z + ap.position[2]);
                    /*
                     * Rotation Setting
                     */
                    Vector3 newRotation = player.transform.eulerAngles;
                    newRotation.y += ap.rotation[1];
                    player.transform.rotation = Quaternion.Euler(newRotation);
                }

                var data = this.server.RecvData();
                if (data != null)
                {
                    for (int off = 0; off < data.Length;)
                    {
                        HakoPositionAndRotation pr = new HakoPositionAndRotation();
                        int size = pr.Decode(data, off, this.scale);
                        off += size;
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
            }
        }
    }
 
}

