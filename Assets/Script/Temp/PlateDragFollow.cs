using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 专用于 Area_Plate 下直接子物体（如 Image_Plate）的鼠标拖拽跟随脚本。
///
/// 与通用版 DragFollow 的区别：
///  - 本物体自身就是精灵（Image 挂在挂载对象上），直接用自身 pivot 对齐鼠标，
///    不再做“子物体精灵偏移”换算（通用版会把自身 anchoredPosition 误当偏移减掉，
///    导致跟随位置错位）；
///  - 不处理父物体布局组（Area_Plate 上没有 LayoutGroup）；
///  - 松手时可选触发 DropToAnimator 触发区（与 Temp 目录下 DropToAnimator 配套）。
///
/// 使用：挂到 Area_Plate 下的 Image_Plate（或其它直接子物体）上即可。
/// 注意：Area_Plate 下不要有覆盖在本物体上方、且 Raycast Target 开启的透明遮挡物
/// （如 Plate_Collider），否则事件会被它拦截。
/// </summary>
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
        parentRt = transform.parent as RectTransform; // Area_Plate
        homeIndex = transform.GetSiblingIndex();
        originScale = rt.localScale;
    }

    private void Start()
    {
        // 首帧布局完成后再记录，保证拿到正确的初始位置
        homePos = rt.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData e)
    {
        isDragging = true;
        transform.SetAsLastSibling(); // 提到最上层，避免被兄弟节点挡住射线/渲染
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

    /// <summary>把鼠标屏幕坐标换算到父物体（Area_Plate）本地坐标，使自身中心对齐鼠标。</summary>
    private void Follow(PointerEventData e)
    {
        if (parentRt == null) return;

        // Canvas 为 Screen Space Overlay 时 pressEventCamera 传 null 是正确用法
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRt, e.position, e.pressEventCamera, out Vector2 localPoint))
        {
            // 本物体锚点在 Area_Plate 中心（AnchorMin=Max=0.5,0.5），
            // anchoredPosition 即相对父中心的偏移，直接赋值即可让 pivot 对齐鼠标
            rt.anchoredPosition = localPoint;
        }
    }

    /// <summary>
    /// 松手时用 RaycastAll 检测鼠标下是否有 DropToAnimator 触发区。
    /// 判定前临时关闭本物体射线的接收，让射线穿透自身打到下方触发区。
    /// </summary>
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
                Debug.LogWarning($"[PlateDragFollow] 「{gameObject.name}」上没找到 DraggableItemTag 或 itemName 为空，跳过触发。" +
                                 "（不影响拖拽跟随，仅无法触发放置动画）");
                return;
            }

            Debug.Log($"[PlateDragFollow] 松手命中触发区「{zone.gameObject.name}」，物品 =「{tag.itemName}」");
            zone.HandleDrop(tag.itemName);
            return;
        }
    }
}
