using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShoppingCartManager : MonoBehaviour
{
    [Header("结算面板赋值")]
    public Transform gridCartParent;
    public GameObject cartSlotPrefab;
    public Text totalPriceText;
    public GameObject panelCartRoot;

    private List<CartItemSlot> allCartSlots = new List<CartItemSlot>();
    private int totalAllPrice = 0;

    public void OpenCartPanel()
    {
        KitchenUIManager.Instance.OpenShoppingCart_InShop();
        RefreshCartList();
    }

    void RefreshCartList()
    {
        foreach (Transform child in gridCartParent)
            Destroy(child.gameObject);
        allCartSlots.Clear();

        UIShoppingList shopList = null;
        if (KitchenUIManager.Instance != null && KitchenUIManager.Instance.panelShoppingList != null)
        {
            shopList = KitchenUIManager.Instance.panelShoppingList.GetComponent<UIShoppingList>();
            if (shopList == null)
                shopList = KitchenUIManager.Instance.panelShoppingList.GetComponentInChildren<UIShoppingList>(true);
        }

        if (shopList == null)
            shopList = FindObjectOfType<UIShoppingList>(true);

        if (shopList == null || shopList.requiredItems == null || shopList.requiredItems.Count == 0)
        {
            CalculateTotalPrice();
            return;
        }

        foreach (var item in shopList.requiredItems)
        {
            if (item == null) continue;
            int have = InventoryManager.Instance.GetItemCount(item);
            int need = item.craftNeedNum;
            int missing = need - have;
            if (missing <= 0) continue;

            GameObject slotObj = Instantiate(cartSlotPrefab, gridCartParent);
            CartItemSlot slot = slotObj.GetComponent<CartItemSlot>();
            slot.Init(item, item.shopPrice, missing, this);
            allCartSlots.Add(slot);
        }
        CalculateTotalPrice();
    }

    public void CalculateTotalPrice()
    {
        totalAllPrice = 0;
        foreach (var slot in allCartSlots)
        {
            totalAllPrice += slot.GetSinglePrice() * slot.GetSelectCount();
        }
        totalPriceText.text = $"总计：{totalAllPrice} 金币";
    }

    public void OnPayClick()
    {
        if (allCartSlots.Count == 0)
        {
            return;
        }
        bool goldEnough = GoldManager.Instance.DeductGold(totalAllPrice);
        if (!goldEnough)
        {
            return;
        }
        foreach (var slot in allCartSlots)
        {
            InventoryManager.Instance.AddItem(slot.GetItem(), slot.GetSelectCount());
        }
        InventoryManager.Instance.ClearShoppingNeedList();
        KitchenUIManager.Instance.CloseAll();
    }

    public void OnCancelClick()
    {
        KitchenUIManager.Instance.AutoCloseShoppingPanel();
    }
}