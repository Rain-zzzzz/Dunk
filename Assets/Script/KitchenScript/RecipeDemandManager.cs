using System.Collections.Generic;
using UnityEngine;

public class RecipeDemandManager : MonoBehaviour
{
    public static RecipeDemandManager Instance;
    // 当前需要的食材清单（对应购物清单显示的需求）
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
        // 订阅公开可访问的库存变更事件
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChangedExternal += RefreshAllDemandUI;
    }

    private void OnDisable()
    {
        // 取消订阅防止内存泄漏
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChangedExternal -= RefreshAllDemandUI;
    }

    // 设置当前工序需求（打开购物清单时调用）
    public void SetDemandList(List<ItemData> demandItems)
    {
        currentDemandList.Clear();
        currentDemandList.AddRange(demandItems);
        RefreshAllDemandUI();
    }

    // 清空需求清单（全部买完后调用）
    public void ClearDemand()
    {
        currentDemandList.Clear();
        RefreshAllDemandUI();
    }

    // 统一刷新购物清单UI
    void RefreshAllDemandUI()
    {
        UIShoppingList uiList = FindObjectOfType<UIShoppingList>();
        if (uiList != null)
            uiList.RefreshUI();
    }

    // 获取当前缺少的食材（过滤库存已有，只返回需要购买的）
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