using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_IOS
using Unity.XR.CoreUtils;
#endif

public class OriginViewer : MonoBehaviour
{
#if UNITY_IOS
    public XROrigin arSessionOrigin;
    public GameObject camera_obj;
    public Text originPositionView;
    public Text originRotationView;

    void Update()
    {
        Vector3 cameraPosition = camera_obj.transform.position;
        originPositionView.text = $"{cameraPosition.x:F2},{cameraPosition.y:F2},{cameraPosition.z:F2}";
        Debug.Log("Current camera position: " + cameraPosition);

        Vector3 cameraRotation = camera_obj.transform.rotation.eulerAngles;
        originRotationView.text = $"{cameraRotation.x:F1},{cameraRotation.y:F1},{cameraRotation.z:F1}";
        Debug.Log("Current camera rotation: " + cameraRotation);
    }
#endif
}
