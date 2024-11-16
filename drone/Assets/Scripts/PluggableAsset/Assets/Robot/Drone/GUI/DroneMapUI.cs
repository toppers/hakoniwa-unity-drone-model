using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneMapUI : MonoBehaviour
{
    public GameObject drone;          // ドローンオブジェクト
    public RectTransform map;         // マップのUI（親オブジェクト）
    public RectTransform droneIcon;   // ドローン位置を示すUIオブジェクト
    public RectTransform droneRollIcon; // ロール角度を示すUIオブジェクト
    public RectTransform dronePitchIcon; // ピッチ角度を示すUIオブジェクト
    public Text scaleText;            // スケール表示用のテキストUI
    public float initialWorldSize = 500.0f;  // 初期設定の飛行可能範囲（ワールド空間の大きさ）
    public float map_adjust_scale = 0.8f;    // マップサイズ調整スケール
    public float pitch_adjust_scale = 2.0f;  // ピッチ角のスケール
    private float map_scale = 1.0f;          // 現在のマップスケール（スケール値）
    private float currentWorldSize;          // 現在のスケール範囲

    private Vector2 initialPitchIconPosition; // ピッチUIの初期位置を記録
    private Vector3 initialDronePosition;     // ドローンの初期位置を記録

    void Start()
    {
        // 初期値として設定されたワールドサイズを現在のワールドサイズに設定
        currentWorldSize = initialWorldSize;

        // 初期のスケールを表示
        UpdateScaleText();

        // ピッチアイコンの初期位置を記録
        initialPitchIconPosition = dronePitchIcon.anchoredPosition;

        // ドローンの初期位置を記録
        initialDronePosition = drone.transform.position;
    }

    void Update()
    {
        UpdateScale();
        UpdatePosition();
        UpdateRotation();
    }

    private void UpdateScale()
    {
        // ドローンの現在の位置と初期位置の距離を計算
        Vector3 realDronePos = drone.transform.position;
        float distanceFromInitial = Vector3.Distance(initialDronePosition, realDronePos);

        // ドローンが現在のスケール範囲を超えている場合
        if (distanceFromInitial > currentWorldSize)
        {
            // 現在のワールドサイズを拡大
            currentWorldSize *= 2.0f;
            map_scale *= 2.0f; // スケールが2倍になるので、表示倍率も更新

            // スケールテキストの更新
            UpdateScaleText();
        }

        // ドローンがスケール範囲内に戻ってきた場合（ヒステリシスを考慮して距離が十分小さくなったら）
        if (distanceFromInitial < currentWorldSize * 0.4f && currentWorldSize > initialWorldSize)
        {
            // 現在のワールドサイズを元に戻す
            currentWorldSize /= 2.0f;
            map_scale /= 2.0f; // スケールが小さくなるので、表示倍率も更新

            // スケールテキストの更新
            UpdateScaleText();
        }
    }



    private void UpdatePosition()
    {
        // マップの半径を取得
        float mapRadius = map_adjust_scale * map.rect.width / 2.0f; // 正方形のマップを前提にして幅の半分を半径とする

        // ドローンの現在の位置と初期位置の距離を計算
        Vector3 realDronePos = drone.transform.position;
        float distanceFromInitial = Vector3.Distance(initialDronePosition, realDronePos);

        // 現在のワールドサイズに応じてスケールを決定
        float scaleFactor = mapRadius / currentWorldSize; // ワールド空間からマップ空間へのスケール
        Vector2 direction = new Vector2(realDronePos.x - initialDronePosition.x, realDronePos.z - initialDronePosition.z).normalized;
        Vector2 mapLocalPos = direction * distanceFromInitial * scaleFactor;

        // 円形の範囲内に収めるためのクランプ処理
        if (mapLocalPos.magnitude > mapRadius)
        {
            mapLocalPos = mapLocalPos.normalized * mapRadius;
        }

        // ドローンアイコンの位置をマップ内の相対的な位置に設定
        droneIcon.localPosition = mapLocalPos;

    }

    private void UpdateRotation()
    {
        // ドローンの回転を取得してマップ内に反映
        Vector3 realDroneAngle = drone.transform.eulerAngles;

        // Yaw (方向) を反映
        Vector3 iconYaw = droneIcon.eulerAngles;
        iconYaw.z = -realDroneAngle.y; // ドローンの向きを反映 (Y軸の回転をZに反映)
        droneIcon.eulerAngles = iconYaw;

        // Roll (ロール角) を反映
        Vector3 iconRoll = droneRollIcon.eulerAngles;
        iconRoll.z = realDroneAngle.z;
        droneRollIcon.eulerAngles = iconRoll;

        // ピッチアイコンの高さを設定
        UpdatePitchIcon(realDroneAngle);
    }

    private void UpdatePitchIcon(Vector3 realDroneAngle)
    {
        // ピッチ角の設定
        float pitchAngle = NormalizeAngle(realDroneAngle.x); // 角度を -180 から 180 の範囲に正規化

        // ±40度の制限を適用
        pitchAngle = pitch_adjust_scale * Mathf.Clamp(pitchAngle, -40.0f, 40.0f);
        //Debug.Log("pitchAngle: " + pitchAngle);

        // ピッチ角を高さに反映 (初期位置からピッチに応じた高さを設定)
        Vector2 newPitchPosition = initialPitchIconPosition;
        newPitchPosition.y += pitchAngle; // プラスが上、マイナスが下になるようにY軸に反映
        dronePitchIcon.anchoredPosition = newPitchPosition;
    }

    // スケールテキストを更新するメソッド
    private void UpdateScaleText()
    {
        if (scaleText != null)
        {
            int scaleInt = Mathf.RoundToInt(map_scale);
            scaleText.text = $"1/{scaleInt}";
        }
    }

    // 角度を -180 から 180 に正規化するメソッド
    private float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }
}
