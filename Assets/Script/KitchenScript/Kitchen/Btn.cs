using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Btn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("缩放参数（基于原始按钮尺寸做相对缩放）")]
    public float hoverScale = 1.1f;

    public float pressScale = 0.9f;
    public float duration = 0.1f;

    private Button btn;
    private RectTransform rect;
    private Vector3 _originScale;
    private bool _isPointerInside;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        _originScale = rect.localScale;
        btn = GetComponent<Button>();
    }

    private void Start()
    {
        btn.onClick.AddListener(ClickBtn);
    }

    private void ClickBtn()
    {
        //GameScene.instance.sfx.Play();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerInside = true;
        rect.DOKill();
        rect.DOScale(_originScale * hoverScale, duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerInside = false;
        rect.DOKill();
        rect.DOScale(_originScale, duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rect.DOKill();
        rect.DOScale(_originScale * pressScale, duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rect.DOKill();
        Vector3 target = _isPointerInside ? _originScale * hoverScale : _originScale;
        rect.DOScale(target, duration).SetEase(Ease.OutQuad);
    }

    private void OnDisable()
    {
        rect.DOKill();
        rect.localScale = _originScale;
    }
}