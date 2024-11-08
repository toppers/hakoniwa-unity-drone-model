using hakoniwa.environment.impl;
using hakoniwa.environment.interfaces;
using hakoniwa.pdu.core;
using hakoniwa.pdu.interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using hakoniwa.ar.bridge;
using System.Threading.Tasks;

namespace Hakoniwa.AR.Core
{
    public class HakoniwaAvatarAr : MonoBehaviour, IHakoniwaArBridgePlayer
    {
        public HakoAvatorObject drone_avatar;
        public DroneAvatorRotors drone_rotors;
        public HakoAvatorObject[] baggages;
        public HakoAvatorObject tb3;
        public HakoAvatorObject signal;
        public GameObject player;
        IEnvironmentService service;
        private HakoniwaArBridge bridge;
        private Vector3 initial_position;
        private Quaternion initial_rotation;

        private IPduManager mgr;
        string robotName = "DroneTransporter";


        public Task<bool> StartService(string serverUri)
        {
            service = EnvironmentServiceFactory.Create("websocket_dotnet", "unity", ".");
            Debug.Log($"Loaded WebSocket Server URI1: {service.GetCommunication().GetServerUri()}");
            mgr = new PduManager(service, ".");
            var ret = mgr.StartService(serverUri);
            Debug.Log($"Loaded WebSocket Server URI2: {service.GetCommunication().GetServerUri()}");
            return ret;
        }

        public bool StopService()
        {
            if (mgr != null)
            {
                mgr.StopService();
            }
            return true;
        }

        private void WritePlayer()
        {
            string robo_name = "CenterEyeAnchor";
            string pdu_name = "pos";
            IPdu player_pdu = mgr.CreatePdu(robo_name, pdu_name);
            if (player_pdu != null)
            {
                player_pdu.GetData<IPdu>("linear").SetData<double>("x", (double)player.transform.position.z);
                player_pdu.GetData<IPdu>("linear").SetData<double>("y", -(double)player.transform.position.x);
                player_pdu.GetData<IPdu>("linear").SetData<double>("z", (double)player.transform.position.y);
                player_pdu.GetData<IPdu>("angular").SetData<double>("x", -(double)player.transform.position.z);
                player_pdu.GetData<IPdu>("angular").SetData<double>("y", (double)player.transform.position.x);
                player_pdu.GetData<IPdu>("angular").SetData<double>("z", -(double)player.transform.position.y);
                mgr.WritePdu(robo_name, player_pdu);
                mgr.FlushPdu(robo_name, pdu_name);
            }

        }

        public void UpdatePosition(HakoVector3 position, HakoVector3 rotation)
        {
            //Debug.Log($"pos:  {position.X} {position.Y} {position.Z}");
            player.transform.position = new Vector3(position.X, position.Y, position.Z);
            Vector3 newRotation = player.transform.eulerAngles;
            newRotation.y = rotation.Y;
            player.transform.rotation = Quaternion.Euler(newRotation);
        }

        public void ResetPostion()
        {
            Debug.Log("Reset position");
            player.transform.position = initial_position;
            player.transform.rotation = initial_rotation;
        }

        public void UpdateAvatars()
        {
            //AVATARS

            //Debug.Log("Update Avatars");
            //Debug.Log("Twist: " + twist);
            foreach (var baggage in baggages)
            {
                //Debug.Log("baggage name: " + baggage.name);
                update_avatar_pos_rot(baggage.name, "pos", baggage, 0);
            }
            update_avatar_pos_rot(tb3.name, "pos", tb3, 0);
            update_signal();
            IPdu twist = mgr.ReadPdu(robotName, "drone_pos");
            //Debug.Log("Twist data: " + twist);
            IPdu controls = mgr.ReadPdu(robotName, "drone_motor");
            UpdateDroneAvatar(twist, controls);


            //Players
            WritePlayer();
        }

        void Start()
        {
            initial_position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            initial_rotation = new Quaternion(player.transform.rotation.x, player.transform.rotation.y, player.transform.rotation.z, player.transform.rotation.w);
            bridge = new HakoniwaArBridge();
            bridge.Register(this);
            bridge.Start();
        }
        void update_avatar_pos_rot(string robot_name, string pdu_name, HakoAvatorObject avatar, int state)
        {
            IPdu twist = mgr.ReadPdu(robot_name, pdu_name);
            if (twist != null)
            {
                var pos_rot = new HakoPositionAndRotation();
                pos_rot.position = new Vector3(
                    -(float)twist.GetData<IPdu>("linear").GetData<double>("y"), //twistData.Linear.Y,
                    (float)twist.GetData<IPdu>("linear").GetData<double>("z"), //twistData.Linear.Z,
                    (float)twist.GetData<IPdu>("linear").GetData<double>("x") //twistData.Linear.X
                );
                pos_rot.rotation = new Vector3(
                    (180 / MathF.PI) * (float)twist.GetData<IPdu>("angular").GetData<double>("y"), //twistData.Angular.Y,
                    -(180 / MathF.PI) * (float)twist.GetData<IPdu>("angular").GetData<double>("z"), //twistData.Angular.Z,
                    -(180 / MathF.PI) * (float)twist.GetData<IPdu>("angular").GetData<double>("x") //twistData.Angular.X
                );
                pos_rot.state = state;
                avatar.SetPosAndRot(pos_rot);
            }

        }
        void update_signal()
        {
            IPdu signal_data = mgr.ReadPdu(signal.name, "signal_data");
            if (signal_data != null)
            {
                uint state = signal_data.GetData<uint>("data");
                update_avatar_pos_rot(signal.name, "pos", signal, (int)state);
            }

        }

        void FixedUpdate()
        {
            bridge.Run();
        }
        private void UpdateDroneAvatar(IPdu twist, IPdu controls)
        {
            if (twist != null)
            {
                var pos_rot = new HakoPositionAndRotation();
                pos_rot.position = new Vector3(
                    -(float)twist.GetData<IPdu>("linear").GetData<double>("y"), //twistData.Linear.Y,
                    (float)twist.GetData<IPdu>("linear").GetData<double>("z"), //twistData.Linear.Z,
                    (float)twist.GetData<IPdu>("linear").GetData<double>("x") //twistData.Linear.X
                );
                pos_rot.rotation = new Vector3(
                    (180 / MathF.PI) * (float)twist.GetData<IPdu>("angular").GetData<double>("y"), //twistData.Angular.Y,
                    -(180 / MathF.PI) * (float)twist.GetData<IPdu>("angular").GetData<double>("z"), //twistData.Angular.Z,
                    -(180 / MathF.PI) * (float)twist.GetData<IPdu>("angular").GetData<double>("x") //twistData.Angular.X
                );
                drone_avatar.SetPosAndRot(pos_rot);
            }
            if (controls != null)
            {
                float c = (float)controls.GetDataArray<float>("controls")[0];
                drone_rotors.SetState((int)(c * 100.0));
            }
        }
        private void OnApplicationQuit()
        {
            bridge.Stop();
        }

    }

}
