using UnityEngine;
using UnityEngine.UI;

public class GoldUIManager : MonoBehaviour
{
    public static GoldUIManager Instance;
    [Header("弹窗基础设置")]
    public GameObject goldPanel;
    public Image maskBg;
    public Button closeBtn;
    [Header("金币显示组件")]
    public Text goldNumText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 绑定关闭弹窗事件
        closeBtn.onClick.AddListener(CloseGoldPanel);
        maskBg.GetComponent<Button>().onClick.AddListener(CloseGoldPanel);
    }

    void OnEnable()
    {
        // 页面激活时立刻刷新 + 订阅金币变更事件
        RefreshGoldText(0);
        if (GoldManager.Instance != null)
            GoldManager.Instance.OnGoldChanged += RefreshGoldText;
    }

    void OnDisable()
    {
        // 页面关闭取消订阅，防止内存泄漏、多次重复刷新
        if (GoldManager.Instance != null)
            GoldManager.Instance.OnGoldChanged -= RefreshGoldText;
    }

    // 打开小金库弹窗
    public void OpenGoldPanel()
    {
        goldPanel.SetActive(true);
    }

    // 关闭小金库弹窗
    public void CloseGoldPanel()
    {
        goldPanel.SetActive(false);
    }

    // 增加int参数，匹配委托格式
    public void RefreshGoldText(int unusedGold)
    {
        if (GoldManager.Instance == null)
        {
            goldNumText.text = "200";
            Debug.LogError("场景缺少GoldManager全局金币管理器！");
            return;
        }
        int nowGold = GoldManager.Instance.GetCurrentGold();
        goldNumText.text = nowGold.ToString();
    }
}