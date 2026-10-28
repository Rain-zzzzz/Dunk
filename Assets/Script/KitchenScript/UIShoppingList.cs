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
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChangedExternal += RefreshUI;
        SwitchCloseBtnLogic();
    }

    void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChangedExternal -= RefreshUI;
    }

    void SwitchCloseBtnLogic()
    {
        if (closeBtn == null || KitchenUIManager.Instance == null) return;
        closeBtn.onClick.RemoveAllListeners();

        bool shopIsOpen = KitchenUIManager.Instance.panelShop.activeSelf;
        if (shopIsOpen)
        {
            closeBtn.onClick.AddListener(KitchenUIManager.Instance.CloseShoppingListOnly);
        }
        else
        {
            closeBtn.onClick.AddListener(KitchenUIManager.Instance.CloseAll);
        }
    }

    public void RefreshUI()
    {
        if (gridParent == null || slotPrefab == null)
        {
            return;
        }
        if (requiredItems == null || requiredItems.Count == 0)
        {
            return;
        }

        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        foreach (var item in requiredItems)
        {
            if (item == null) continue;
            var inventory = InventoryManager.Instance;
            int have = inventory != null ? inventory.GetItemCount(item) : 0;
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

        }
    }
}