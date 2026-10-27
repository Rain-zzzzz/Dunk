using UnityEngine;

public class TableManager : MonoBehaviour
{
    public static TableManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 切完整苹果
    public void CutApple(DragItem dragItem)
    {
        // 1. 扣除背包完整苹果
        InventoryManager.Instance.DeductItem(dragItem.GetComponent<ItemData>(), 1);
        // 2. 销毁拖拽物体，生成切块苹果
        Destroy(dragItem.gameObject);
        Debug.Log("完整苹果已切开");
    }

    // 切块苹果装盘
    public void PutCutAppleToPlate(DragItem dragItem)
    {
        // 1. 扣除切块苹果
        InventoryManager.Instance.DeductItem(dragItem.GetComponent<ItemData>(), 1);
        Destroy(dragItem.gameObject);
        Debug.Log("苹果块装盘完成");
    }
}