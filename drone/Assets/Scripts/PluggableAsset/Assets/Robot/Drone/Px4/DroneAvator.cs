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
        private IPduWriter pdu_writer_collision;

        public GameObject motor_0;
        public GameObject motor_1;
        public GameObject motor_2;
        public GameObject motor_3;

        public double normal_x = 1.0;//m
        public double normal_z = 1.0;//m
        public double normal_y = 1.0;//m

        public double scale_x = 0.2;//m
        public double scale_y = 0.2;//m
        public double scale_z = 0.2;//m


        private Vector3 NormalizedCollisionPosition(Vector3 real_pos, Vector3 current_pos)
        {
            Vector3  pos = real_pos - current_pos;
            Debug.Log("Unity: relative Pos: " + pos);

            float normalizedX = (float)((pos.x / scale_x) * normal_x);
            float normalizedY = (float)((pos.y / scale_y) * normal_y);
            float normalizedZ = (float)((pos.z / scale_z) * normal_z);

            return new Vector3(normalizedX, normalizedY, normalizedZ);
        }


        private DroneRotor motor_parts_0;
        private DroneRotor motor_parts_1;
        private DroneRotor motor_parts_2;
        private DroneRotor motor_parts_3;

        public string[] topic_type = {
                "hako_mavlink_msgs/HakoHilActuatorControls",
                "geometry_msgs/Twist",
                "hako_msgs/Collision"
        };
        public string[] topic_name = {
            "drone_motor",
            "drone_pos",
            "drone_collision"
        };
        public int update_cycle = 1;
        public IoMethod io_method = IoMethod.SHM;
        public CommMethod comm_method = CommMethod.DIRECT;
        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            RoboPartsConfigData[] configs = new RoboPartsConfigData[3];
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


            configs[2] = new RoboPartsConfigData();
            configs[2].io_dir = IoDir.WRITE;
            configs[2].io_method = this.io_method;
            configs[2].value.org_name = this.topic_name[2];
            configs[2].value.type = this.topic_type[2];
            configs[2].value.class_name = ConstantValues.pdu_writer_class;
            configs[2].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[2].value.pdu_size = 280;
            configs[2].value.write_cycle = this.update_cycle;
            configs[2].value.method_type = this.comm_method.ToString();

            return configs;
        }
        private Collision lastCollision = null;
        private bool hasCollision = false;
        void OnCollisionEnter(Collision collision)
        {
            this.lastCollision = collision;
            this.hasCollision = true;
            Debug.Log("# Enter Collision");
        }
        private void WriteCollisionData()
        {
            this.pdu_writer_collision.GetWriteOps().SetData("collision", hasCollision);
            if (hasCollision)
            {
                uint contactNum = (uint)Mathf.Min(lastCollision.contactCount, 10);
                this.pdu_writer_collision.GetWriteOps().SetData("contact_num", contactNum);

                Vector3 relVelocity = ConvertUnity2Ros(lastCollision.relativeVelocity);
                this.pdu_writer_collision.GetWriteOps().Ref("relative_velocity").SetData("x", (double)relVelocity.x);
                this.pdu_writer_collision.GetWriteOps().Ref("relative_velocity").SetData("y", (double)relVelocity.y);
                this.pdu_writer_collision.GetWriteOps().Ref("relative_velocity").SetData("z", (double)relVelocity.z);

                Debug.Log("# Number of contact points: " + contactNum);
                Debug.Log("# Relative Velocity: " + relVelocity);
                for (int i = 0; i < contactNum; i++)
                {
                    Vector3 pos = ConvertUnity2Ros(NormalizedCollisionPosition(lastCollision.contacts[i].point, this.transform.position));
                    Debug.Log(string.Format("Unity: Contact point {0}: Position - {1}", i, lastCollision.contacts[i].point));
                    Debug.Log(string.Format("ROS  : Contact point {0}: Position - {1}", i, pos));
                    this.pdu_writer_collision.GetWriteOps().Refs("contact_position")[i].SetData("x", (double)pos.x);
                    this.pdu_writer_collision.GetWriteOps().Refs("contact_position")[i].SetData("y", (double)pos.y);
                    this.pdu_writer_collision.GetWriteOps().Refs("contact_position")[i].SetData("z", (double)pos.z);
                }

                double restitutionCoefficient = 0.2;
                this.pdu_writer_collision.GetWriteOps().SetData("restitution_coefficient", restitutionCoefficient);
            }
            else
            {
                //nothing to do
            }
            hasCollision = false;
            lastCollision = null;
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

            WriteCollisionData();
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
                var pdu_writer_name = root_name + "_" + this.topic_name[2] + "Pdu";
                this.pdu_writer_collision = this.pdu_io.GetWriter(pdu_writer_name);
                if (this.pdu_writer_collision == null)
                {
                    throw new ArgumentException("can not found pud_writer:" + pdu_writer_name);
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
            RosTopicMessageConfig[] cfg = new RosTopicMessageConfig[3];
            cfg[0] = new RosTopicMessageConfig();
            cfg[0].topic_message_name = this.topic_name[0];
            cfg[0].topic_type_name = this.topic_type[0];
            cfg[0].sub = true;

            cfg[1] = new RosTopicMessageConfig();
            cfg[1].topic_message_name = this.topic_name[1];
            cfg[1].topic_type_name = this.topic_type[1];
            cfg[1].sub = true;

            cfg[2] = new RosTopicMessageConfig();
            cfg[2].topic_message_name = this.topic_name[2];
            cfg[2].topic_type_name = this.topic_type[2];
            cfg[2].sub = false;
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
        private Vector3 ConvertUnity2Ros(Vector3 unity_data)
        {
            return new Vector3(
                unity_data.z, // ros.x
                -unity_data.x, // ros.y
                unity_data.y  // ros.z
                );
        }

    }

}

