using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragFollow : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public float returnSpeed = 8f;
    public float dragScale = 2f;
    public bool detectDropZone = true;

    private RectTransform rt;
    private Vector2 homePos;
    private int homeIndex;
    private bool isDragging;

    private Text[] childTexts;
    private bool[] textStates;
    private Vector3 originScale;

    private Image spriteImage;
    private Vector2 spriteCenterOffset;

    private UnityEngine.UI.LayoutGroup parentLayout;

    private void Awake()
    {
        rt = transform as RectTransform;
        homeIndex = transform.GetSiblingIndex();
        originScale = rt.localScale;

        if (transform.parent != null)
            parentLayout = transform.parent.GetComponent<UnityEngine.UI.LayoutGroup>();

        childTexts = GetComponentsInChildren<Text>(true);
        if (childTexts != null && childTexts.Length > 0)
        {
            textStates = new bool[childTexts.Length];
            for (int i = 0; i < childTexts.Length; i++)
                textStates[i] = childTexts[i].gameObject.activeSelf;
        }

        spriteImage = GetComponentInChildren<Image>(true);
        if (spriteImage != null)
        {
            RectTransform srt = spriteImage.rectTransform;
            spriteCenterOffset = srt.anchoredPosition;
        }
    }

    private void Start()
    {
        homePos = rt.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData e)
    {
        isDragging = true;
        transform.SetAsLastSibling();

        if (parentLayout != null) parentLayout.enabled = false;

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

        if (detectDropZone)
            TryTriggerDropZone(e);

        transform.SetSiblingIndex(homeIndex);

        SetDraggingState(false);

        if (parentLayout != null) parentLayout.enabled = true;
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
                tag = GetComponentInChildren<DraggableItemTag>();

            string itemName = (tag != null) ? tag.itemName : "";
            if (tag == null || string.IsNullOrEmpty(itemName))
            {
                return;
            }

            zone.HandleDrop(itemName);

            return;
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