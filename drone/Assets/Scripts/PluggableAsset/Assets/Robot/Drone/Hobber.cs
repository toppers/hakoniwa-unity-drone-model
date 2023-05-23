using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hobber : MonoBehaviour
{
    private Transform propera;
    public Rigidbody my_body;
    public float force = 0;
    // Start is called before the first frame update
    void Start()
    {
        this.propera = this.transform.Find("Propera");
    }
    public Vector3 current_force;
    public int motor_id;
    public float rotationSpeed = 1f;

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log("hasControl=" + hasControl);
        current_force = this.force * this.my_body.transform.up;
        this.my_body.AddForce(current_force, ForceMode.Force);

        if (this.propera != null)
        {
            rotationSpeed = 20 * rotation_keisu * current_force.magnitude;
            this.propera.Rotate(0f, rotationSpeed, 0f);
        }
    }
    bool hasControl = false;
    public void SetForce(float c_force, bool ctrl)
    {
        this.force = c_force;
        this.hasControl = ctrl;
    }

    public float rotation_keisu = 2.0f;
    private void Update()
    {
    }
}
