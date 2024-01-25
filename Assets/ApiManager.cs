using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class ApiManager : MonoBehaviour
{
    public ServerConfig serverConfig;
    public TextMeshProUGUI code;
    GameObject obj;

    void Start()
    {
        getCode();
        obj = GameObject.Find("Socket");
    }

    void getCode()
    {
        StartCoroutine(UnityWebRequestGETTest());
    }

    IEnumerator UnityWebRequestGETTest()
    {
        string url = serverConfig.baseUrl + serverConfig.roomApi;

        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.error == null)
        {
            code.text = www.downloadHandler.text;
            obj.GetComponent<SocketManager>().OnReady(code.text);
        }
        else
        {
            Debug.Log("error");
        }
    }
}
