using UnityEngine;

[CreateAssetMenu(fileName = "ServerConfig", menuName = "ScriptableObjects/ServerConfig", order = 1)]
public class ServerConfig : ScriptableObject
{
    public string baseUrl, roomApi;
}
