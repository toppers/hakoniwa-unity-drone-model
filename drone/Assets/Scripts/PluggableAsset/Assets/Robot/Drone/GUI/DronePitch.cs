using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DronePitch : MonoBehaviour
{
    private float deg_5 = 32; // ピッチ角が5度増えた場合のY座標の変化量
    public RectTransform pitchpicture; // ピッチ画像のRectTransform
    public GameObject drone; // ドローンのGameObject


    void Update()
    {
        // ドローンのピッチ角を取得
        float dronePitch = GetDronePitch();
        // ピッチ角を-180度から180度の範囲に正規化
        if (dronePitch > 180f)
        {
            dronePitch -= 360f;
        }
        // ピッチ角に応じたY座標の変化量を計算
        float yChange = -(dronePitch / 5) * deg_5;
        // ピッチ画像のY座標を変更
        pitchpicture.anchoredPosition = new Vector2(pitchpicture.anchoredPosition.x, yChange);

    }

    float GetDronePitch()
    {
        // ドローンのピッチ角度（X軸の回転角度）を取得
        return drone.transform.eulerAngles.x;
    }
}
