using UnityEngine;
using UnityEngine.UI;

public class BagItemSlot : MonoBehaviour
{
    public ItemData item;       // 这个槽位对应的物品
    public Text countText;      // 显示"数量：X"的 Text

    void OnEnable()
    {
        Refresh();
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChangedExternal += Refresh;
    }

    void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChangedExternal -= Refresh;
    }

    void Refresh()
    {
        if (item == null || countText == null) return;
        int count = InventoryManager.Instance != null ? InventoryManager.Instance.GetItemCount(item) : 0;
        countText.text = $"数量：{count}";
    }
}
