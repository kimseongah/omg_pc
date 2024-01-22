using System;
using System.Collections;
using System.Collections.Generic;
using SocketIOClient;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

[Serializable]
class JoinData
{
    public string name;
    public int statusCode;

    public JoinData(string name)
    {
        this.name = name;
    }
}

[Serializable]
class SensorData
{
    public string name;
    public double accX, accY, accZ, accT, gyrX, gyrY, gyrZ, gyrT;
}

public class SocketManager : MonoBehaviour
{
    public ServerConfig serverConfig;
    public SocketIOUnity socket;
    public TextMeshProUGUI player1, player2;
    short playerN = 0;
    JoinData[] player = new JoinData[2];
    GameObject obj;

    static SocketManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        var uri = new Uri(serverConfig.baseUrl);
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

        obj = GameObject.Find("Game Start");
    }

    public void OnReady(string code)
    {
        socket.OnUnityThread("ready " + code, (response) =>
        {
            JoinData data = JsonUtility.FromJson<JoinData>(response.ToString().Trim('[', ']'));
            if (data.statusCode == 201)
            {
                if (playerN == 0)
                {
                    player[0] = new JoinData(data.name);
                    player1.text = data.name;
                    playerN++;
                }
                else if (playerN == 1)
                {
                    if (player[0] == null)
                    {
                        player[0] = new JoinData(data.name);
                        player1.text = data.name;
                    }
                    else
                    {
                        player[1] = new JoinData(data.name);
                        player2.text = data.name;
                    }
                    playerN++;
                    StartCoroutine(StartIf2(code));
                }
            }
            else if (data.statusCode == 200)
            {
                if (player[0] != null && player[0].name == data.name)
                {
                    player[0] = null;
                    player1.text = "Player 1";
                    playerN--;
                }
                else if (player[1] != null && player[1].name == data.name)
                {
                    player[1] = null;
                    player2.text = "Player 2";
                    playerN--;
                }
            }
        });
    }

    IEnumerator StartIf2(string code)
    {
        yield return new WaitForSeconds(5.0f);
        socket.OnUnityThread("game " + code, (response) =>
        {
            SensorData data = JsonUtility.FromJson<SensorData>(response.ToString().Trim('[', ']'));
            if (player[0].name == data.name)
            {
                /* TODO */
            } else if (player[1].name == data.name)
            {
                /* TODO */
            }
        });
        socket.Emit("start", code);
        obj.GetComponent<SceneChange>().GameScene();
    }


    public void Disconnect()
    {
        socket.Disconnect();
    }
}