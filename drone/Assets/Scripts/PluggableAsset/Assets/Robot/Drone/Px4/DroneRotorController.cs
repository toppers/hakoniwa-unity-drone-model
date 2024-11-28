using System;
using System.Collections;
using System.Collections.Generic;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts
{
    public class DroneRotorController : MonoBehaviour, IRobotPartsController, IRobotPartsConfig
    {
        private GameObject root;
        private string root_name;
        public GameObject motor_0;
        public GameObject motor_1;
        public GameObject motor_2;
        public GameObject motor_3;

        private DroneRotor motor_parts_0;
        private DroneRotor motor_parts_1;
        private DroneRotor motor_parts_2;
        private DroneRotor motor_parts_3;

        private PduIoConnector pdu_io;
        private IPduReader pdu_reader_actuator;

        public string[] topic_type = {
                "hako_mavlink_msgs/HakoHilActuatorControls"
        };
        public string[] topic_name = {
            "motor",
        };
        public void DoControl()
        {
            float[] controls = this.pdu_reader_actuator.GetReadOps().GetDataFloat32Array("controls");
            motor_parts_0.AddForce(controls[0]);
            motor_parts_1.AddForce(controls[1]);
            motor_parts_2.AddForce(controls[2]);
            motor_parts_3.AddForce(controls[3]);
        }
        public int update_cycle = 1;
        public IoMethod io_method = IoMethod.SHM;
        public CommMethod comm_method = CommMethod.DIRECT;

        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            RoboPartsConfigData[] configs = new RoboPartsConfigData[topic_name.Length];
            int i = 0; //motor
            configs[i] = new RoboPartsConfigData();
            configs[i].io_dir = IoDir.READ;
            configs[i].io_method = this.io_method;
            configs[i].value.org_name = this.topic_name[i];
            configs[i].value.type = this.topic_type[i];
            configs[i].value.class_name = ConstantValues.pdu_reader_class;
            configs[i].value.conv_class_name = ConstantValues.conv_pdu_reader_class;
            configs[i].value.pdu_size = 88 + ConstantValues.PduMetaDataSize;
            configs[i].value.write_cycle = this.update_cycle;
            configs[i].value.method_type = this.comm_method.ToString();
            return configs;
        }

        public RosTopicMessageConfig[] getRosConfig()
        {
            var pcfg = GetRoboPartsConfig();
            RosTopicMessageConfig[] cfg = new RosTopicMessageConfig[pcfg.Length];
            for (int i = 0; i < pcfg.Length; i++)
            {
                cfg[i] = new RosTopicMessageConfig();
                cfg[i].topic_message_name = this.topic_name[i];
                cfg[i].topic_type_name = this.topic_type[i];
                if (pcfg[i].io_dir == IoDir.READ)
                {
                    cfg[i].sub = true;
                }
                else
                {
                    cfg[i].sub = false;
                }
            }

            return cfg;
        }

        public void Initialize(object obj)
        {
            GameObject tmp = null;
            try
            {
                tmp = obj as GameObject;
            }
            catch (InvalidCastException e)
            {
                Debug.LogError("Initialize error: " + e.Message);
                return;
            }

            if (this.root == null)
            {
                this.root = tmp;
                this.root_name = string.Copy(this.root.transform.name);
                this.pdu_io = PduIoConnector.Get(root_name);
                if (this.pdu_io == null)
                {
                    throw new ArgumentException("can not found pdu_io:" + root_name);
                }

                var pdu_reader_name = root_name + "_" + this.topic_name[0] + "Pdu";
                this.pdu_reader_actuator = this.pdu_io.GetReader(pdu_reader_name);
                if (this.pdu_reader_actuator == null)
                {
                    throw new ArgumentException("can not found pdu_reader:" + pdu_reader_name);
                }
                motor_parts_0 = motor_0.GetComponent<DroneRotor>();
                motor_parts_1 = motor_1.GetComponent<DroneRotor>();
                motor_parts_2 = motor_2.GetComponent<DroneRotor>();
                motor_parts_3 = motor_3.GetComponent<DroneRotor>();
            }
            motor_parts_0.Initialize(motor_0);
            motor_parts_1.Initialize(motor_1);
            motor_parts_2.Initialize(motor_2);
            motor_parts_3.Initialize(motor_3);
        }
    }
}
