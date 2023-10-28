using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts
{
    public class Px4Hobber : MonoBehaviour, IRobotPartsMotor
    {
        private Transform propera;
        public Rigidbody my_body;
        public bool cw = true;
        private float force = 0;
        private Vector3 current_force;
        public Vector3 GetCurrentForce()
        {
            return this.current_force;
        }

        public float rotation_keisu = 40.0f;
        private GameObject root;
        private float targetVelocity;


        public void SetForce(int c_force)
        {
            throw new System.NotImplementedException();
        }
        public void SetTargetVelicty(float targetVelocity)
        {
            throw new System.NotImplementedException();
        }
        public void DoFriction(Vector3 deceleration)
        {
            my_body.AddForce(deceleration, ForceMode.Acceleration);
        }

        public void AddForce(float c_force)
        {
            this.force = c_force;
            current_force = this.force * this.my_body.transform.up;
            this.my_body.AddForce(current_force, ForceMode.Force);
            var rotationSpeed = rotation_keisu * current_force.magnitude;
            if (cw)
            {
                this.propera.Rotate(0f, rotationSpeed, 0f);
            }
            else
            {
                this.propera.Rotate(0f, -rotationSpeed, 0f);
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

            if (this.root != null)
            {
                //Nothing to do.
            }
            else
            {
                Debug.Log("Hobber init");
                this.root = tmp;
                this.propera = this.transform.Find("Propera");
            }
        }

        public RosTopicMessageConfig[] getRosConfig()
        {
            return null;
        }


        private double tau = 20.0;
        public double fullThrust = 1.0;
        public double fullTorque = 1.0;
        private double w = 0.0;
        private long lastTime = -1;
        private long currentTime = 0;
        private double control = 0.0f;
        public void DoUpdate(double ctrl)
        {
            this.setControl(ctrl);
            currentTime += 1000; //usec
            if (lastTime >= 0)
            {
                double dt = (currentTime - lastTime) / 1000.0;
                w += (control - w) * (1.0 - Math.Exp(-dt / tau));
            }
            lastTime = currentTime;
        }
        private void setControl(double ctrl)
        {
            this.control = ctrl;
        }
        public double getThrust()
        {
            return w * fullThrust;
        }
        public double getTorque()
        {
            if (cw)
            {
                return control * fullTorque;
            }
            else
            {
                return -control * fullTorque;
            }
        }
        public Vector3 location = new Vector3();
        public Vector3 GetPosition()
        {
            return new Vector3(
                location.x,
                location.y,
                location.z
                );
        }
    }

}
