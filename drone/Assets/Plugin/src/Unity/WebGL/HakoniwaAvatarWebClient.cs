#if UNITY_WEBGL
using System;
using Hakoniwa.AR.Core;
using UnityEngine;
using hakoniwa.environment.interfaces;
using hakoniwa.environment.impl;
using hakoniwa.pdu.core;
using hakoniwa.pdu.interfaces;
using Hakoniwa.PluggableAsset.Assets.Robot.Parts;

namespace Hakoniwa.Web.Core
{

    public class HakoniwaAvatarWebclient : MonoBehaviour, IDroneBatteryStatus
    {
        public HakoAvatorObject drone_avatar;
        public DroneAvatorRotors drone_rotors;
        public HakoAvatorObject[] baggages;
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
        private double full_voltage;
        private double curr_voltage;
        private double curr_temperature;
        private uint status;
        private uint cycles;
        public double get_full_voltage()
        {
            return full_voltage;
        }

        public double get_curr_voltage()
        {
            return curr_voltage;
        }

        public uint get_status()
        {
            return status;
        }

        public uint get_cycles()
        {
            return cycles;
        }

        public double get_temperature()
        {
            return curr_temperature;
        }
        private void DoReadBatteryStatus(string robotName)
        {
            IPdu battery = mgr.ReadPdu(robotName, "hako_battery");
            if (battery != null)
            {
                full_voltage = battery.GetData<double>("full_voltage");
                curr_voltage = battery.GetData<double>("curr_voltage");
                curr_temperature = battery.GetData<double>("curr_temp");
                status = battery.GetData<UInt32>("status");
                cycles = battery.GetData<UInt32>("cycles");
            }
        }
        void FixedUpdate()
        {
            if (mgr != null)
            {
#if UNITY_EDITOR
                WebGLSocketCommunicationService srv = service.GetCommunication() as WebGLSocketCommunicationService;
                if (srv != null)
                {
                    srv.GetWebSocket().DispatchMessageQueue();
                }
#endif
                //Debug.Log("Twist: " + twist);
                foreach (var baggage in baggages)
                {
                    //Debug.Log("baggage name: " + baggage.name);
                    IPdu baggage_twist = mgr.ReadPdu(baggage.name, "pos");
                    if (baggage_twist != null)
                    {
                        var pos_rot = new HakoPositionAndRotation();
                        pos_rot.position = new Vector3(
                            -(float)baggage_twist.GetData<IPdu>("linear").GetData<double>("y"), //twistData.Linear.Y,
                            (float)baggage_twist.GetData<IPdu>("linear").GetData<double>("z"), //twistData.Linear.Z,
                            (float)baggage_twist.GetData<IPdu>("linear").GetData<double>("x") //twistData.Linear.X
                        );
                        //Debug.Log("name: " + baggage.name + " pos_x: " + pos_rot.position.x + " pos_y: " + pos_rot.position.y + "pos_z: " + pos_rot.position.z);
                        pos_rot.rotation = new Vector3(
                            (180 / MathF.PI) * (float)baggage_twist.GetData<IPdu>("angular").GetData<double>("y"), //twistData.Angular.Y,
                            -(180 / MathF.PI) * (float)baggage_twist.GetData<IPdu>("angular").GetData<double>("z"), //twistData.Angular.Z,
                            -(180 / MathF.PI) * (float)baggage_twist.GetData<IPdu>("angular").GetData<double>("x") //twistData.Angular.X
                        );
                        baggage.SetPosAndRot(pos_rot);
                    }
                }
                IPdu twist = mgr.ReadPdu(robotName, "drone_pos");
                IPdu controls = mgr.ReadPdu(robotName, "drone_motor");
                UpdateDroneAvatar(twist, controls);
                DoReadBatteryStatus(robotName);
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

#endif