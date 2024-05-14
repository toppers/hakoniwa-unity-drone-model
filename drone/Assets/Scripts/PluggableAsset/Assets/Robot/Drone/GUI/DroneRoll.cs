using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneRoll : MonoBehaviour
{
    public RectTransform rollpicture;
    public GameObject drone;

    void Update()
    {
        float droneRoll =  -GetDroneRoll();

        rollpicture.localRotation = Quaternion.Euler(0, 0, droneRoll);
    }

    float GetDroneRoll()
    {
        return drone.transform.eulerAngles.z;
    }
}
