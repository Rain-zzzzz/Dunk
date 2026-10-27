using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShoppingList : MonoBehaviour
{
    [Header("当前工序所需物品")]
    public List<ItemData> requiredItems;
    [Header("格子预制体")]
    public GameObject slotPrefab;
    [Header("格子父容器")]
    public Transform gridParent;
    [Header("清单关闭按钮")]
    public Button closeBtn;

    void OnEnable()
    {
        RefreshUI();
        // 绑定库存监听
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChangedExternal += RefreshUI;
        // 每次打开自动切换关闭按钮逻辑
        SwitchCloseBtnLogic();
    }

    void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChangedExternal -= RefreshUI;
    }

    // 自动判断底层是商店还是厨房，切换关闭函数
    void SwitchCloseBtnLogic()
    {
        if (closeBtn == null || KitchenUIManager.Instance == null) return;
        // 清空手动绑定的所有事件，避免冲突
        closeBtn.onClick.RemoveAllListeners();

        // 判断商店是否正在显示
        bool shopIsOpen = KitchenUIManager.Instance.panelShop.activeSelf;
        if (shopIsOpen)
        {
            // 商店打开的清单：只关清单，商店留下
            closeBtn.onClick.AddListener(KitchenUIManager.Instance.CloseShoppingListOnly);
        }
        else
        {
            // 厨房打开的清单：全部关闭，回到厨房
            closeBtn.onClick.AddListener(KitchenUIManager.Instance.CloseAll);
        }
    }

    public void RefreshUI()
    {
        if (gridParent == null || slotPrefab == null)
        {
            Debug.LogError("清单缺少格子父物体/预制体赋值！");
            return;
        }
        if (requiredItems == null || requiredItems.Count == 0)
        {
            Debug.LogWarning("清单未配置需求食材！");
            return;
        }

        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        foreach (var item in requiredItems)
        {
            if (item == null) continue;
            int have = InventoryManager.Instance.GetItemCount(item);
            int need = item.craftNeedNum;

            GameObject slot = Instantiate(slotPrefab, gridParent);
            Transform iconTf = slot.transform.Find("Icon");
            Transform nameTf = slot.transform.Find("Name");
            Transform countTf = slot.transform.Find("Count");

            if (iconTf != null) iconTf.GetComponent<Image>().sprite = item.icon;
            if (nameTf != null) nameTf.GetComponent<Text>().text = item.itemName;
            if (countTf != null)
            {
                countTf.GetComponent<Text>().text = $"{have}/{need}";
                countTf.GetComponent<Text>().color = have < need ? Color.red : Color.green;
            }

            // 动态挂载拖拽组件：运行时从 prefab 实例化的物品默认不带 DragFollow/DraggableItemTag，
            // 这里补上，并按当前物品设置 itemName，保证能拖动到触发区。
            if (slot.GetComponent<DragFollow>() == null)
                slot.AddComponent<DragFollow>();
            var tag = slot.GetComponent<DraggableItemTag>();
            if (tag == null)
                tag = slot.AddComponent<DraggableItemTag>();
            tag.itemName = item.itemName;
        }
    }
}