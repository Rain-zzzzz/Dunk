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
        closeBtn.onClick.AddListener(CloseGoldPanel);
        maskBg.GetComponent<Button>().onClick.AddListener(CloseGoldPanel);
    }

    void OnEnable()
    {
        RefreshGoldText(0);
        if (GoldManager.Instance != null)
            GoldManager.Instance.OnGoldChanged += RefreshGoldText;
    }

    void OnDisable()
    {
        if (GoldManager.Instance != null)
            GoldManager.Instance.OnGoldChanged -= RefreshGoldText;
    }

    public void OpenGoldPanel()
    {
        goldPanel.SetActive(true);
    }

    public void CloseGoldPanel()
    {
        goldPanel.SetActive(false);
    }

    public void RefreshGoldText(int unusedGold)
    {
        if (GoldManager.Instance == null)
        {
            goldNumText.text = "200";
            return;
        }
        int nowGold = GoldManager.Instance.GetCurrentGold();
        goldNumText.text = nowGold.ToString();
    }
}