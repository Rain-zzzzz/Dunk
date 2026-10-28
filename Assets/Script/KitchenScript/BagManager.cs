using UnityEngine;
using UnityEngine.UI;

public class BagManager : MonoBehaviour
{
    public static BagManager Instance;
    [Header("背包面板")]
    public GameObject bagPanel;
    public Image maskBg;
    public Button closeBtn;
    [Header("分页标签")]
    public Button[] tabBtns;
    public GameObject[] pages;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        closeBtn.onClick.AddListener(CloseBag);
        maskBg.GetComponent<Button>().onClick.AddListener(CloseBag);
        for (int i = 0; i < tabBtns.Length; i++)
        {
            int index = i;
            tabBtns[i].onClick.AddListener(() => SwitchPage(index));
        }
        SwitchPage(1);
    }

    public void OpenBag()
    {
        bagPanel.SetActive(true);
    }

    public void CloseBag()
    {
        bagPanel.SetActive(false);
    }

    void SwitchPage(int index)
    {
        foreach (var page in pages)
            page.SetActive(false);
        pages[index].SetActive(true);
    }
}