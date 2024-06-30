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
            Debug.Log("Write before: " + pdu_data);
            this.pdu_writer.GetWriteOps().SetData("data", pdu_data);
            Debug.Log("Write after: " + pdu_data);
        }

        private GameObject root;
        private string root_name;
        private PduIoConnector pdu_io;
        private IPduWriter pdu_writer;
        public string topic_type = "std_msgs/UInt32";
        public string topic_name = "signal_data";


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
                var pdu_writer_name = root_name + "_" + this.topic_name + "Pdu";
                this.pdu_writer = this.pdu_io.GetWriter(pdu_writer_name);
                if (this.pdu_writer == null)
                {
                    throw new ArgumentException("can not found pdu_reader:" + pdu_writer_name);
                }
                Debug.Log("Pdu Writer is loaded:" + pdu_writer_name);
            }
        }
        public RosTopicMessageConfig[] getRosConfig()
        {
            RosTopicMessageConfig[] cfg = new RosTopicMessageConfig[1];
            cfg[0] = new RosTopicMessageConfig();
            cfg[0].topic_message_name = this.topic_name;
            cfg[0].topic_type_name = this.topic_type;
            cfg[0].sub = false;
            cfg[0].pub_option = new RostopicPublisherOption();
            cfg[0].pub_option.cycle_scale = this.update_cycle;
            cfg[0].pub_option.latch = false;
            cfg[0].pub_option.queue_size = 1;
            return cfg;
        }

        public IoMethod io_method = IoMethod.SHM;
        public CommMethod comm_method = CommMethod.DIRECT;
        public int update_cycle = 1;
        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            RoboPartsConfigData[] configs = new RoboPartsConfigData[1];
            configs[0] = new RoboPartsConfigData();
            configs[0].io_dir = IoDir.WRITE;
            configs[0].io_method = this.io_method;
            configs[0].value.org_name = this.topic_name;
            configs[0].value.type = this.topic_type;
            configs[0].value.class_name = ConstantValues.pdu_writer_class;
            configs[0].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[0].value.pdu_size = ConstantValues.PduMetaDataSize + 2 * sizeof(uint);
            configs[0].value.write_cycle = this.update_cycle;
            configs[0].value.method_type = this.comm_method.ToString();
            return configs;
        }
    }
}
