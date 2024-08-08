using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class HakoXrParamServer : MonoBehaviour
{
    public int port = 38528;  // サーバーポート
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    void Start()
    {
        udpClient = new UdpClient(port);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
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
        udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
    }

    private void SendResponse(string message)
    {
        byte[] responseBytes = Encoding.ASCII.GetBytes(message);
        udpClient.Send(responseBytes, responseBytes.Length, remoteEndPoint);
        Debug.Log("Response sent: " + message);
    }

    private void ProcessReceivedData(string data)
    {
        HakoXrParams settings = JsonUtility.FromJson<HakoXrParams>(data);

        // 受信したデータに基づいて処理を行う
        Debug.Log($"Position: {settings.position[0]}, {settings.position[1]}, {settings.position[2]}");
        Debug.Log($"Rotation: {settings.rotation[0]}, {settings.rotation[1]}, {settings.rotation[2]}");
        // 必要な処理をここに追加
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}

[System.Serializable]
public class HakoXrParams
{
    public string server_url;
    public string client_ur;
    public float[] position;
    public float[] rotation;
}
