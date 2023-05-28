using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts
{
    public class HobberControl : MonoBehaviour, IRobotPartsController, IRobotPartsConfig
    {
        public Rigidbody my_body;
        public GameObject motor_fl;
        public GameObject motor_fr;
        public GameObject motor_bl;
        public GameObject motor_br;

        private Hobber motor_parts_fl;
        private Hobber motor_parts_fr;
        private Hobber motor_parts_bl;
        private Hobber motor_parts_br;

        public float force_base = 0.0f;
        public float force_base_max = 2.5f;
        public float force_base_min = 1.0f;
        public float h_force_delta = 0.001f;
        public float h_backrate = 100.0f;
        public float limit_horizon = 0.1f;
        public float v_force_delta = 0.01f;
        public float v_backrate = 1.0f;
        public float cf = 0.0f;
        public float cb = 0.0f;
        public float cl = 0.0f;
        public float cr = 0.0f;
        public float c_down = 0.01f;
        public float t_r = 0.0f;
        public float t_l = 0.0f;

        private float GetLimit(float value)
        {
            if (value > limit_horizon)
            {
                return limit_horizon;
            }
            else if (value < 0.0f)
            {
                return 0.0f;
            }
            else
            {
                return value;
            }
        }


        private void BalanceControl()
        {
            float xRotation = this.my_body.transform.localEulerAngles.x;
            float yRotation = this.my_body.transform.localEulerAngles.y;
            float zRotation = this.my_body.transform.localEulerAngles.z;
            //Debug.Log("xRotation =" + xRotation + " yRotation=" + yRotation + " zRotation=" + zRotation);
            var del_x = this.GetDel(this.my_body.transform.localEulerAngles.x);
            var del_z = this.GetDel(this.my_body.transform.localEulerAngles.z);
            //Debug.Log("del_x =" + del_x + " del_z=" + del_z);

            del_x_integral = del_x + del_x_integral * p_integral;
            del_z_integral = del_z + del_z_integral * p_integral;
            this.my_body.AddTorque(this.my_body.transform.right * CalcForce(del_x, del_x_prev, del_x_integral));
            this.my_body.AddTorque(this.my_body.transform.forward * CalcForce(del_z, del_z_prev, del_z_integral));

            del_x_prev = del_x;
            del_z_prev = del_z;
        }
        private void RotateControl()
        {
            if (t_r > 0)
            {
                //Debug.Log("t_r=" + t_r);
                this.my_body.AddTorque(-this.my_body.transform.up * t_r);
            }
            else if (t_l > 0)
            {
                //Debug.Log("t_r=" + t_l);
                this.my_body.AddTorque(this.my_body.transform.up * t_l);
            }
        }

        private float del_x_prev = 0f;
        private float del_z_prev = 0f;
        private float del_x_integral = 0f;
        private float del_z_integral = 0f;
        public float k_diff = 0.7f;
        public float k_integral = 0.1f;
        public float k_bibun = 1.0f;
        public float p_integral = 0.01f;
        float CalcForce(float diff, float prev_diff, float integral)
        {
            return k_diff * diff + k_integral * integral + k_bibun * (diff - prev_diff);
        }

        float GetDel(float value)
        {
            var del = 0f;
            if (value > 180f)
            {
                del = (360f - value) / 180f;
            }
            else if (value > 0)
            {
                del = -(value) / 180f;
            }
            else
            {
                del = -(value) / 180f;
            }
            return del;
        }
        private void ResetPower()
        {
            cf = 0f;
            cb = 0f;
            cl = 0f;
            cr = 0f;
            t_r = 0f;
            t_l = 0f;
        }
        private void DoSpeedControl()
        {
            Vector3 velocity = new Vector3(my_body.velocity.x, 0f, my_body.velocity.z);

            Vector3 deceleration = -velocity.normalized * k;
            my_body.AddForce(deceleration, ForceMode.Acceleration);
            motor_parts_fr.DoFriction(deceleration);
            motor_parts_fl.DoFriction(deceleration);
            motor_parts_br.DoFriction(deceleration);
            motor_parts_bl.DoFriction(deceleration);

            Vector3 fsum = motor_parts_fr.GetCurrentForce()
                        + motor_parts_fl.GetCurrentForce()
                        + motor_parts_br.GetCurrentForce()
                        + motor_parts_bl.GetCurrentForce();
            curr_speed = velocity.magnitude;
            if (velocity.magnitude > max_speed)
            {
                velocity = velocity.normalized * max_speed;
            }
            my_body.velocity = new Vector3(velocity.x, velocity.y, velocity.z);
        }

        public float max_speed = 10.0f;
        public float curr_speed = 0.0f;
        public float k = 0.1f;
        private void DoHobberControl()
        {
            motor_parts_fr.AddForce(force_base + (-cf + cb + cl - cr));
            motor_parts_fl.AddForce(force_base + (-cf + cb - cl + cr));
            motor_parts_br.AddForce(force_base + ( cf - cb + cl - cr));
            motor_parts_bl.AddForce(force_base + ( cf - cb - cl + cr));
        }

        public string topic_type = "geometry_msgs/Twist";
        public string topic_name = "hobber_control";
        public int update_cycle = 10;
        public IoMethod io_method = IoMethod.RPC;
        public CommMethod comm_method = CommMethod.UDP;
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
            configs[0].value.pdu_size = ConstantValues.Twist_pdu_size;
            configs[0].value.write_cycle = this.update_cycle;
            configs[0].value.method_type = this.comm_method.ToString();
            return configs;
        }
        private GameObject root;
        private string root_name;
        private PduIoConnector pdu_io;
        private IPduReader pdu_reader;
        private int count = 0;

        Vector3 motor_control = new Vector3();
        float rotate_value = 0f;
        public bool enableBalance = false;
        public void DoControl()
        {
            this.count++;
            if (this.count < this.update_cycle)
            {
                return;
            }
            this.count = 0;
            this.RecvMsg();

            DoHobberControl();
            if (enableBalance)
            {
                BalanceControl();
            }
            RotateControl();
            DoSpeedControl();
        }
        // Update is called once per frame
        private void RecvMsg()
        {
            //cl, cr
            motor_control.x = (float)this.pdu_reader.GetReadOps().Ref("linear").GetDataFloat64("x");
            //cu, cd
            motor_control.y = (float)this.pdu_reader.GetReadOps().Ref("linear").GetDataFloat64("y");
            //cf, cb
            motor_control.z = (float)this.pdu_reader.GetReadOps().Ref("linear").GetDataFloat64("z");
            //t_l, t_r
            rotate_value = (float)this.pdu_reader.GetReadOps().Ref("angular").GetDataFloat64("x");
            //Debug.Log("motor_control=" + motor_control);
            //Debug.Log("rotate_value=" + rotate_value);
            if (motor_control.z > 0f)
            {
                cf = GetLimit(cf + h_force_delta);
            }
            else
            {
                cf = GetLimit(cf - h_backrate * h_force_delta);
            }
            if (motor_control.z < 0f)
            {
                cb = GetLimit(cb + h_force_delta);
            }
            else
            {
                cb = GetLimit(cb - h_backrate * h_force_delta);
            }
            if (motor_control.x > 0f)
            {
                cl = GetLimit(cl + h_force_delta);
            }
            else
            {
                cl = GetLimit(cl - h_backrate * h_force_delta);
            }
            if (motor_control.x < 0f)
            {
                cr = GetLimit(cr + h_force_delta);
            }
            else
            {
                cr = GetLimit(cr - h_backrate * h_force_delta);
            }

            if (motor_control.y > 0f)
            {
                force_base += v_force_delta;
            }
            else
            {
                //force_base -= 0.0005f;
                force_base -= (v_backrate * v_force_delta);
            }
            if (force_base < force_base_min)
            {
                force_base = force_base_min;
            }
            else if (force_base >= force_base_max)
            {
                force_base = force_base_max;
            }
            if (motor_control.y < 0f)
            {
                force_base -= c_down;
            }
            if (force_base < 0.0f)
            {
                force_base = 0.0f;
            }

            if (rotate_value > 0f)
            {
                t_r = 0.1f;
            }
            else
            {
                t_r = -0.001f;
            }
            if (t_r <= 0.0f)
            {
                t_r = 0.0f;
            }
            if (rotate_value < 0f)
            {
                t_l = 0.1f;
            }
            else
            {
                t_l = -0.001f;
            }
            if (t_l <= 0.0f)
            {
                t_l = 0.0f;
            }



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
                motor_parts_fr = motor_fr.GetComponent<Hobber>();
                motor_parts_fl = motor_fl.GetComponent<Hobber>();
                motor_parts_br = motor_br.GetComponent<Hobber>();
                motor_parts_bl = motor_bl.GetComponent<Hobber>();
            }
            motor_parts_fr.Initialize(motor_fr);
            motor_parts_fl.Initialize(motor_fl);
            motor_parts_br.Initialize(motor_br);
            motor_parts_bl.Initialize(motor_bl);
            this.count = 0;
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

