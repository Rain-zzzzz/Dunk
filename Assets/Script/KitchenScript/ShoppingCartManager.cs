using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShoppingCartManager : MonoBehaviour
{
    [Header("结算面板赋值")]
    public Transform gridCartParent;    // Grid_CartItems
    public GameObject cartSlotPrefab;   // CartItemSlot预制体
    public Text totalPriceText;
    public GameObject panelCartRoot;    // Panel_ShoppingCart根面板

    private List<CartItemSlot> allCartSlots = new List<CartItemSlot>();
    private int totalAllPrice = 0;

    // ✅ 商店内打开结算面板（保留商店主面板，不调用全局CloseAllPanels）
    public void OpenCartPanel()
    {
        Debug.Log("=== 打开结算面板，执行刷新 ===");
        KitchenUIManager.Instance.OpenShoppingCart_InShop();
        RefreshCartList();
    }

    // 清空旧条目，重新生成购物清单缺料商品
    void RefreshCartList()
    {
        // 销毁旧条目
        foreach (Transform child in gridCartParent)
            Destroy(child.gameObject);
        allCartSlots.Clear();

        // 精确定位：从 KitchenUIManager 的 panelShoppingList 上找 UIShoppingList
        UIShoppingList shopList = null;
        if (KitchenUIManager.Instance != null && KitchenUIManager.Instance.panelShoppingList != null)
        {
            shopList = KitchenUIManager.Instance.panelShoppingList.GetComponent<UIShoppingList>();
            if (shopList == null)
                shopList = KitchenUIManager.Instance.panelShoppingList.GetComponentInChildren<UIShoppingList>(true);
        }

        // 兜底：如果面板引用丢了，再全局找
        if (shopList == null)
            shopList = FindObjectOfType<UIShoppingList>(true);

        // 诊断日志：确认抓到的是哪个对象
        Debug.Log($"[结算清单] 找到的购物清单对象：{(shopList != null ? shopList.gameObject.name : "null")}, requiredItems数量：{(shopList?.requiredItems?.Count ?? 0)}");

        if (shopList == null || shopList.requiredItems == null || shopList.requiredItems.Count == 0)
        {
            Debug.LogWarning("购物清单未配置需求物品");
            CalculateTotalPrice();
            return;
        }

        foreach (var item in shopList.requiredItems)
        {
            if (item == null) continue;
            int have = InventoryManager.Instance.GetItemCount(item);
            int need = item.craftNeedNum;
            int missing = need - have;
            if (missing <= 0) continue; // 已够，不需购买

            Debug.Log($"[结算清单] 生成商品：{item.itemName} | 缺{missing}个 | 单价{item.shopPrice}");
            GameObject slotObj = Instantiate(cartSlotPrefab, gridCartParent);
            CartItemSlot slot = slotObj.GetComponent<CartItemSlot>();
            slot.Init(item, item.shopPrice, missing, this);
            allCartSlots.Add(slot);
        }
        CalculateTotalPrice();
    }



    // 实时计算全部商品总价
    public void CalculateTotalPrice()
    {
        totalAllPrice = 0;
        foreach (var slot in allCartSlots)
        {
            totalAllPrice += slot.GetSinglePrice() * slot.GetSelectCount();
        }
        totalPriceText.text = $"总计：{totalAllPrice} 金币";
        Debug.Log($"当前总价：{totalAllPrice}");
    }

    // ✅ 支付按钮：购买完成后全局关闭面板，回到厨房
    public void OnPayClick()
    {
        // 1. 校验是否有商品
        if (allCartSlots.Count == 0)
        {
            Debug.Log("暂无需要购买的商品");
            return;
        }
        // 2. 校验金币是否充足
        bool goldEnough = GoldManager.Instance.DeductGold(totalAllPrice);
        if (!goldEnough)
        {
            Debug.Log("金币不足，无法支付");
            return;
        }
        // 3. 批量添加食材到背包
        foreach (var slot in allCartSlots)
        {
            InventoryManager.Instance.AddItem(slot.GetItem(), slot.GetSelectCount());
        }
        // 4. 清空采购需求清单
        InventoryManager.Instance.ClearShoppingNeedList();
        // ✅ 支付完成后全局关闭所有面板，回到厨房
        KitchenUIManager.Instance.CloseAll();
        Debug.Log("购买完成，清单已清空，背包已更新");
    }

    // ✅ 取消按钮：自动判断场景，只关结算面板、保留商店
    public void OnCancelClick()
    {
        KitchenUIManager.Instance.AutoCloseShoppingPanel();
    }
}