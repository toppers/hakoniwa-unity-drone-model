using System;
using System.Collections;
using System.Collections.Generic;
using Hakoniwa.AR.Core;
using Hakoniwa.PluggableAsset;
using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using UnityEngine;

namespace Hakoniwa.AR.Assets
{
    public class VirtualSignalController : MonoBehaviour, IHakoPlayerState, IRobotPartsSensor, IRobotPartsConfig
    {
        public Renderer signal_red;
        public Renderer signal_yellow;
        public Renderer signal_blue;

        public Material[] reds;
        public Material[] yellows;
        public Material[] blues;

        public Light redLight; // 赤信号用ライト
        public Light yellowLight; // 黄色信号用ライト
        public Light blueLight; // 青信号用ライト

        private int signal_state = 0;
        private float timer = 0.0f;

        // 信号の各状態の持続時間（秒）
        private float redDuration = 10.0f; // 赤信号の時間
        private float yellowDuration = 5.0f; // 黄色信号の時間
        private float blueDuration = 10.0f; // 青信号の時間
        private float blueBlinkDuration = 5.0f; // 青信号の点滅時間
        private float blinkInterval = 0.5f; // 点滅の間隔

        void Start()
        {
            SetSignalState(0);
        }

        void Update()
        {
            timer += Time.deltaTime;
            switch (signal_state)
            {
                case 0: // 赤信号
                    if (timer >= redDuration)
                    {
                        SetSignalState(1);
                    }
                    break;
                case 1: // 黄色信号
                    if (timer >= yellowDuration)
                    {
                        SetSignalState(2);
                    }
                    break;
                case 2: // 青信号
                    if (timer >= blueDuration)
                    {
                        SetSignalState(3);
                    }
                    break;
                case 3: // 青信号点滅
                    if (timer >= blueBlinkDuration)
                    {
                        SetSignalState(0);
                    }
                    else if (timer % blinkInterval < blinkInterval / 2)
                    {
                        SetBlinkingState(true);
                    }
                    else
                    {
                        SetBlinkingState(false);
                    }
                    break;
            }
        }

        private void SetSignalState(int state)
        {
            signal_state = state;
            timer = 0.0f;

            switch (signal_state)
            {
                case 0:
                    this.signal_red.material = reds[1];
                    this.signal_yellow.material = yellows[0];
                    this.signal_blue.material = blues[0];
                    redLight.enabled = true;
                    yellowLight.enabled = false;
                    blueLight.enabled = false;
                    break;
                case 1:
                    this.signal_red.material = reds[0];
                    this.signal_yellow.material = yellows[1];
                    this.signal_blue.material = blues[0];
                    redLight.enabled = false;
                    yellowLight.enabled = true;
                    blueLight.enabled = false;
                    break;
                case 2:
                    this.signal_red.material = reds[0];
                    this.signal_yellow.material = yellows[0];
                    this.signal_blue.material = blues[1];
                    redLight.enabled = false;
                    yellowLight.enabled = false;
                    blueLight.enabled = true;
                    break;
                case 3: // 青信号点滅
                    redLight.enabled = false;
                    yellowLight.enabled = false;
                    break;
            }
        }

        private void SetBlinkingState(bool isVisible)
        {
            if (isVisible)
            {
                this.signal_blue.material = blues[1];
                blueLight.enabled = true;
            }
            else
            {
                this.signal_blue.material = blues[0];
                blueLight.enabled = false;
            }
        }

        public int GetState()
        {
            int state = 0x0;
            if (this.signal_state >= 2)
            {
                if (blueLight.enabled)
                {
                    state |= (0x1 << 0);
                }
            }
            if (this.signal_state == 1)
            {
                state |= (0x1 << 1);
            }
            if (this.signal_state == 0)
            {
                state |= (0x1 << 2);
            }
            //Debug.Log("sate: " + state + " signal_sate:" + signal_state);
            return state;
        }

        public bool isAttachedSpecificController()
        {
            return false;
        }

