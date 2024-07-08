using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_IOS
using Unity.XR.CoreUtils;
#endif

public class OriginSetter : MonoBehaviour
{
#if UNITY_IOS
    public XROrigin arSessionOrigin;
    void Start()
    {
        string pos_text = PlayerPrefs.GetString("session_savedPos", "0,0,0");
        float pos_x = float.Parse(pos_text.Split(",")[0].Trim());
        float pos_y = float.Parse(pos_text.Split(",")[1].Trim());
        float pos_z = float.Parse(pos_text.Split(",")[2].Trim());

        Debug.Log("pos_x : " + pos_x + " pos_y:" + pos_y + " pos_z:" + pos_z);

        string rot_text = PlayerPrefs.GetString("session_savedRot", "0,0,0");
        float rot_x = float.Parse(rot_text.Split(",")[0].Trim());
        float rot_y = float.Parse(rot_text.Split(",")[1].Trim());
        float rot_z = float.Parse(rot_text.Split(",")[2].Trim());

        Debug.Log("rot_x : " + rot_x + " rot_y:" + rot_y + " rot_z:" + rot_z);

        arSessionOrigin.transform.position = new Vector3(pos_x, pos_y, pos_z);
        arSessionOrigin.transform.rotation = Quaternion.Euler(rot_x, rot_y, rot_z);
    }
#endif
}
