using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originParent;
    public Vector3 originPos;
    public bool isFullApple;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originParent = transform.parent;
        originPos = transform.position;

        // 获取根画布并提升层级，拖拽时显示在最上层
        Canvas canvas = transform.root.GetComponent<Canvas>();
        if (canvas != null)
        {
            transform.SetParent(canvas.transform);
        }
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // 新建射线接收列表，作为RaycastAll第二个入参
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);

        foreach (var hit in hits)
        {
            // 拖拽完整苹果到菜板
            if (hit.gameObject.name == "CutBoard_Collider" && isFullApple)
            {
                TableManager.Instance.CutApple(this);
                return;
            }
            // 拖拽切块苹果到盘子
            if (hit.gameObject.name == "Plate_Collider" && !isFullApple)
            {
                TableManager.Instance.PutCutAppleToPlate(this);
                return;
            }
        }
        // 没有命中有效区域，放回原始父物体
        transform.SetParent(originParent);
        transform.position = originPos;
    }
}