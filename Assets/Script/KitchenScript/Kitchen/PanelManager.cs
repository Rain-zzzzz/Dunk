using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;

    private void Awake()
    {
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
    }

    public void ShowApple()
    {
        panels[0].SetActive(true);
    }

    public void ShowOrange()
    {
        panels[1].SetActive(true);
    }

    public void ShowOnion()
    {
        panels[2].SetActive(true);
    }
    public void ShowTiaoLiao()
    {
        panels[3].SetActive(true);
    }
}