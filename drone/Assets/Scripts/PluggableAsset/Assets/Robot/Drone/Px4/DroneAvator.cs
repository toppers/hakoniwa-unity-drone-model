using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts
{
    public class DroneAvator : MonoBehaviour, IRobotPartsController, IRobotPartsConfig
    {
        private GameObject root;
        private string root_name;
        private PduIoConnector pdu_io;
        private IPduReader pdu_reader_actuator;
        private IPduReader pdu_reader_pos;

        public GameObject motor_0;
        public GameObject motor_1;
        public GameObject motor_2;
        public GameObject motor_3;

        private DroneRotor motor_parts_0;
        private DroneRotor motor_parts_1;
        private DroneRotor motor_parts_2;
        private DroneRotor motor_parts_3;

        public string[] topic_type = {
                "hako_mavlink_msgs/HakoHilActuatorControls",
                "geometry_msgs/Twist"
        };
        public string[] topic_name = {
            "drone_motor",
            "drone_pos"
        };
        public int update_cycle = 1;
        public IoMethod io_method = IoMethod.SHM;
        public CommMethod comm_method = CommMethod.DIRECT;
        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            RoboPartsConfigData[] configs = new RoboPartsConfigData[2];
            configs[0] = new RoboPartsConfigData();
            configs[0].io_dir = IoDir.READ;
            configs[0].io_method = this.io_method;
            configs[0].value.org_name = this.topic_name[0];
            configs[0].value.type = this.topic_type[0];
            configs[0].value.class_name = ConstantValues.pdu_reader_class;
            configs[0].value.conv_class_name = ConstantValues.conv_pdu_reader_class;
            configs[0].value.pdu_size = 88;
            configs[0].value.write_cycle = this.update_cycle;
            configs[0].value.method_type = this.comm_method.ToString();

            configs[1] = new RoboPartsConfigData();
            configs[1].io_dir = IoDir.READ;
            configs[1].io_method = this.io_method;
            configs[1].value.org_name = this.topic_name[1];
            configs[1].value.type = this.topic_type[1];
            configs[1].value.class_name = ConstantValues.pdu_reader_class;
            configs[1].value.conv_class_name = ConstantValues.conv_pdu_reader_class;
            configs[1].value.pdu_size = ConstantValues.Twist_pdu_size;
            configs[1].value.write_cycle = this.update_cycle;
            configs[1].value.method_type = this.comm_method.ToString();
            return configs;
        }

        public void DoControl()
        {
            float[] controls = this.pdu_reader_actuator.GetReadOps().GetDataFloat32Array("controls");

            motor_parts_0.AddForce(controls[0]);
            motor_parts_1.AddForce(controls[1]);
            motor_parts_2.AddForce(controls[2]);
            motor_parts_3.AddForce(controls[3]);

            Vector3 ros_pos = new Vector3(
                (float)this.pdu_reader_pos.GetReadOps().Ref("linear").GetDataFloat64("x"),
                (float)this.pdu_reader_pos.GetReadOps().Ref("linear").GetDataFloat64("y"),
                (float)this.pdu_reader_pos.GetReadOps().Ref("linear").GetDataFloat64("z")
             );
            Vector3 ros_angle = new Vector3(
                -(180 / MathF.PI) * (float)this.pdu_reader_pos.GetReadOps().Ref("angular").GetDataFloat64("x"),
                -(180 / MathF.PI) * (float)this.pdu_reader_pos.GetReadOps().Ref("angular").GetDataFloat64("y"),
                -(180 / MathF.PI) * (float)this.pdu_reader_pos.GetReadOps().Ref("angular").GetDataFloat64("z")
             );
            //Debug.Log("angle: " + ros_angle);
            this.transform.position = this.ConvertRos2Unity(ros_pos);
            //this.transform.eulerAngles = this.ConvertRos2Unity(ros_angle);
            this.transform.rotation = Quaternion.Euler(this.ConvertRos2Unity(ros_angle));
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
                var pdu_reader_name = root_name + "_" + this.topic_name[0] + "Pdu";
                this.pdu_reader_actuator = this.pdu_io.GetReader(pdu_reader_name);
                if (this.pdu_reader_actuator == null)
                {
                    throw new ArgumentException("can not found pdu_reader:" + pdu_reader_name);
                }
                pdu_reader_name = root_name + "_" + this.topic_name[1] + "Pdu";
                this.pdu_reader_pos = this.pdu_io.GetReader(pdu_reader_name);
                if (this.pdu_reader_pos == null)
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

        public RosTopicMessageConfig[] getRosConfig()
        {
            RosTopicMessageConfig[] cfg = new RosTopicMessageConfig[2];
            cfg[0] = new RosTopicMessageConfig();
            cfg[0].topic_message_name = this.topic_name[0];
            cfg[0].topic_type_name = this.topic_type[0];
            cfg[0].sub = true;

            cfg[1] = new RosTopicMessageConfig();
            cfg[1].topic_message_name = this.topic_name[1];
            cfg[1].topic_type_name = this.topic_type[1];
            cfg[1].sub = true;
            return cfg;
        }

        private Vector3 ConvertRos2Unity(Vector3 ros_data)
        {
            return new Vector3(
                -ros_data.y, // unity.x
                ros_data.z, // unity.y
                ros_data.x  // unity.z
                );
        }
    }

}

