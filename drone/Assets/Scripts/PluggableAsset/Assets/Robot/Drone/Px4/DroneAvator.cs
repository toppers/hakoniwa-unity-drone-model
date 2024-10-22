using Hakoniwa.PluggableAsset.Assets.Environment;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static Hakoniwa.PluggableAsset.Assets.Robot.Parts.MagnetHolder;
using Hakoniwa.AR.Core;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts
{
    public class DroneAvator : MonoBehaviour, IRobotPartsController, IRobotPartsConfig, IRobotProperty, IHakoPlayerState
    {
        private GameObject root;
        private TemperatureColorExpression colorExpression;
        private string root_name;
        private PduIoConnector pdu_io;
        private IPduReader pdu_reader_actuator;
        private IPduReader pdu_reader_pos;
        private IPduWriter pdu_writer_collision;
        private IPduWriter pdu_writer_disturb;

        public GameObject motor_0;
        public GameObject motor_1;
        public GameObject motor_2;
        public GameObject motor_3;

        private DroneRotor motor_parts_0;
        private DroneRotor motor_parts_1;
        private DroneRotor motor_parts_2;
        private DroneRotor motor_parts_3;

        private List<RigidbodyInfo> targets;

        public string[] topic_type = {
                "hako_mavlink_msgs/HakoHilActuatorControls",
                "geometry_msgs/Twist",
                "hako_msgs/Collision",
                "hako_msgs/ManualPosAttControl",
                "hako_msgs/Disturbance",
                "hako_msgs/HakoDroneCmdTakeoff",
                "hako_msgs/HakoDroneCmdMove",
                "hako_msgs/HakoDroneCmdLand",
                "hako_msgs/GameControllerOperation"
        };
        public string[] topic_name = {
            "drone_motor",
            "drone_pos",
            "drone_collision",
            "drone_manual_pos_att_control",
            "drone_disturbance",
            "drone_cmd_takeoff",
            "drone_cmd_move",
            "drone_cmd_land",
            "hako_cmd_game"
        };
        public int update_cycle = 1;
        public IoMethod io_method = IoMethod.SHM;
        public CommMethod comm_method = CommMethod.DIRECT;
        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            RoboPartsConfigData[] configs = new RoboPartsConfigData[9];
            int i = 0;
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
            i++;
            configs[i] = new RoboPartsConfigData();
            configs[i].io_dir = IoDir.READ;
            configs[i].io_method = this.io_method;
            configs[i].value.org_name = this.topic_name[i];
            configs[i].value.type = this.topic_type[i];
            configs[i].value.class_name = ConstantValues.pdu_reader_class;
            configs[i].value.conv_class_name = ConstantValues.conv_pdu_reader_class;
            configs[i].value.pdu_size = ConstantValues.Twist_pdu_size;
            configs[i].value.write_cycle = this.update_cycle;
            configs[i].value.method_type = this.comm_method.ToString();
            i++;

            configs[i] = new RoboPartsConfigData();
            configs[i].io_dir = IoDir.WRITE;
            configs[i].io_method = this.io_method;
            configs[i].value.org_name = this.topic_name[i];
            configs[i].value.type = this.topic_type[i];
            configs[i].value.class_name = ConstantValues.pdu_writer_class;
            configs[i].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[i].value.pdu_size = 280 + ConstantValues.PduMetaDataSize;
            configs[i].value.write_cycle = this.update_cycle;
            configs[i].value.method_type = this.comm_method.ToString();
            i++;
            configs[i] = new RoboPartsConfigData();
            configs[i].io_dir = IoDir.READ;
            configs[i].io_method = this.io_method;
            configs[i].value.org_name = this.topic_name[i];
            configs[i].value.type = this.topic_type[i];
            configs[i].value.class_name = ConstantValues.pdu_writer_class;
            configs[i].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[i].value.pdu_size = 56 + ConstantValues.PduMetaDataSize;
            configs[i].value.write_cycle = this.update_cycle;
            configs[i].value.method_type = this.comm_method.ToString();
            i++;
            configs[i] = new RoboPartsConfigData();
            configs[i].io_dir = IoDir.WRITE;
            configs[i].io_method = this.io_method;
            configs[i].value.org_name = this.topic_name[i];
            configs[i].value.type = this.topic_type[i];
            configs[i].value.class_name = ConstantValues.pdu_writer_class;
            configs[i].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[i].value.pdu_size = 64 + ConstantValues.PduMetaDataSize;
            configs[i].value.write_cycle = this.update_cycle;
            configs[i].value.method_type = this.comm_method.ToString();

            i++;
            configs[i] = new RoboPartsConfigData();
            configs[i].io_dir = IoDir.READ;
            configs[i].io_method = this.io_method;
            configs[i].value.org_name = this.topic_name[i];
            configs[i].value.type = this.topic_type[i];
            configs[i].value.class_name = ConstantValues.pdu_writer_class;
            configs[i].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[i].value.pdu_size = 40 + ConstantValues.PduMetaDataSize;
            configs[i].value.write_cycle = this.update_cycle;
            configs[i].value.method_type = this.comm_method.ToString();
            i++;
            configs[i] = new RoboPartsConfigData();
            configs[i].io_dir = IoDir.READ;
            configs[i].io_method = this.io_method;
            configs[i].value.org_name = this.topic_name[i];
            configs[i].value.type = this.topic_type[i];
            configs[i].value.class_name = ConstantValues.pdu_writer_class;
            configs[i].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[i].value.pdu_size = 56 + ConstantValues.PduMetaDataSize;
            configs[i].value.write_cycle = this.update_cycle;
            configs[i].value.method_type = this.comm_method.ToString();
            i++;
            configs[i] = new RoboPartsConfigData();
            configs[i].io_dir = IoDir.READ;
            configs[i].io_method = this.io_method;
            configs[i].value.org_name = this.topic_name[i];
            configs[i].value.type = this.topic_type[i];
            configs[i].value.class_name = ConstantValues.pdu_writer_class;
            configs[i].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[i].value.pdu_size = 40 + ConstantValues.PduMetaDataSize;
            configs[i].value.write_cycle = this.update_cycle;
            configs[i].value.method_type = this.comm_method.ToString();
            i++;
            configs[i] = new RoboPartsConfigData();
            configs[i].io_dir = IoDir.READ;
            configs[i].io_method = this.io_method;
            configs[i].value.org_name = this.topic_name[i];
            configs[i].value.type = this.topic_type[i];
            configs[i].value.class_name = ConstantValues.pdu_writer_class;
            configs[i].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[i].value.pdu_size = 112 + ConstantValues.PduMetaDataSize;
            configs[i].value.write_cycle = this.update_cycle;
            configs[i].value.method_type = this.comm_method.ToString();

            return configs;
        }
        private Collision lastCollision = null;
        private bool hasCollision = false;
        public double restitutionCoefficient = 1.0;
        void OnCollisionEnter(Collision collision)
        {
            var rb = collision.gameObject.GetComponentInChildren<Rigidbody>();
            foreach (var info in targets)
            {
                if (info.Rigidbody == rb)
                {
                    return;
                }
            }
            // 衝突オブジェクトが地面かどうかをタグで判定
            if (collision.gameObject.CompareTag("HakoAsset"))
            {
                this.restitutionCoefficient = 0.0;
            }
            else
            {
                this.restitutionCoefficient = 1.0;
            }
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
                    Vector3 pos = ConvertUnity2Ros(lastCollision.contacts[i].point);
                    Debug.Log(string.Format("Unity: Contact point {0}: Position - {1}", i, lastCollision.contacts[i].point));
                    Debug.Log(string.Format("ROS  : Contact point {0}: Position - {1}", i, pos));
                    this.pdu_writer_collision.GetWriteOps().Refs("contact_position")[i].SetData("x", (double)pos.x);
                    this.pdu_writer_collision.GetWriteOps().Refs("contact_position")[i].SetData("y", (double)pos.y);
                    this.pdu_writer_collision.GetWriteOps().Refs("contact_position")[i].SetData("z", (double)pos.z);
                }

                this.pdu_writer_collision.GetWriteOps().SetData("restitution_coefficient", restitutionCoefficient);
            }
            else
            {
                //nothing to do
            }
            hasCollision = false;
            lastCollision = null;
        }
        private void WriteDisturbanceData()
        {
            this.pdu_writer_disturb.GetWriteOps().Ref("d_temp").SetData("value", (double)this.current_temperature);
        }
        public GameObject target_camera;
        public string audio_filepath;
        private double my_controls = 0;
        public float maxDistance = 100.0f;
        public float minDistance = 0.0f;
        private float updateTimer = 0.0f; // Timer to track the update interval
        private float updateInterval = 1.0f; // Interval to update (1 second)
        private bool enabledAudio = false;
        private void Update()
        {
            if (enabledAudio == false)
            {
                return;
            }
            updateTimer += Time.deltaTime;
            if (updateTimer < updateInterval)
            {
                return;
            }
            updateTimer = 0;

            // Calculate distance to the target camera
            float distance = Vector3.Distance(target_camera.transform.position, transform.position);

            // Map the distance to volume level
            float volume = 1.0f - Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));

            if (audioSource.isPlaying == false && my_controls > 0)
            {
                audioSource.Play();
            }
            else if (audioSource.isPlaying == true && my_controls == 0)
            {
                audioSource.Stop();
            }

            if (audioSource.isPlaying)
            {
                audioSource.volume = volume;
            }
        }
        IEnumerator LoadAudio(string filePath)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = clip;
                    audioSource.Stop();
                    enabledAudio = true;
                }
            }
        }
        private string audioPath;
        [System.Serializable]
        public class DroneConfig
        {
            public Dictionary<string, DroneDetails> drones;
        }

        [System.Serializable]
        public class DroneDetails
        {
            public string audio_rotor_path;
            public Dictionary<string, DroneLidarDetails> LiDARs;
        }
        [System.Serializable]
        public class DroneLidarDetails
        {
            public bool Enabled;
            public int NumberOfChannels;
            public int RotationsPerSecond;
            public int PointsPerSecond;
            public float VerticalFOVUpper;
            public float VerticalFOVLower;
            public float HorizontalFOVStart;
            public float HorizontalFOVEnd;
            public bool DrawDebugPoints;
            public float MaxDistance;
            public float X;
            public float Y;
            public float Z;
            public float Roll;
            public float Pitch;
            public float Yaw;
        }
        private DroneConfig loadedData = null;
        private void LoadDroneConfig()
        {
            string droneName = this.root_name;
            string filePath = "./drone_config.json";
            Debug.Log("Looking for config file at: " + filePath);

            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                loadedData = JsonConvert.DeserializeObject<DroneConfig>(dataAsJson);

                if (loadedData != null && loadedData.drones != null)
                {
                    if (loadedData.drones.ContainsKey(droneName))
                    {
                        audioPath = loadedData.drones[droneName].audio_rotor_path;
                        Debug.Log("Audio Path for " + droneName + ": " + audioPath);
                    }
                    else
                    {
                        Debug.LogError("Drone configuration for " + droneName + " not found.");
                    }
                }
                else
                {
                    Debug.LogError("Drone configurations are missing or corrupt. Check JSON structure.");
                }
            }
            else
            {
                Debug.LogError("Cannot find drone_config.json file at: " + filePath);
            }
        }
        private bool GetParam(string name, out LiDAR3DParams param)
        {
            string droneName = this.root_name;
            param = new LiDAR3DParams();
            if (loadedData == null)
            {
                return false;
            }
            if (loadedData.drones.ContainsKey(droneName))
            {
                if (loadedData.drones[droneName].LiDARs.ContainsKey(name))
                {
                    Debug.Log("found param: " + name);
                    param.Enabled = loadedData.drones[droneName].LiDARs[name].Enabled;
                    param.NumberOfChannels = loadedData.drones[droneName].LiDARs[name].NumberOfChannels;
                    param.RotationsPerSecond = loadedData.drones[droneName].LiDARs[name].RotationsPerSecond;
                    param.PointsPerSecond = loadedData.drones[droneName].LiDARs[name].PointsPerSecond;
                    param.MaxDistance = loadedData.drones[droneName].LiDARs[name].MaxDistance;
                    param.VerticalFOVUpper = loadedData.drones[droneName].LiDARs[name].VerticalFOVUpper;
                    param.VerticalFOVLower = loadedData.drones[droneName].LiDARs[name].VerticalFOVLower;
                    param.HorizontalFOVStart = loadedData.drones[droneName].LiDARs[name].HorizontalFOVStart;
                    param.HorizontalFOVEnd = loadedData.drones[droneName].LiDARs[name].HorizontalFOVEnd;
                    param.DrawDebugPoints = loadedData.drones[droneName].LiDARs[name].DrawDebugPoints;
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        public void DoControl()
        {
            float[] controls = this.pdu_reader_actuator.GetReadOps().GetDataFloat32Array("controls");
            my_controls = controls[0];


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
            //WriteDisturbanceData();

            this.colorExpression.SetTemperature(this.current_temperature);

        }

        private AudioSource audioSource;
        private LiDAR3D[] lidars;
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
                this.targets = GetTargets();
                this.root = tmp;
                this.root_name = string.Copy(this.root.transform.name);
                LoadDroneConfig();
                this.lidars = this.GetComponentsInChildren<LiDAR3D>();
                foreach (var lidar in lidars)
                {
                    Debug.Log("Found Lidar: " + lidar.transform.parent.gameObject.name);
                    LiDAR3DParams param;
                    if (this.GetParam(lidar.transform.parent.gameObject.name, out param))
                    {
                        lidar.SetParams(param);
                        //pos
                        float x = loadedData.drones[this.root_name].LiDARs[lidar.transform.parent.gameObject.name].X;
                        float y = loadedData.drones[this.root_name].LiDARs[lidar.transform.parent.gameObject.name].Y;
                        float z = loadedData.drones[this.root_name].LiDARs[lidar.transform.parent.gameObject.name].Z;
                        float y_off = lidar.transform.parent.parent.position.y;
                        Vector3 v = new Vector3(x, y, z);
                        Vector3 v_unity = ConvertRos2Unity(v);
                        v_unity.y += y_off;
                        Debug.Log("v: " + v_unity);
                        lidar.transform.parent.position = v_unity;
                        //angle
                        float roll = loadedData.drones[this.root_name].LiDARs[lidar.transform.parent.gameObject.name].Roll;
                        float pitch = loadedData.drones[this.root_name].LiDARs[lidar.transform.parent.gameObject.name].Pitch;
                        float yaw = loadedData.drones[this.root_name].LiDARs[lidar.transform.parent.gameObject.name].Yaw;
                        Vector3 euler_angle = new Vector3(roll, pitch, yaw);
                        Vector3 euler_angle_unity = - ConvertRos2Unity(euler_angle);
                        Debug.Log("euler_angle: " + euler_angle_unity);
                        lidar.transform.parent.eulerAngles = euler_angle_unity;
                    }
                }
                audioSource = GetComponent<AudioSource>();
                if (target_camera != null && audioSource != null && !string.IsNullOrEmpty(audioPath))
                {
                    StartCoroutine(LoadAudio(audioPath));
                }
                this.pdu_io = PduIoConnector.Get(root_name);
                if (this.pdu_io == null)
                {
                    throw new ArgumentException("can not found pdu_io:" + root_name);
                }
                this.colorExpression = GetComponentInChildren<TemperatureColorExpression>();
                if (this.colorExpression == null)
                {
                    throw new ArgumentException("can not found color expression object on " + root_name);
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

                pdu_writer_name = root_name + "_" + this.topic_name[4] + "Pdu";
                this.pdu_writer_disturb = this.pdu_io.GetWriter(pdu_writer_name);
                if (this.pdu_writer_disturb == null)
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

        public double current_temperature;
        public int temperation_region_count = 0;
        public double GetTemperature()
        {
            return current_temperature;
        }

        public void SetTemperature(double temperature)
        {
            this.current_temperature = temperature;
        }

        public void IncrementTemperatureRegion()
        {
            this.temperation_region_count = 1;
            Debug.LogWarning("Exnter:count: " + this.temperation_region_count);
        }

        public void DecrementTemperatureRegion()
        {
            this.temperation_region_count = 0;
            Debug.LogWarning("Exit:count: " + this.temperation_region_count);
        }
        bool IRobotProperty.IsInTemeratureRegion()
        {
            return this.temperation_region_count > 0;
        }

        public int GetState()
        {
            return (int)(this.my_controls * 100.0);
        }
    }
}

