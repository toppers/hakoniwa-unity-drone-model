using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DronePos : MonoBehaviour
{
    public GameObject drone; // ドローンのGameObject
    public Text altitudeText; // 高度表示用のText
    public Text positionText; // 位置表示用のText
    public Text velocityText; // 速度表示用のText
    public Text distanceText; // 走行距離表示用のText

    private float altitudeScale = 1f; // 高度表示のスケール
    private float positionScale = 1f; // 位置表示のスケール

    private Vector3 previousPosition; // 前フレームのドローンの位置
    private float totalDistance; // 合計走行距離
    private Queue<Vector3> velocityQueue = new Queue<Vector3>(); // 速度のキュー
    private int velocityQueueSize = 100; // 速度の平均化に使用するサンプル数
    void Start()
    {
        previousPosition = drone.transform.position; // 初期位置を保存
        totalDistance = 0f; // 走行距離を初期化
    }

    void Update()
    {
        // ドローンの高度と位置を取得
        float altitude = GetDroneAltitude();
        Vector3 position = GetDronePosition();
        Vector3 velocity = GetSmoothedVelocity(GetDroneVelocity());

        // 走行距離を計算
        float distance = Vector3.Distance(position, previousPosition);
        totalDistance += distance;

        // 高度に応じたY座標の変化量を計算
        float altitudeYChange = altitude * altitudeScale;

        // 位置に応じたX, Z座標の変化量を計算
        float positionXChange = position.x * positionScale;
        float positionZChange = position.z * positionScale;

        // 高度と位置をテキストに更新
        altitudeText.text = "Altitude(m): " + altitude.ToString("F1");
        positionText.text = "Position(m): (" + position.x.ToString("F1") + ", " + position.z.ToString("F1") + ")";

        // 速度をテキストに更新
        // ベクトルの大きさを計算
        float speedMagnitude = Mathf.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z);

        // メートル毎秒(m/s)を時速(km/h)に変換
        float speedKmh = speedMagnitude * 3.6f;

        // 速度をテキストに更新
        velocityText.text = "Speed (km/h): " + speedKmh.ToString("F1");

        // 走行距離をテキストに更新
        distanceText.text = "Distance(m): " + totalDistance.ToString("F1");

        // 現在の位置を保存
        previousPosition = position;
    }

    float GetDroneAltitude()
    {
        // ドローンの高度を取得する処理をここに記述します
        return drone.transform.position.y;
    }

    Vector3 GetDronePosition()
    {
        // ドローンの位置を取得する処理をここに記述します
        return drone.transform.position;
    }

    Vector3 GetDroneVelocity()
    {
        // ドローンの速度を計算する
        Vector3 currentPosition = drone.transform.position;
        Vector3 velocity = (currentPosition - previousPosition) / Mathf.Max(Time.deltaTime, 0.0001f); // Time.deltaTimeが0にならないようにする
        return velocity;
    }
    Vector3 GetSmoothedVelocity(Vector3 newVelocity)
    {
        // 新しい速度をキューに追加
        velocityQueue.Enqueue(newVelocity);

        // キューのサイズを超えたら古い速度を削除
        if (velocityQueue.Count > velocityQueueSize)
        {
            velocityQueue.Dequeue();
        }

        // 平均速度を計算
        Vector3 smoothedVelocity = Vector3.zero;
        foreach (Vector3 v in velocityQueue)
        {
            smoothedVelocity += v;
        }
        smoothedVelocity /= velocityQueue.Count;

        return smoothedVelocity;
    }
}
