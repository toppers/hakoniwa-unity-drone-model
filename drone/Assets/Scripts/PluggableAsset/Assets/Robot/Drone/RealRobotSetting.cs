using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealRobotSetting : MonoBehaviour
{
    public GameObject objectToIgnore;
    private int originalLayer;

    // Start is called before the first frame update
    void Start()
    {
        if (objectToIgnore != null)
        {
            originalLayer = objectToIgnore.layer;
            objectToIgnore.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.objectToIgnore != null)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                objectToIgnore.layer = originalLayer;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                objectToIgnore.layer = LayerMask.NameToLayer("Ignore Raycast");
            }

        }

    }
}
