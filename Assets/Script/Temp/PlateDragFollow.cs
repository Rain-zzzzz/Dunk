using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlateDragFollow : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("松手归位速度（0 = 瞬间归位）")]
    public float returnSpeed = 8f;

    [Header("拖动时放大倍数（1 = 不放大）")]
    public float dragScale = 2f;

    [Header("松手时是否检测 DropToAnimator 触发区")]
    public bool detectDropZone = true;

    private RectTransform rt;
    private RectTransform parentRt;
    private Vector2 homePos;
    private int homeIndex;
    private bool isDragging;
    private Vector3 originScale;

    private void Awake()
    {
        rt = (RectTransform)transform;
        parentRt = transform.parent as RectTransform;
        homeIndex = transform.GetSiblingIndex();
        originScale = rt.localScale;
    }

    private void Start()
    {
        homePos = rt.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData e)
    {
        isDragging = true;
        transform.SetAsLastSibling();
        if (!Mathf.Approximately(dragScale, 1f))
            rt.localScale = originScale * dragScale;
    }

    public void OnDrag(PointerEventData e)
    {
        Follow(e);
    }

    public void OnPointerUp(PointerEventData e)
    {
        isDragging = false;

        if (detectDropZone)
            TryTriggerDropZone(e);

        transform.SetSiblingIndex(homeIndex);
        rt.localScale = originScale;

        if (Mathf.Approximately(returnSpeed, 0f))
            rt.anchoredPosition = homePos;
    }

    private void Update()
    {
        if (!isDragging && returnSpeed > 0f)
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, homePos, returnSpeed * Time.deltaTime);
    }

    private void Follow(PointerEventData e)
    {
        if (parentRt == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRt, e.position, e.pressEventCamera, out Vector2 localPoint))
        {
            rt.anchoredPosition = localPoint;
        }
    }

    private void TryTriggerDropZone(PointerEventData e)
    {
        Graphic[] graphics = GetComponentsInChildren<Graphic>(true);
        List<Graphic> toggled = new List<Graphic>();
        foreach (var g in graphics)
        {
            if (g.raycastTarget)
            {
                g.raycastTarget = false;
                toggled.Add(g);
            }
        }

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(e, results);

        foreach (var g in toggled)
            g.raycastTarget = true;

        foreach (var r in results)
        {
            if (r.gameObject == null) continue;

            var zone = r.gameObject.GetComponentInParent<DropToAnimator>();
            if (zone == null) continue;

            var tag = GetComponentInParent<DraggableItemTag>();
            if (tag == null)
                tag = GetComponentInChildren<DraggableItemTag>(true);

            if (tag == null || string.IsNullOrEmpty(tag.itemName))
            {
                return;
            }

            zone.HandleDrop(tag.itemName);
            return;
        }
    }
}
