using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts
{
    public class Hobber : MonoBehaviour, IRobotPartsMotor
    {
        private Transform propera;
        public Rigidbody my_body;
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
            this.propera.Rotate(0f, rotationSpeed, 0f);
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
    }

}
