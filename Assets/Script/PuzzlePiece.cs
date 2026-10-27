using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("正确位置")]
    public Vector2 correctPos;
    [Header("吸附范围（像素）")]
    public float snapRange = 25f;
    [Header("碎片序号")]
    public int pieceIndex;

    private RectTransform _rect;
    public static GameObject draggingPiece;
    public bool isLocked = false;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
       // correctPos = _rect.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        draggingPiece = gameObject;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rect.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out mousePos);
        _rect.anchoredPosition = mousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        draggingPiece = null;

        // 1. 先判断是否靠近正确位置，满足则吸附锁定
        float distance = Vector2.Distance(_rect.anchoredPosition, correctPos);
        if (distance < snapRange)
        {
            Debug.Log($"碎片 {pieceIndex} 即将吸附！当前坐标: {_rect.anchoredPosition}, 正确坐标: {correctPos}");
            _rect.anchoredPosition = correctPos;
            isLocked = true;
            PuzzleManager.Instance.CheckWinCondition();
            AudioGlobalManager.Instance.PlayPuzzleSuccess();
            return;
        }

        // 2. 未吸附，尝试和其他碎片交换位置
        GameObject hitObj = eventData.pointerEnter;
        if (hitObj != null && hitObj.CompareTag("PuzzlePiece"))
        {
            PuzzlePiece targetPiece = hitObj.GetComponent<PuzzlePiece>();
            // 对方已锁定，不交换
            if (targetPiece.isLocked)
            {
                PuzzleManager.Instance.CheckWinCondition();
                return;
            }

            // 交换位置
            RectTransform targetRect = hitObj.GetComponent<RectTransform>();
            Vector2 tempPos = _rect.anchoredPosition;
            _rect.anchoredPosition = targetRect.anchoredPosition;
            targetRect.anchoredPosition = tempPos;
        }

        PuzzleManager.Instance.CheckWinCondition();
    }

    // 解锁，用于重新开始
    public void Unlock()
    {
        isLocked = false;
    }
}