using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

namespace Hakoniwa.PluggableAsset.Assets.Environment
{
    public class EnvTemperatureRegion : MonoBehaviour, IEnvironmentObject
    {
        private List<IRobotProperty> robot_properties = new List<IRobotProperty>();
        private TemperatureColorExpression colorExpression;
        public double regionTemperature = 25.0;
        public double Tr = 1.0;
        public bool isBase = false;

        public void Initialize()
        {
            if (!isBase)
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
            else
            {
                foreach (var rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    var components = rootGameObject.GetComponentsInChildren<Component>(true);
                    foreach (var component in components)
                    {
                        if (component is IRobotProperty myInterfaceObj)
                        {
                            robot_properties.Add(myInterfaceObj);
                        }
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!isBase)
            {
                var robotProperty = collider.GetComponent<IRobotProperty>();
                if (robotProperty != null && !robot_properties.Contains(robotProperty))
                {
                    Debug.LogWarning(this.transform.name + " tirgger enter: " + collider.transform.name);
                    robotProperty.IncrementTemperatureRegion();
                    robot_properties.Add(robotProperty);
                }
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (!isBase)
            {
                var robotProperty = other.GetComponent<IRobotProperty>();
                if (robotProperty != null)
                {
                    Debug.LogWarning(this.transform.name + " tirgger exit: " + GetComponent<Collider>().transform.name);
                    robotProperty.DecrementTemperatureRegion();
                    robot_properties.Remove(robotProperty);
                }
            }
        }

        public void UpdateRobotProperty()
        {
            foreach (var robotProperty in robot_properties)
            {
                if (isBase && robotProperty.IsInTemeratureRegion())
                {
                    continue;
                }
                var currTemperature = robotProperty.GetTemperature();
                // (T_new - T_curr) / dt = (T_region - T_curr) / Tr
                var newTemperature = currTemperature + (Time.fixedDeltaTime / Tr) * (regionTemperature - currTemperature);
                //Debug.Log("newTemperature=" + newTemperature);

                robotProperty.SetTemperature(newTemperature);
            }
        }

        public void ClearAll()
        {
            if (!isBase)
            {
                robot_properties.Clear();
            }
        }
    }
}
