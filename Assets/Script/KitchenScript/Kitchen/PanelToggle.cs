using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelToggle : MonoBehaviour
{
    public GameObject panel;
    public bool controll;
    private Button btn;
    private void Awake()
    {
        btn=GetComponent<Button>();
    }
    private void Start()
    {
        btn.onClick.AddListener(Controller);
    }
    void Controller()
    {
        panel.SetActive(controll);
    }
}
