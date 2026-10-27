using System;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public class ItemSlot
{
    public ItemData item;
    public int count;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("初始物品（测试用）")]
    public List<ItemSlot> initialItems;

    // 实际库存（用List方便UI遍历）
    private List<ItemSlot> inventory = new List<ItemSlot>();

    // 库存变化事件（内部私有）
    public delegate void InventoryChanged();
    private event InventoryChanged OnInventoryChanged;

    void Start()
    {
        // ====== 方案二标准写法 + 打印日志排查加载问题 ======
        // 你的文件真实路径：Assets/Resources/ItemDatas/Food/Item_苹果
       /*ItemData apple = Resources.Load<ItemData>("ItemDatas/Food/Item_苹果");
        ItemData onion = Resources.Load<ItemData>("ItemDatas/Food/Item_洋葱");

        if (apple != null)
        {
            AddShoppingNeed(apple, 2);
            Debug.Log("✅ 苹果加载成功，已加入采购列表");
        }
        else
        {
            Debug.LogError("❌ 苹果加载失败，检查Resources文件夹层级与文件名字");
        }

        if (onion != null)
        {
            AddShoppingNeed(onion, 1);
            Debug.Log("✅ 洋葱加载成功，已加入采购列表");
        }
        else
        {
            Debug.LogError("❌ 洋葱加载失败，检查Resources文件夹层级与文件名字");
        }*/
    }

    // 对外公开包装事件，外部脚本仅能 += / -= 订阅，不会报CS0070
    public event InventoryChanged OnInventoryChangedExternal
    {
        add => OnInventoryChanged += value;
        remove => OnInventoryChanged -= value;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 加入初始物品
        foreach (var slot in initialItems)
        {
            AddItem(slot.item, slot.count);
        }
    }

    /// <summary>添加指定数量食材</summary>
    public void AddItem(ItemData item, int count = 1)
    {
        var existing = inventory.Find(s => s.item == item);
        if (existing != null)
        {
            existing.count += count;
        }
        else
        {
            inventory.Add(new ItemSlot { item = item, count = count });
        }
        OnInventoryChanged?.Invoke();
    }

    /// <summary>移除/扣除指定数量食材</summary>
    public bool RemoveItem(ItemData item, int count = 1)
    {
        var slot = inventory.Find(s => s.item == item);
        if (slot == null || slot.count < count) return false;

        slot.count -= count;
        if (slot.count <= 0)
            inventory.Remove(slot);

        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>扣除食材别名</summary>
    public bool DeductItem(ItemData item, int count)
    {
        return RemoveItem(item, count);
    }

    /// <summary>查询食材数量</summary>
    public int GetItemCount(ItemData item)
    {
        var slot = inventory.Find(s => s.item == item);
        return slot != null ? slot.count : 0;
    }

    /// <summary>获取全部库存</summary>
    public List<ItemSlot> GetAllItems()
    {
        return inventory;
    }

    // 采购需求列表
    private List<InventoryNeedItem> shoppingNeedList = new List<InventoryNeedItem>();

    public void AddShoppingNeed(ItemData item, int num)
    {
        bool hasItem = false;
        for (int i = 0; i < shoppingNeedList.Count; i++)
        {
            if (shoppingNeedList[i].item == item)
            {
                // 原有数量叠加
                InventoryNeedItem temp = shoppingNeedList[i];
                temp.needCount += num;
                shoppingNeedList[i] = temp;
                hasItem = true;
                break;
            }
        }
        if (!hasItem)
        {
            InventoryNeedItem newNeed = new InventoryNeedItem();
            newNeed.item = item;
            newNeed.needCount = num;
            shoppingNeedList.Add(newNeed);
        }
        OnInventoryChanged?.Invoke();
    }


    public List<InventoryNeedItem> GetAllNeedItems()
    {
        return new List<InventoryNeedItem>(shoppingNeedList);
    }

    public void ClearShoppingNeedList()
    {
        shoppingNeedList.Clear();
        OnInventoryChanged?.Invoke();
    }
}
