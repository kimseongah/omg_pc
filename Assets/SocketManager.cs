using System;
using System.Collections.Generic;
using SocketIOClient;
using UnityEngine;

public class SocketManager : MonoBehaviour
{
    public SocketIOUnity socket;

    void Start()
    {
        var uri = new Uri("http://172.10.5.176");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });


        socket.Connect();


        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("Connected");
        };
        socket.OnDisconnected += (sender, e) => { Debug.Log("Disconnected: " + e); };
    }


    private void OnDestroy()
    {
        socket.Disconnect();
    }
}