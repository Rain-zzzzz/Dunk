using UnityEngine;
using DG.Tweening;

public class PanelPop : MonoBehaviour
{
    [Header("弹出动画")]
    public float openDuration = 0.32f;
    public float closeDuration = 0.24f;
    [Header("回弹强度 0~2，推荐0.8‑1.3，越大弹得越猛")]
    public float overshoot = 1.15f;

    private RectTransform _rect;
    private Vector3 _originScale;
    private Tween _currentTween;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _originScale = _rect.localScale;
        _rect.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        PlayOpenAnim();
    }

    public void PlayOpenAnim()
    {
        _currentTween?.Kill();
        _rect.localScale = Vector3.zero;

        _currentTween = _rect.DOScale(_originScale, openDuration)
            .SetEase(Ease.OutBack, overshoot);
    }

    public void PlayCloseAnim()
    {
        _currentTween?.Kill();

        _currentTween = _rect.DOScale(Vector3.zero, closeDuration)
            .SetEase(Ease.InBack, overshoot)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    private void OnDisable()
    {
        _currentTween?.Kill();
    }
}