        private uint GetPduSignalState()
        {
            return (uint)this.signal_state;
        }
        public void UpdateSensorValues()
        {
            uint pdu_data = GetPduSignalState();
            this.pdu_writer_state.GetWriteOps().SetData("data", pdu_data);


            Vector3 unity_pos = transform.position;
            //Debug.Log($"{this.root.name}: {rd.transform.position}");
            Vector3 ros_pos = ObjectSyncher.ConvertUnity2Ros(unity_pos);
            pdu_writer_pos.GetWriteOps().Ref("linear").SetData("x", (double)ros_pos.x);
            pdu_writer_pos.GetWriteOps().Ref("linear").SetData("y", (double)ros_pos.y);
            pdu_writer_pos.GetWriteOps().Ref("linear").SetData("z", (double)ros_pos.z);
            //Debug.Log("name: " + this.root.name + " pos: " + this.my_transform.position);

            // Unityの回転をROSの回転に変換
            Quaternion unity_rot = transform.rotation;
            Vector3 unity_euler = unity_rot.eulerAngles;

            // オイラー角をラジアンに変換し、座標系に合わせてROSへ変換
            Vector3 ros_euler = ObjectSyncher.ConvertUnity2RosRotation(unity_euler);
            pdu_writer_pos.GetWriteOps().Ref("angular").SetData("x", -(double)ros_euler.x);
            pdu_writer_pos.GetWriteOps().Ref("angular").SetData("y", -(double)ros_euler.y);
            pdu_writer_pos.GetWriteOps().Ref("angular").SetData("z", -(double)ros_euler.z);

        }

        private GameObject root;
        private string root_name;
        private PduIoConnector pdu_io;
        private IPduWriter pdu_writer_state;
        private IPduWriter pdu_writer_pos;


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
                var pdu_writer_name = root_name + "_" + this.topic_name[0] + "Pdu";
                this.pdu_writer_pos = this.pdu_io.GetWriter(pdu_writer_name);
                if (this.pdu_writer_pos == null)
                {
                    throw new ArgumentException("can not found pdu_writer:" + pdu_writer_name);
                }
                Debug.Log("Pdu Writer is loaded:" + pdu_writer_name);

                pdu_writer_name = root_name + "_" + this.topic_name[1] + "Pdu";
                this.pdu_writer_state = this.pdu_io.GetWriter(pdu_writer_name);
                if (this.pdu_writer_state == null)
                {
                    throw new ArgumentException("can not found pdu_writer:" + pdu_writer_name);
                }
                Debug.Log("Pdu Writer is loaded:" + pdu_writer_name);
            }
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

        public IoMethod io_method = IoMethod.SHM;
        public CommMethod comm_method = CommMethod.DIRECT;
        public int update_cycle = 1;
        public string[] topic_type = {
                "geometry_msgs/Twist",
                "std_msgs/UInt32"
        };
        public string[] topic_name = {
            "pos",
            "signal_data"
        };
        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            RoboPartsConfigData[] configs = new RoboPartsConfigData[2];
            configs[0] = new RoboPartsConfigData();
            configs[0].io_dir = IoDir.WRITE;
            configs[0].io_method = this.io_method;
            configs[0].value.org_name = this.topic_name[0];
            configs[0].value.type = this.topic_type[0];
            configs[0].value.class_name = ConstantValues.pdu_writer_class;
            configs[0].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[0].value.pdu_size = 88 + ConstantValues.PduMetaDataSize;
            configs[0].value.write_cycle = this.update_cycle;
            configs[0].value.method_type = this.comm_method.ToString();

            configs[1] = new RoboPartsConfigData();
            configs[1].io_dir = IoDir.WRITE;
            configs[1].io_method = this.io_method;
            configs[1].value.org_name = this.topic_name[1];
            configs[1].value.type = this.topic_type[1];
            configs[1].value.class_name = ConstantValues.pdu_writer_class;
            configs[1].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[1].value.pdu_size = ConstantValues.PduMetaDataSize + 2 * sizeof(uint);
            configs[1].value.write_cycle = this.update_cycle;
            configs[1].value.method_type = this.comm_method.ToString();

            return configs;
        }
    }
}
