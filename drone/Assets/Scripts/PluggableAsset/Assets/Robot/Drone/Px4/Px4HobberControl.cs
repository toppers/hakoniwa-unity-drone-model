using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts
{
    public class Px4HobberControl : MonoBehaviour, IRobotPartsController, IRobotPartsConfig
    {
        private GameObject root;
        private string root_name;
        private PduIoConnector pdu_io;
        private IPduReader pdu_reader;

        public Rigidbody my_body;
        public GameObject motor_fl;
        public GameObject motor_fr;
        public GameObject motor_bl;
        public GameObject motor_br;

        private Px4Hobber motor_parts_fl;
        private Px4Hobber motor_parts_fr;
        private Px4Hobber motor_parts_bl;
        private Px4Hobber motor_parts_br;

        public string topic_type = "hako_mavlink_msgs/HakoHilActuatorControls";
        public string topic_name = "hobber_control";
        public int update_cycle = 1;
        public IoMethod io_method = IoMethod.SHM;
        public CommMethod comm_method = CommMethod.DIRECT;
        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            RoboPartsConfigData[] configs = new RoboPartsConfigData[1];
            configs[0] = new RoboPartsConfigData();
            configs[0].io_dir = IoDir.READ;
            configs[0].io_method = this.io_method;
            configs[0].value.org_name = this.topic_name;
            configs[0].value.type = this.topic_type;
            configs[0].value.class_name = ConstantValues.pdu_reader_class;
            configs[0].value.conv_class_name = ConstantValues.conv_pdu_reader_class;
            configs[0].value.pdu_size = 88;
            configs[0].value.write_cycle = this.update_cycle;
            configs[0].value.method_type = this.comm_method.ToString();
            return configs;
        }
        public float keisu = 0.01f;

        public void DoControl()
        {
            float[] controls = this.pdu_reader.GetReadOps().GetDataFloat32Array("controls");
            // 各モーターの指示値を取得
            float motorForceFR = keisu * controls[0];
            float motorForceBL = keisu * controls[1];
            float motorForceFL = keisu * controls[2];
            float motorForceBR = keisu * controls[3];

            // 各モーターに指示を送る
            motor_parts_fr.AddForce(motorForceFR);
            motor_parts_bl.AddForce(motorForceBL);
            motor_parts_fl.AddForce(motorForceFL);
            motor_parts_br.AddForce(motorForceBR);
        }

        public void Initialize(System.Object obj)
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
                var pdu_reader_name = root_name + "_" + this.topic_name + "Pdu";
                this.pdu_reader = this.pdu_io.GetReader(pdu_reader_name);
                if (this.pdu_reader == null)
                {
                    throw new ArgumentException("can not found pdu_reader:" + pdu_reader_name);
                }
                motor_parts_fr = motor_fr.GetComponent<Px4Hobber>();
                motor_parts_fl = motor_fl.GetComponent<Px4Hobber>();
                motor_parts_br = motor_br.GetComponent<Px4Hobber>();
                motor_parts_bl = motor_bl.GetComponent<Px4Hobber>();
            }
            motor_parts_fr.Initialize(motor_fr);
            motor_parts_fl.Initialize(motor_fl);
            motor_parts_br.Initialize(motor_br);
            motor_parts_bl.Initialize(motor_bl);
        }

        public RosTopicMessageConfig[] getRosConfig()
        {
            RosTopicMessageConfig[] cfg = new RosTopicMessageConfig[1];
            cfg[0] = new RosTopicMessageConfig();
            cfg[0].topic_message_name = this.topic_name;
            cfg[0].topic_type_name = this.topic_type;
            cfg[0].sub = true;
            return cfg;
        }

    }

}

