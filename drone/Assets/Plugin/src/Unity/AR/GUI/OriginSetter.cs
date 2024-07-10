using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_IOS
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
#endif

public class OriginSetter : MonoBehaviour
{
#if UNITY_IOS
    public XROrigin arSessionOrigin;
    private ARSession arSession;

    void Awake()
    {
        arSession = FindObjectOfType<ARSession>();

        // ARセッションをリセット
        if (arSession != null)
        {
            StartCoroutine(ResetARSession());
        }
    }
    private IEnumerator ResetARSession()
    {
        arSession.Reset();
        yield return null;
        // 位置データの取得
        string pos_text = PlayerPrefs.GetString(OriginUI.session_savedPosKey, "0,0,0");
        Vector3 position = ParseVector3(pos_text);
        Debug.Log("Position: " + position);

        // 回転データの取得
        string rot_text = PlayerPrefs.GetString(OriginUI.session_savedRotKey, "0,0,0");
        Vector3 rotation = ParseVector3(rot_text);
        Debug.Log("Rotation: " + rotation);

        Debug.Log("BEFORE: arSessionOrigin position: " + arSessionOrigin.transform.position);

        // 位置と回転の設定
        arSessionOrigin.transform.position = position;
        //Vector3 newRotation = arSessionOrigin.transform.eulerAngles;
        //newRotation.y = rotation.y;
        //arSessionOrigin.transform.rotation = Quaternion.Euler(newRotation);
        Debug.Log("After: arSessionOrigin position: " + arSessionOrigin.transform.position);
    }
    // ベクトル3を解析するヘルパーメソッド
    private Vector3 ParseVector3(string vectorString)
    {
        string[] values = vectorString.Split(',');
        if (values.Length != 3)
        {
            Debug.LogError("Invalid vector format: " + vectorString);
            return Vector3.zero;
        }

        try
        {
            float x = float.Parse(values[0].Trim());
            float y = float.Parse(values[1].Trim());
            float z = float.Parse(values[2].Trim());
            return new Vector3(x, y, z);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error parsing vector: " + vectorString + " Exception: " + e);
            return Vector3.zero;
        }
    }
#endif
}
