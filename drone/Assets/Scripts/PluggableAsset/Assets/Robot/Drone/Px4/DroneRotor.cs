using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts
{
    public class DroneRotor : MonoBehaviour, IRobotPartsMotor
    {
        private Transform propera;
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

        public void AddForce(float c_force)
        {
            this.force = c_force;
            current_force = this.force * this.transform.up;
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
                Debug.Log("Rotor init");
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
