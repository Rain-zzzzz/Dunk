using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PanelManager : MonoBehaviour
{
    [Header("食材展示面板（0苹果 1橘子 2洋葱 3料汁）")]
    [SerializeField] private GameObject[] panels;

    [Header("步骤面板（0 Step2 1 Step3 2 Step4 3 Step5）")]
    [SerializeField] private GameObject[] stepPanels;

    [Header("背包面板")]
    [SerializeField] private GameObject backPackPanel;

    [Header("顶部进度条（留空会自动找 TopSlider）")]
    [SerializeField] private Slider topSlider;

    public static PanelManager Instance { get; private set; }

    private const int TotalSteps = 5; // 5 个步骤，每完成一步 20%
    private int currentStep;

    private void Awake()
    {
        bool isFirst = Instance == null;
        if (isFirst) Instance = this;

        HideAll(panels);
        HideAll(stepPanels);

        if (topSlider == null) topSlider = FindTopSlider();

        if (topSlider != null)
        {
            // 只改 Slider 的 value，不碰 Handle / Handle Slide Area 的激活状态和颜色状态。
            // 也不设置 interactable，避免 Unity 把 Handle 切到 Disabled 灰色状态。
            // 只允许第一个 PanelManager 初始化进度，避免场景里存在多余副本时把进度重置
            if (isFirst)
            {
                topSlider.DOKill();
                topSlider.value = 0f;
            }
        }
        else if (isFirst)
        {
            Debug.LogWarning("[PanelManager] 未找到 TopSlider，进度条不会更新。请把 TopSlider 下挂有 Slider 的子物体拖到 Top Slider 字段。", this);
        }
    }

    /// <summary>
    /// 完成第 completedSteps 步（1 ~ 5），进度条走到 completedSteps / 5。
    /// 进度只允许前进，重复的动画事件不会把进度条拉回去。
    /// </summary>
    public void SetProgress(int completedSteps)
    {
        // 场景里如果有多份 PanelManager，统一交给第一份处理，保证进度状态唯一
        if (Instance != null && Instance != this)
        {
            Instance.SetProgress(completedSteps);
            return;
        }

        completedSteps = Mathf.Clamp(completedSteps, 0, TotalSteps);
        if (completedSteps <= currentStep) return;

        currentStep = completedSteps;
        if (topSlider == null) return;

        float target = (float)currentStep / TotalSteps;
        topSlider.DOKill();

        // 面板没打开时 DOTween 不一定更新 UI，直接赋值保证下次打开时数值正确
        if (!topSlider.gameObject.activeInHierarchy)
        {
            topSlider.value = target;
            return;
        }

        topSlider.DOValue(target, 0.35f).SetEase(Ease.OutCubic);
    }

    public void ResetProgress()
    {
        if (Instance != null && Instance != this)
        {
            Instance.ResetProgress();
            return;
        }

        currentStep = 0;
        if (topSlider == null) return;

        topSlider.DOKill();
        topSlider.value = 0f;
    }

    public void ShowApple()
    {
        ShowPanel(0);
    }

    public void ShowOrange()
    {
        ShowPanel(1);
    }

    // 切完果蔬：20%，弹出“步骤二 料汁调配”
    public void ShowOnion()
    {
        ShowPanel(2);
        ShowStep(0);
        SetProgress(1);
    }

    // 倒完醋，展示料汁图标（只展示，不推进进度）
    public void ShowTiaoLiao()
    {
        ShowPanel(3);
    }

    // 料汁调配完成：40%，弹出“步骤三 烫鸭吹气”
    public void Step3()
    {
        ShowStep(1);
        SetProgress(2);
    }

    // 烫鸭吹气完成：60%，弹出“步骤四 填料”
    public void Step4()
    {
        ShowStep(2);
        SetProgress(3);
    }

    // 填料完成：80%，弹出“步骤五 刷汁”
    public void Step5()
    {
        ShowStep(3);
        SetProgress(4);
    }

    // 刷皮完成：100%
    public void CompleteBrush()
    {
        SetProgress(TotalSteps);
    }

    private void ShowPanel(int index)
    {
        GameObject panel = null;

        if (panels != null && index >= 0 && index < panels.Length)
        {
            panel = panels[index];
        }

        // 场景里有多份 PanelManager 时，用主实例的配置兜底
        if (panel == null && Instance != null && Instance != this && Instance.panels != null &&
            index >= 0 && index < Instance.panels.Length)
        {
            panel = Instance.panels[index];
        }

        if (panel != null) panel.SetActive(true);
    }

    private void ShowStep(int index)
    {
        GameObject stepPanel = null;

        if (stepPanels != null && index >= 0 && index < stepPanels.Length)
        {
            stepPanel = stepPanels[index];
        }

        if (stepPanel == null && Instance != null && Instance != this && Instance.stepPanels != null &&
            index >= 0 && index < Instance.stepPanels.Length)
        {
            stepPanel = Instance.stepPanels[index];
        }

        if (stepPanel != null) stepPanel.SetActive(true);

        if (backPackPanel != null)
        {
            backPackPanel.SetActive(false);
        }
    }

    private static void HideAll(GameObject[] targets)
    {
        if (targets == null) return;

        foreach (var target in targets)
        {
            if (target != null) target.SetActive(false);
        }
    }

    /// <summary>
    /// TopSlider 根物体本身可能没有 Slider 组件（Slider 挂在子物体上），
    /// 这里两种层级都能找到；Panel_ItemTable 未激活时也能找到。
    /// </summary>
    private Slider FindTopSlider()
    {
        foreach (var slider in FindObjectsOfType<Slider>(true))
        {
            if (slider.name == "TopSlider") return slider;

            if (slider.transform.parent != null && slider.transform.parent.name == "TopSlider")
            {
                return slider;
            }
        }

        return null;
    }
}
