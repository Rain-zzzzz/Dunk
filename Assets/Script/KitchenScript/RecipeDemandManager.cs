using System.Collections.Generic;
using UnityEngine;

public class RecipeDemandManager : MonoBehaviour
{
    public static RecipeDemandManager Instance;
    public List<ItemData> currentDemandList = new List<ItemData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChangedExternal += RefreshAllDemandUI;
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChangedExternal -= RefreshAllDemandUI;
    }

    public void SetDemandList(List<ItemData> demandItems)
    {
        currentDemandList.Clear();
        currentDemandList.AddRange(demandItems);
        RefreshAllDemandUI();
    }

    public void ClearDemand()
    {
        currentDemandList.Clear();
        RefreshAllDemandUI();
    }

    void RefreshAllDemandUI()
    {
        UIShoppingList uiList = FindObjectOfType<UIShoppingList>();
        if (uiList != null)
            uiList.RefreshUI();
    }

    public List<ItemData> GetMissingItems()
    {
        List<ItemData> missing = new List<ItemData>();
        foreach (var item in currentDemandList)
        {
            int have = InventoryManager.Instance.GetItemCount(item);
            int need = item.craftNeedNum;
            if (have < need)
                missing.Add(item);
        }
        return missing;
    }
}