using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts.TestDriver
{
    public class HobberControlTestDriver : MonoBehaviour, IRobotPartsController, IRobotPartsConfig
    {
        private GameObject root;
        private string root_name;
        private PduIoConnector pdu_io;
        private IPduReader pdu_reader;

        public int update_cycle = 1;
        public string topic_name = "hobber_control";
        public string roboname = "Drone";
        private int count = 0;

        public RosTopicMessageConfig[] getRosConfig()
        {
            return new RosTopicMessageConfig[0];
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
                this.pdu_io = PduIoConnector.Get(roboname);
                if (this.pdu_io == null)
                {
                    throw new ArgumentException("can not found pdu_io:" + roboname);
                }
                var pdu_io_name = roboname + "_" + this.topic_name + "Pdu";
                this.pdu_reader = this.pdu_io.GetReader(pdu_io_name);
                if (this.pdu_reader == null)
                {
                    throw new ArgumentException("can not found pdu_reader:" + pdu_io_name);
                }
            }
            this.count = 0;
        }
        private Vector3 move_control;
        private float rotate_value;

        private void Update()
        {
            Vector3 value = new Vector3(0f, 0f, 0f);
            rotate_value = 0f;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                value.z = 1f;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                value.z = -1f;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                value.x = 1f;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                value.x = -1f;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                value.y = 1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                value.y = -1f;
            }

            if (Input.GetKey(KeyCode.R))
            {
                rotate_value = 1f;
            }
            if (Input.GetKey(KeyCode.E))
            {
                rotate_value = -1f;
            }
            move_control = value;
        }

        public void DoControl()
        {
            this.count++;
            if (this.count < this.update_cycle)
            {
                return;
            }
            this.count = 0;

            //Debug.Log("motor_control=" + move_control);
            this.pdu_reader.GetWriteOps().Ref("linear").SetData("x", (double)move_control.x);
            this.pdu_reader.GetWriteOps().Ref("linear").SetData("y", (double)move_control.y);
            this.pdu_reader.GetWriteOps().Ref("linear").SetData("z", (double)move_control.z);
            this.pdu_reader.GetWriteOps().Ref("angular").SetData("x", (double)rotate_value);
        }
        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            return new RoboPartsConfigData[0];
        }
    }
}

