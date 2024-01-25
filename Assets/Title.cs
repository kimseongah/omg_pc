using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Title : MonoBehaviour
{
    bool isCreateSelected = true;

    GameObject indicator;
    TextMeshProUGUI createText, exitText;

    Color selectedColor, unselectedColor;

    void Start()
    {
        indicator = GameObject.Find("Indicator");
        createText = GameObject.Find("Create Button").GetComponentInChildren<TextMeshProUGUI>();
        exitText = GameObject.Find("Exit Button").GetComponentInChildren<TextMeshProUGUI>();

        ColorUtility.TryParseHtmlString("#FFFFFF", out selectedColor);
        ColorUtility.TryParseHtmlString("#BDE5FA", out unselectedColor);
    }

    public void OnClickCreateBtn()
    {
        if (isCreateSelected)
        {
            SceneManager.LoadScene("ReadyScene");
        }
        else
        {
            isCreateSelected = true;
            indicator.GetComponent<RectTransform>().anchoredPosition = new Vector3(120, -396, 0);
            createText.color = selectedColor;
            exitText.color = unselectedColor;
        }
    }

    public void OnClickExitBtn()
    {
        if (!isCreateSelected)
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        else
        {
            isCreateSelected = false;
            indicator.GetComponent<RectTransform>().anchoredPosition = new Vector3(120, -588, 0);
            createText.color = unselectedColor;
            exitText.color = selectedColor;
        }
    }
}
