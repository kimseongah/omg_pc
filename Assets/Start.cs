using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void ReadyScene()
    {
        SceneManager.LoadScene("ReadyScene");
    }

    public void GameScene()
    {
        SceneManager.LoadScene("edited");
    }
}
