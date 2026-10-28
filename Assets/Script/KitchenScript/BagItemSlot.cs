using UnityEngine;
using UnityEngine.UI;

public class BagItemSlot : MonoBehaviour
{
    public ItemData item;
    public Text countText;

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
        countText.text = $"ÊýÁ¿£º{count}";
    }
}
