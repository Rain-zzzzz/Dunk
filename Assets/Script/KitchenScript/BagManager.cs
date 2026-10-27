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
        // 单例模式
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 绑定关闭事件
        closeBtn.onClick.AddListener(CloseBag);
        maskBg.GetComponent<Button>().onClick.AddListener(CloseBag);
        // 绑定分页切换按钮
        for (int i = 0; i < tabBtns.Length; i++)
        {
            int index = i;
            tabBtns[i].onClick.AddListener(() => SwitchPage(index));
        }
        // 默认打开食材页
        SwitchPage(1);
    }

    // 打开背包
    public void OpenBag()
    {
        bagPanel.SetActive(true);
    }

    // 关闭背包
    public void CloseBag()
    {
        bagPanel.SetActive(false);
    }

    // 切换分页
    void SwitchPage(int index)
    {
        // 隐藏所有页面
        foreach (var page in pages)
            page.SetActive(false);
        // 显示选中页面
        pages[index].SetActive(true);
    }
}