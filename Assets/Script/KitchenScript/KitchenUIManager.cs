using UnityEngine;

public class KitchenUIManager : MonoBehaviour
{
    public static KitchenUIManager Instance;

    [Header("面板引用")]
    public GameObject panelItemTable;
    public GameObject panelBackpack;
    public GameObject panelGold;
    public GameObject panelShoppingList;
    public GameObject panelShop;
    public GameObject panelShoppingCart;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PlayClickSound()
    {
        if (AudioGlobalManager.Instance != null)
        {
            AudioGlobalManager.Instance.PlayClick();
        }
    }

    public bool IsShopOpen
    {
        get
        {
            return panelShop != null && panelShop.activeSelf;
        }
    }

    public void OpenPanel(GameObject panel)
    {
        PlayClickSound();
        CloseAllPanels();
        if (panel != null)
            panel.SetActive(true);
    }

    public void OpenShopSubPanel(GameObject panel)
    {
        PlayClickSound();
        if (panelShoppingList != null) panelShoppingList.SetActive(false);
        if (panelShoppingCart != null) panelShoppingCart.SetActive(false);

        if (panel != null)
            panel.SetActive(true);
    }

    public void CloseAllPanels()
    {
        if (panelItemTable != null) panelItemTable.SetActive(false);
        if (panelBackpack != null) panelBackpack.SetActive(false);
        if (panelGold != null) panelGold.SetActive(false);
        if (panelShoppingList != null) panelShoppingList.SetActive(false);
        if (panelShop != null) panelShop.SetActive(false);
        if (panelShoppingCart != null) panelShoppingCart.SetActive(false);
    }

    public void AutoCloseShoppingPanel()
    {
        PlayClickSound();
        if (IsShopOpen)
        {
            if (panelShoppingList != null) panelShoppingList.SetActive(false);
            if (panelShoppingCart != null) panelShoppingCart.SetActive(false);
        }
        else
        {
            CloseAllPanels();
        }
    }

    public void CloseShoppingListOnly()
    {
        PlayClickSound();
        if (panelShoppingList != null)
            panelShoppingList.SetActive(false);
    }

    public void CloseShoppingCartOnly()
    {
        PlayClickSound();
        if (panelShoppingCart != null)
            panelShoppingCart.SetActive(false);
    }

    public void OpenItemTable() => OpenPanel(panelItemTable);
    public void OpenBackpack() => OpenPanel(panelBackpack);
    public void OpenGold() => OpenPanel(panelGold);
    public void OpenShop() => OpenPanel(panelShop);
    public void OpenShoppingList_Global() => OpenPanel(panelShoppingList);
    public void OpenShoppingCart_Global() => OpenPanel(panelShoppingCart);

    public void OpenShoppingList_InShop() => OpenShopSubPanel(panelShoppingList);
    public void OpenShoppingCart_InShop() => OpenShopSubPanel(panelShoppingCart);

    public void CloseAll()
    {
        PlayClickSound();
        CloseAllPanels();
    }
}