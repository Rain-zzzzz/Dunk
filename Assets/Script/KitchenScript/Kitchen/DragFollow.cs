using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragFollow : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public float returnSpeed = 8f;
    public float dragScale = 2f; // 拖动时的放大倍数
    public bool detectDropZone = true;

    private RectTransform rt;
    private Vector2 homePos;
    private int homeIndex;
    private bool isDragging;

    private Text[] childTexts;        // 挂载对象所有子物体上的 Text（含嵌套）
    private bool[] textStates;        // 记录每个 Text 原本是否激活
    private Vector3 originScale;      // 记录原始缩放

    private Image spriteImage;        // 子物体上承载精灵的 Image
    private Vector2 spriteCenterOffset; // sprite 中心相对父对象锚点的偏移（父对象本地坐标）

    private UnityEngine.UI.LayoutGroup parentLayout; // 父物体上的布局组件（如 GridLayoutGroup），拖动时需临时禁用

    private void Awake()
    {
        rt = transform as RectTransform;
        // 注意：homePos 延后到 Start 记录，确保在 GridLayoutGroup 等布局完成后再取，
        // 避免动态 AddComponent 时拿到未布局的 (0,0)。
        homeIndex = transform.GetSiblingIndex();
        originScale = rt.localScale;

        // 记录父物体上的布局组件，拖动时临时禁用以避免 GridLayoutGroup 把位置打回网格
        if (transform.parent != null)
            parentLayout = transform.parent.GetComponent<UnityEngine.UI.LayoutGroup>();

        // 自动查找所有子物体上的 Text（含嵌套、含未激活的）
        childTexts = GetComponentsInChildren<Text>(true);
        if (childTexts != null && childTexts.Length > 0)
        {
            textStates = new bool[childTexts.Length];
            for (int i = 0; i < childTexts.Length; i++)
                textStates[i] = childTexts[i].gameObject.activeSelf;
        }

        // 查找子物体上的 Image（精灵），并计算其中心相对父对象的偏移
        spriteImage = GetComponentInChildren<Image>(true);
        if (spriteImage != null)
        {
            RectTransform srt = spriteImage.rectTransform;
            spriteCenterOffset = srt.anchoredPosition; // 子物体锚点相对父 pivot 的偏移
        }
    }

    private void Start()
    {
        // 布局完成后再记录归位位置
        homePos = rt.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData e)
    {
        isDragging = true;
        transform.SetAsLastSibling();

        // 临时禁用父物体布局，让物品能自由跟随鼠标而不是被网格布局拉回
        if (parentLayout != null) parentLayout.enabled = false;

        // 拖动开始：隐藏 Text，并放大 dragScale 倍
        SetDraggingState(true);

        Follow(e);
    }

    public void OnDrag(PointerEventData e)
    {
        Follow(e);
    }

    public void OnPointerUp(PointerEventData e)
    {
        isDragging = false;

        // 松手时检测是否落在触发区（方案A：判定放在物品自己的松手回调里）
        if (detectDropZone)
            TryTriggerDropZone(e);

        transform.SetSiblingIndex(homeIndex);

        // 拖动结束：还原 Text 显示与原始缩放
        SetDraggingState(false);

        // 恢复父物体布局
        if (parentLayout != null) parentLayout.enabled = true;
    }

    /// <summary>
    /// 松手时用 RaycastAll 检测鼠标下是否有 DropToAnimator 触发区，命中则按物品标识触发动画。
    /// 判定前临时关闭本物品的射线接收，让射线穿透物品打到下方的触发区。
    /// </summary>
    private void TryTriggerDropZone(PointerEventData e)
    {
        // 临时关闭本物体上所有 Graphic 的 raycastTarget，避免物品自己挡住射线
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

        // 恢复射线
        foreach (var g in toggled)
            g.raycastTarget = true;

        foreach (var r in results)
        {
            if (r.gameObject == null) continue;

            // 找触发区（挂在命中物体或其父物体上）
            var zone = r.gameObject.GetComponentInParent<DropToAnimator>();
            if (zone == null) continue;

            // 读取本物品的标识（向上找父级、也向下找子级，覆盖 Tag 挂在不同层级的情况）
            var tag = GetComponentInParent<DraggableItemTag>();
            if (tag == null)
                tag = GetComponentInChildren<DraggableItemTag>();

            string itemName = (tag != null) ? tag.itemName : "";
            if (tag == null || string.IsNullOrEmpty(itemName))
            {
                Debug.LogWarning($"[DragFollow] 物品「{gameObject.name}」上没找到 DraggableItemTag 或 itemName 为空！" +
                                 "请确认 Tag 挂在物品上且 itemName 已填写。");
                return;
            }

            Debug.Log($"[DragFollow] 松手命中触发区「{zone.gameObject.name}」，物品标识=「{itemName}」");

            // 统一交给触发区，触发区按名字自动判定是食材还是触发物
            zone.HandleDrop(itemName);

            return; // 命中第一个触发区即停止
        }
    }

    private void Update()
    {
        if (!isDragging)
        {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, homePos, returnSpeed * Time.deltaTime);
        }
    }

    private void Follow(PointerEventData e)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rt.parent as RectTransform, e.position, e.pressEventCamera, out Vector2 p);
        // 使 sprite 中心对齐鼠标位置（减去 sprite 中心相对父对象的偏移）
        rt.anchoredPosition = p - spriteCenterOffset;
    }

    private void SetDraggingState(bool dragging)
    {
        if (dragging)
        {
            if (childTexts != null)
                foreach (var t in childTexts)
                    t.gameObject.SetActive(false);
            rt.localScale = originScale * dragScale;
        }
        else
        {
            if (childTexts != null)
                for (int i = 0; i < childTexts.Length; i++)
                    childTexts[i].gameObject.SetActive(textStates[i]);
            rt.localScale = originScale;
        }
    }
}