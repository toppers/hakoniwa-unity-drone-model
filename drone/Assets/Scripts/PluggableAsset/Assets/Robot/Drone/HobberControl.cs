using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HobberControl : MonoBehaviour
{
    public Rigidbody my_body;
    public GameObject motor_fl;
    public GameObject motor_fr;
    public GameObject motor_bl;
    public GameObject motor_br;
    // Start is called before the first frame update
    void Start()
    {
        
    }
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

    // Update is called once per frame
    void Update()
    {
        bool hasControl = false;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            cf = GetLimit(cf + h_force_delta);
            hasControl = true;
        }
        else
        {
            cf = GetLimit(cf - h_backrate * h_force_delta);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            cb = GetLimit(cb + h_force_delta);
            hasControl = true;
        }
        else
        {
            cb = GetLimit(cb - h_backrate * h_force_delta);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            cl = GetLimit(cl + h_force_delta);
            hasControl = true;
        }
        else
        {
            cl = GetLimit(cl - h_backrate * h_force_delta);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            cr = GetLimit(cr + h_force_delta);
            hasControl = true;
        }
        else
        {
            cr = GetLimit(cr - h_backrate * h_force_delta);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            force_base += v_force_delta;
            hasControl = true;
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

        if (Input.GetKey(KeyCode.R))
        {
            t_r = 0.3f;
            hasControl = true;
        }
        else
        {
            t_r = -0.1f;
        }
        if (t_r <= 0.0f)
        {
            t_r = 0.0f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            t_l = 0.3f;
            hasControl = true;
        }
        else
        {
            t_l = -0.1f;
        }
        if (t_l <= 0.0f)
        {
            t_l = 0.0f;
        }


        if (Input.GetKey(KeyCode.A))
        {
            force_base -= c_down;
            hasControl = true;
        }
        if (force_base < 0.0f)
        {
            force_base = 0.0f;
        }

        //float angle_x = this.my_body.transform.up.x;
        //float angle_z = this.my_body.transform.up.z;
        if (hasControl == false)
        {
            //af = (1 - my_body.transform.up.y) * (my_body.transform.up.x);
            //ab = (1 - my_body.transform.up.y) * (-my_body.transform.up.z);
            float xRotation = this.my_body.transform.localEulerAngles.x;
            float yRotation = this.my_body.transform.localEulerAngles.y;
            float zRotation = this.my_body.transform.localEulerAngles.z;
            Debug.Log("xRotation =" + xRotation + " yRotation=" + yRotation + " zRotation=" + zRotation);
            var del_x = this.GetDel(this.my_body.transform.localEulerAngles.x);
            var del_z = this.GetDel(this.my_body.transform.localEulerAngles.z);
            Debug.Log("del_x =" + del_x + " del_z=" + del_z);

            del_x_integral = del_x + del_x_integral * p_integral;
            del_z_integral = del_z + del_z_integral * p_integral;
            this.my_body.AddTorque(this.my_body.transform.right * CalcForce(del_x, del_x_prev, del_x_integral));
            this.my_body.AddTorque(this.my_body.transform.forward * CalcForce(del_z, del_z_prev, del_z_integral));

            del_x_prev = del_x;
            del_z_prev = del_z;
            //this.my_body.AddTorque(adjust * this.my_body.transform.right * del_x);
            //this.my_body.AddTorque(adjust * this.my_body.transform.forward * del_z);
        }

        motor_fr.GetComponent<Hobber>().SetForce( force_base + ( -cf + cb + cl - cr), hasControl);
        motor_fl.GetComponent<Hobber>().SetForce( force_base + ( -cf + cb - cl + cr), hasControl);
        motor_br.GetComponent<Hobber>().SetForce( force_base + (  cf - cb + cl - cr), hasControl);
        motor_bl.GetComponent<Hobber>().SetForce( force_base + (  cf - cb - cl + cr), hasControl);

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
    public float adjust = 1.0f;
    public float max_speed = 10.0f;
    public float curr_speed = 0.0f;
    public float k = 0.1f;
    private void FixedUpdate()
    {
        Vector3 velocity = new Vector3(my_body.velocity.x, 0f, my_body.velocity.z);

        Vector3 deceleration = -velocity.normalized * k;
        my_body.AddForce(deceleration, ForceMode.Acceleration);
        motor_fr.GetComponent<Hobber>().my_body.AddForce(deceleration, ForceMode.Acceleration);
        motor_fl.GetComponent<Hobber>().my_body.AddForce(deceleration, ForceMode.Acceleration);
        motor_br.GetComponent<Hobber>().my_body.AddForce(deceleration, ForceMode.Acceleration);
        motor_bl.GetComponent<Hobber>().my_body.AddForce(deceleration, ForceMode.Acceleration);

        Vector3 fsum = motor_fr.GetComponent<Hobber>().current_force 
                    + motor_fl.GetComponent<Hobber>().current_force
                    + motor_br.GetComponent<Hobber>().current_force
                    + motor_bl.GetComponent<Hobber>().current_force;
        curr_speed = velocity.magnitude;
        if (velocity.magnitude > max_speed)
        {
            velocity = velocity.normalized * max_speed;
        }
        my_body.velocity = new Vector3(velocity.x, velocity.y, velocity.z);
    }
}
