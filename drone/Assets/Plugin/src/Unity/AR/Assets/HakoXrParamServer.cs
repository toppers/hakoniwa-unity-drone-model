using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

[System.Serializable]
public class HakoXrParams
{
    public string server_url;
    public string client_url;
    public float[] position;
    public float[] rotation;

    public HakoXrParams DeepCopy()
    {
        return new HakoXrParams
        {
            server_url = this.server_url,
            client_url = this.client_url,
            position = (float[])this.position.Clone(),
            rotation = (float[])this.rotation.Clone()
        };
    }
}

[System.Serializable]
public class HakoXrAdjustmentParams
{
    public float[] position = new float[3];
    public float[] rotation = new float[3];

    public HakoXrAdjustmentParams DeepCopy()
    {
        return new HakoXrAdjustmentParams
        {
            position = (float[])this.position.Clone(),
            rotation = (float[])this.rotation.Clone()
        };
    }
}

public class HakoXrParamServer
{
    public int port = 38528;  // サーバーポート
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    private bool is_exist_param = false;
    private HakoXrParams settings;
    private HakoXrAdjustmentParams adjustments;
    private int adjust_count = 0;

    private readonly object lockObject = new object(); // 排他制御用のオブジェクト

    public HakoXrParams GetHakoXrParams()
    {
        lock (lockObject)
        {
            if (is_exist_param)
            {
                return settings.DeepCopy();
            }
            else
            {
                return null;
            }
        }
    }

    public HakoXrAdjustmentParams GetAdjustmentParams()
    {
        lock (lockObject)
        {
            if (adjust_count > 0)
            {
                var obj = adjustments.DeepCopy();
                adjust_count = 0;
                adjustments = new HakoXrAdjustmentParams(); // adjustmentsを初期化
                return obj;
            }
            else
            {
                return null;
            }
        }
    }

    public void Initialize()
    {
        udpClient = new UdpClient(port);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        adjustments = new HakoXrAdjustmentParams(); // adjustmentsを初期化
        udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        Debug.Log("UDP server started on port " + port);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        byte[] receivedBytes = udpClient.EndReceive(ar, ref remoteEndPoint);
        string receivedData = Encoding.ASCII.GetString(receivedBytes);
        Debug.Log("Received: " + receivedData);

        // パラメータを反映する処理をここに追加
        ProcessReceivedData(receivedData);

        // 応答を送信
        SendResponse("Parameters received and applied successfully");

        // 再度リッスンを開始
        udpClient.BeginReceive(new AsyncCallback(ReceiveAdjustCallback), null);
    }

    private void ReceiveAdjustCallback(IAsyncResult ar)
    {
        byte[] receivedBytes = udpClient.EndReceive(ar, ref remoteEndPoint);
        string receivedData = Encoding.ASCII.GetString(receivedBytes);
        Debug.Log("AdjustReceived: " + receivedData);

        // パラメータを反映する処理をここに追加
        ProcessReceivedAdjustData(receivedData);

        // 応答を送信
        SendResponse("Adjustment parameters received and applied successfully");

        // 再度リッスンを開始
        udpClient.BeginReceive(new AsyncCallback(ReceiveAdjustCallback), null);
    }

    private void SendResponse(string message)
    {
        byte[] responseBytes = Encoding.ASCII.GetBytes(message);
        udpClient.Send(responseBytes, responseBytes.Length, remoteEndPoint);
        Debug.Log("Response sent: " + message);
    }

    private void ProcessReceivedData(string data)
    {
        lock (lockObject)
        {
            settings = JsonUtility.FromJson<HakoXrParams>(data);
            is_exist_param = true;

            // 受信したデータに基づいて処理を行う
            Debug.Log($"Position: {settings.position[0]}, {settings.position[1]}, {settings.position[2]}");
            Debug.Log($"Rotation: {settings.rotation[0]}, {settings.rotation[1]}, {settings.rotation[2]}");
        }
    }

    private void ProcessReceivedAdjustData(string data)
    {
        lock (lockObject)
        {
            this.adjust_count++;
            var obj = JsonUtility.FromJson<HakoXrAdjustmentParams>(data);

            for (int i = 0; i < 3; i++)
            {
                adjustments.position[i] += obj.position[i];
                adjustments.rotation[i] += obj.rotation[i];
            }

            // 受信したデータに基づいて処理を行う
            Debug.Log($"Adjusted Position: {adjustments.position[0]}, {adjustments.position[1]}, {adjustments.position[2]}");
            Debug.Log($"Adjusted Rotation: {adjustments.rotation[0]}, {adjustments.rotation[1]}, {adjustments.rotation[2]}");
        }
    }

    public void DestroyObjects()
    {
        udpClient.Close();
    }
}
