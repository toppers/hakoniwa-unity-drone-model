using System;
using System.Collections;
using System.Collections.Generic;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts
{
    public class ObjectSyncher : MonoBehaviour, IRobotPartsController, IRobotPartsConfig
    {
        public static Vector3 ConvertUnity2Ros(Vector3 unity_data)
        {
            return new Vector3(
                unity_data.z, // ros.x
                -unity_data.x, // ros.y
                unity_data.y  // ros.z
                );
        }

        public static Vector3 ConvertUnity2RosRotation(Vector3 unityEulerAngles)
        {
            // 座標系変換とラジアン変換：Unity (x, y, z) -> ROS (z, -x, y), 度 -> ラジアン
            return new Vector3(
                Mathf.Deg2Rad * unityEulerAngles.z,
                -Mathf.Deg2Rad * unityEulerAngles.x,
                Mathf.Deg2Rad * unityEulerAngles.y
            );
        }
        public static  Vector3 ConvertRos2Unity(Vector3 ros_data)
        {
            return new Vector3(
                -ros_data.y, // unity.x
                ros_data.z, // unity.y
                ros_data.x  // unity.z
                );
        }

        public string[] topic_type = {
                "geometry_msgs/Twist"
        };
        public string[] topic_name = {
            "pos"
        };
        public int update_cycle = 1;
        public IoMethod io_method = IoMethod.SHM;
        public CommMethod comm_method = CommMethod.DIRECT;
        private GameObject root;
        private string root_name;
        private PduIoConnector pdu_io;
        private IPduWriter pdu_writer;
        private IPduReader pdu_reader;
        public Rigidbody rd = null;
        private Transform my_transform;
        public bool isWriter = true;

        public void DoControl()
        {
            if (isWriter)
            {
                Vector3 unity_pos = my_transform.position;
                //Debug.Log($"{this.root.name}: {rd.transform.position}");
                Vector3 ros_pos = ConvertUnity2Ros(unity_pos);
                pdu_writer.GetWriteOps().Ref("linear").SetData("x", (double)ros_pos.x);
                pdu_writer.GetWriteOps().Ref("linear").SetData("y", (double)ros_pos.y);
                pdu_writer.GetWriteOps().Ref("linear").SetData("z", (double)ros_pos.z);
                //Debug.Log("name: " + this.root.name + " pos: " + this.my_transform.position);

                // Unityの回転をROSの回転に変換
                Quaternion unity_rot = my_transform.rotation;
                Vector3 unity_euler = unity_rot.eulerAngles;

                // オイラー角をラジアンに変換し、座標系に合わせてROSへ変換
                Vector3 ros_euler = ConvertUnity2RosRotation(unity_euler);
                pdu_writer.GetWriteOps().Ref("angular").SetData("x", -(double)ros_euler.x);
                pdu_writer.GetWriteOps().Ref("angular").SetData("y", -(double)ros_euler.y);
                pdu_writer.GetWriteOps().Ref("angular").SetData("z", -(double)ros_euler.z);
            }
            else
            {
                Vector3 ros_pos = new Vector3(
                    (float)this.pdu_reader.GetReadOps().Ref("linear").GetDataFloat64("x"),
                    (float)this.pdu_reader.GetReadOps().Ref("linear").GetDataFloat64("y"),
                    (float)this.pdu_reader.GetReadOps().Ref("linear").GetDataFloat64("z")
                 );
                Vector3 ros_angle = new Vector3(
                    -(180 / MathF.PI) * (float)this.pdu_reader.GetReadOps().Ref("angular").GetDataFloat64("x"),
                    -(180 / MathF.PI) * (float)this.pdu_reader.GetReadOps().Ref("angular").GetDataFloat64("y"),
                    -(180 / MathF.PI) * (float)this.pdu_reader.GetReadOps().Ref("angular").GetDataFloat64("z")
                 );
                //Debug.Log("angle: " + ros_angle);
                this.my_transform.position = ConvertRos2Unity(ros_pos);
                this.my_transform.rotation = Quaternion.Euler(ConvertRos2Unity(ros_angle));
            }
        }

        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            RoboPartsConfigData[] configs = new RoboPartsConfigData[1];
            int i = 0;
            configs[i] = new RoboPartsConfigData();
            if (isWriter)
            {
                configs[i].io_dir = IoDir.WRITE;
                configs[i].value.class_name = ConstantValues.pdu_writer_class;
                configs[i].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            }
            else
            {
                configs[i].io_dir = IoDir.READ;
                configs[i].value.class_name = ConstantValues.pdu_reader_class;
                configs[i].value.conv_class_name = ConstantValues.conv_pdu_reader_class;
            }
            configs[i].io_method = this.io_method;
            configs[i].value.org_name = this.topic_name[i];
            configs[i].value.type = this.topic_type[i];
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
                if (rd != null)
                {
                    this.my_transform = rd.transform;
                }
                else
                {
                    this.my_transform = this.transform;
                }

                this.pdu_io = PduIoConnector.Get(root_name);
                if (this.pdu_io == null)
                {
                    throw new ArgumentException("can not found pdu_io:" + root_name);
                }
                if (isWriter)
                {
                    var pdu_writer_name = root_name + "_" + this.topic_name[0] + "Pdu";
                    this.pdu_writer = this.pdu_io.GetWriter(pdu_writer_name);
                    if (this.pdu_writer == null)
                    {
                        throw new ArgumentException("can not found pud_writer:" + pdu_writer_name);
                    }
                }
                else
                {
                    var pdu_reader_name = root_name + "_" + this.topic_name[0] + "Pdu";
                    this.pdu_reader = this.pdu_io.GetReader(pdu_reader_name);
                    if (this.pdu_reader == null)
                    {
                        throw new ArgumentException("can not found pud_reader:" + pdu_reader_name);
                    }
                }
            }
        }

    }
}
