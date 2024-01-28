using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Resolution : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private GameObject check;
    private int value;

    public void Awake()
    {
        if (!PlayerPrefs.HasKey("ScreenWidth")) Save(1920);
        if (dropdown != null) { 
            dropdown.value = 0;
            for (int i = 0; i < dropdown.options.Count; i++)
            {
                if (dropdown.options[i].text.StartsWith(Load().ToString())) dropdown.value = i;
            }
        }
        if (check != null) check.GetComponent<Toggle>().isOn = Screen.fullScreen;
        
    }

    public void GetDropdownText()
    {
        try
        {
            string selectedText = dropdown.options[dropdown.value].text;
            string noWhite = selectedText.Replace(" ", "");
            string[] resolutions = noWhite.Split('x');
            int width = Int32.Parse(resolutions[0]);
            int height = Int32.Parse(resolutions[1]);
            Screen.SetResolution(width, height, Screen.fullScreen);

            Save(width);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            Screen.SetResolution(1920, 1080, false);
        }
    }
    public void FullscreenMode()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    private void Save(int width)
    {
        PlayerPrefs.SetInt("ScreenWidth", width);
    }

    private int Load()
    {
        return PlayerPrefs.GetInt("ScreenWidth");
    }
}
