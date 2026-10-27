using UnityEngine;

public class KitchenUIManager : MonoBehaviour
{
    public static KitchenUIManager Instance;

    [Header("面板引用")]
    public GameObject panelItemTable;    // 操作台面板
    public GameObject panelBackpack;     // 背包面板
    public GameObject panelGold;         // 小金库面板
    public GameObject panelShoppingList; // 购物清单面板
    public GameObject panelShop;         // 商城主面板
    public GameObject panelShoppingCart; // 结算购物车面板

    private void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //播放点击音效，自带空判断防止报错
    private void PlayClickSound()
    {
        if (AudioGlobalManager.Instance != null)
        {
            AudioGlobalManager.Instance.PlayClick();
        }
    }

    // ✅ 判断当前是否处于商店界面
    public bool IsShopOpen
    {
        get
        {
            return panelShop != null && panelShop.activeSelf;
        }
    }

    // ✅ 全局打开面板（厨房场景用：关闭所有面板）
    public void OpenPanel(GameObject panel)
    {
        PlayClickSound();
        CloseAllPanels();
        if (panel != null)
            panel.SetActive(true);
    }

    // ✅ 商店内部打开子面板（仅关闭购物清单/结算面板，保留商店主面板）
    public void OpenShopSubPanel(GameObject panel)
    {
        PlayClickSound();
        // 只关闭商店子弹窗，不关闭panelShop商城主面板
        if (panelShoppingList != null) panelShoppingList.SetActive(false);
        if (panelShoppingCart != null) panelShoppingCart.SetActive(false);

        if (panel != null)
            panel.SetActive(true);
    }

    // ✅ 关闭全部面板（回到厨房主界面时调用）
    public void CloseAllPanels()
    {
        if (panelItemTable != null) panelItemTable.SetActive(false);
        if (panelBackpack != null) panelBackpack.SetActive(false);
        if (panelGold != null) panelGold.SetActive(false);
        if (panelShoppingList != null) panelShoppingList.SetActive(false);
        if (panelShop != null) panelShop.SetActive(false);
        if (panelShoppingCart != null) panelShoppingCart.SetActive(false);
    }

    // ✅ 自动适配关闭方法：根据是否在商店，决定只关弹窗 OR 全部关闭
    public void AutoCloseShoppingPanel()
    {
        PlayClickSound();
        if (IsShopOpen)
        {
            // 在商店：只关闭购物清单 + 结算面板，保留商店主面板
            if (panelShoppingList != null) panelShoppingList.SetActive(false);
            if (panelShoppingCart != null) panelShoppingCart.SetActive(false);
        }
        else
        {
            // 在厨房：全部关闭，回到厨房
            CloseAllPanels();
        }
    }

    // ✅ 仅关闭购物清单面板
    public void CloseShoppingListOnly()
    {
        PlayClickSound();
        if (panelShoppingList != null)
            panelShoppingList.SetActive(false);
    }

    // ✅ 仅关闭购物结算面板
    public void CloseShoppingCartOnly()
    {
        PlayClickSound();
        if (panelShoppingCart != null)
            panelShoppingCart.SetActive(false);
    }

    // ==== 全局打开方法（厨房跳转用） ====
    public void OpenItemTable() => OpenPanel(panelItemTable);
    public void OpenBackpack() => OpenPanel(panelBackpack);
    public void OpenGold() => OpenPanel(panelGold);
    public void OpenShop() => OpenPanel(panelShop);
    public void OpenShoppingList_Global() => OpenPanel(panelShoppingList);
    public void OpenShoppingCart_Global() => OpenPanel(panelShoppingCart);

    // ==== 商店内部打开子面板 ====
    public void OpenShoppingList_InShop() => OpenShopSubPanel(panelShoppingList);
    public void OpenShoppingCart_InShop() => OpenShopSubPanel(panelShoppingCart);

    // ✅ 全部关闭（支付完成/厨房场景使用）
    public void CloseAll()
    {
        PlayClickSound();
        CloseAllPanels();
    }
}