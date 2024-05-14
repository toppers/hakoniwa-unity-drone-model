using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDBlink : MonoBehaviour
{
    public Renderer ledRenderer; // LEDモジュールのRenderer
    public Light ledLight; // LEDモジュールのLight

    public Color onColor = Color.red; // 点灯時のカラー
    public Color offColor = Color.black; // 消灯時のカラー

    public float blinkInterval = 1.0f; // 点滅間隔
    public float blinkDuration = 0.5f; // 点滅のオン・オフ時間
    public float phaseOffset = 0.0f; // 位相のズレ

    private float elapsedTime; // 経過時間

    void Start()
    {
        // 経過時間を位相のズレで初期化
        elapsedTime = phaseOffset;

        // 初期状態をオフに設定
        SetLEDState(false);
    }

    void Update()
    {
        // 経過時間を更新
        elapsedTime += Time.deltaTime;

        // 経過時間が点滅間隔を超えたらリセット
        if (elapsedTime >= blinkInterval)
        {
            elapsedTime -= blinkInterval;
        }

        // 点灯状態を計算
        bool isOn = elapsedTime < blinkDuration;
        SetLEDState(isOn);
    }

    void SetLEDState(bool isOn)
    {
        Color color = isOn ? onColor : offColor;
        ledRenderer.material.color = color;
        ledLight.color = color;
        ledLight.enabled = isOn; // LEDライトを点灯/消灯
    }
}
