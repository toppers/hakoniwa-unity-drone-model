using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Hakoniwa.PluggableAsset.Assets.Environment
{
    public class EnvTemperatureRegion : MonoBehaviour, IEnvironmentObject
    {
        private List<IRobotProperty> robot_properties = new List<IRobotProperty>();
        private TemperatureColorExpression colorExpression;
        public double regionTemperature = 25.0;
        public double Tr = 1.0;

        public void Initialize()
        {
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity);
            int i = 0;
            while (i < hitColliders.Length)
            {
                OnTriggerEnter(hitColliders[i]);
                i++;
            }
            this.colorExpression = GetComponentInChildren<TemperatureColorExpression>();
            if (this.colorExpression == null)
            {
                throw new ArgumentException("can not found color expression object on " + this.name);
            }
            this.colorExpression.SetTemperature(this.regionTemperature);
        }

        private void OnTriggerEnter(Collider collider)
        {
            var robotProperty = collider.GetComponent<IRobotProperty>();
            if (robotProperty != null && !robot_properties.Contains(robotProperty))
            {
                Debug.LogWarning(this.transform.name + " tirgger enter: " + collider.transform.name);
                robot_properties.Add(robotProperty);
            }
        }


        private void OnTriggerExit(Collider other)
        {
            var robotProperty = other.GetComponent<IRobotProperty>();
            if (robotProperty != null)
            {
                Debug.LogWarning(this.transform.name + " tirgger exit: " + GetComponent<Collider>().transform.name);
                robot_properties.Remove(robotProperty);
            }
        }

        public void UpdateRobotProperty()
        {
            foreach (var robotProperty in robot_properties)
            {
                var currTemperature = robotProperty.GetTemperature();
                // (T_new - T_curr) / dt = (T_region - T_curr) / Tr
                var newTemperature = currTemperature + (Time.fixedDeltaTime / Tr) * (regionTemperature - currTemperature);
                //Debug.Log("newTemperature=" + newTemperature);

                robotProperty.SetTemperature(newTemperature);
            }
        }

        public void ClearAll()
        {
            robot_properties.Clear();
        }
    }
}
