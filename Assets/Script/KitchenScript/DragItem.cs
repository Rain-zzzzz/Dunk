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

        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);

        foreach (var hit in hits)
        {
            if (hit.gameObject.name == "CutBoard_Collider" && isFullApple)
            {
                TableManager.Instance.CutApple(this);
                return;
            }
            if (hit.gameObject.name == "Plate_Collider" && !isFullApple)
            {
                TableManager.Instance.PutCutAppleToPlate(this);
                return;
            }
        }
        transform.SetParent(originParent);
        transform.position = originPos;
    }
}