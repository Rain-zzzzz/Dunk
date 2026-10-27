using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    [Header("弹窗面板")]
    public GameObject panelSciencePop;

    [Header("按钮")]
    public Button btnScience;
    public Button btnCloseScience;

    [Header("动画设置")]
    public float fadeDuration = 0.3f; // 动画时长，单位秒
    private CanvasGroup canvasGroup;
    private bool isFading = false;

    void Start()
    {
        // 给弹窗添加CanvasGroup组件（如果没有的话）
        canvasGroup = panelSciencePop.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panelSciencePop.AddComponent<CanvasGroup>();
        }

        // 初始状态：隐藏弹窗，透明度为0
        panelSciencePop.SetActive(false);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // 绑定按钮事件
        btnScience.onClick.AddListener(ShowSciencePopup);
        btnCloseScience.onClick.AddListener(CloseSciencePopup);
    }

    // 显示科普弹窗（淡入）
    public void ShowSciencePopup()
    {
        if (isFading) return;

        panelSciencePop.SetActive(true);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        StartCoroutine(Fade(0, 1, fadeDuration));
    }

    // 关闭科普弹窗（淡出）
    public void CloseSciencePopup()
    {
        if (isFading) return;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        StartCoroutine(Fade(1, 0, fadeDuration, () =>
        {
            panelSciencePop.SetActive(false);
        }));
    }

    // 淡入淡出协程
    private System.Collections.IEnumerator Fade(float from, float to, float duration, System.Action onComplete = null)
    {
        isFading = true;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        canvasGroup.alpha = to;
        isFading = false;
        onComplete?.Invoke();
    }
}