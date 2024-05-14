using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCompass : MonoBehaviour
{
    public RectTransform arrow;  // 矢印のRectTransformをInspectorで設定
    public GameObject drone;

    void Update()
    {
        // ここでドローンの方向を取得します。例として、y軸の回転角度を使用します。
        float droneDirection = GetDroneDirection();

        // 矢印をドローンの方向に合わせて回転させます。
        arrow.localRotation = Quaternion.Euler(0, 0, -droneDirection);
    }

    float GetDroneDirection()
    {
        // ドローンの方向を取得する処理をここに記述します。
        // 例: ドローンのY軸の回転角度を返す
        return drone.transform.eulerAngles.y;
    }
}
