using hakoniwa.environment.impl;
using hakoniwa.environment.interfaces;
using hakoniwa.pdu.core;
using hakoniwa.pdu.interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.AR.Core
{
    public class HakoniwaAvatarAr : MonoBehaviour
    {
        public HakoAvatorObject drone_avatar;
        public DroneAvatorRotors drone_rotors;
        public HakoAvatorObject[] baggages;
        public HakoAvatorObject tb3;
        public HakoAvatorObject signal;
        IEnvironmentService service;

        private IPduManager mgr;
        string robotName = "DroneTransporter";

        async void Start()
        {
            service = EnvironmentServiceFactory.Create("websocket_unity", "unity", ".");
            Debug.Log($"Loaded WebSocket Server URI (WebGL): {service.GetCommunication().GetServerUri()}");
            mgr = new PduManager(service, ".");
            await mgr.StartService();

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
            if (mgr != null)
            {
                //Debug.Log("Twist: " + twist);
                foreach (var baggage in baggages)
                {
                    //Debug.Log("baggage name: " + baggage.name);
                    update_avatar_pos_rot(baggage.name, "pos", baggage, 0);
                }
                update_avatar_pos_rot(tb3.name, "pos", tb3, 0);
                update_signal();
                IPdu twist = mgr.ReadPdu(robotName, "drone_pos");
                IPdu controls = mgr.ReadPdu(robotName, "drone_motor");
                UpdateDroneAvatar(twist, controls);
            }
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
            if (mgr != null)
            {
                mgr.StopService();
            }
        }
    }

}
