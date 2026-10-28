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

    private List<ItemSlot> inventory = new List<ItemSlot>();

    public delegate void InventoryChanged();
    private event InventoryChanged OnInventoryChanged;

    void Start()
    {

    }

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

        foreach (var slot in initialItems)
        {
            AddItem(slot.item, slot.count);
        }
    }

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

    public bool DeductItem(ItemData item, int count)
    {
        return RemoveItem(item, count);
    }

    public int GetItemCount(ItemData item)
    {
        var slot = inventory.Find(s => s.item == item);
        return slot != null ? slot.count : 0;
    }

    public List<ItemSlot> GetAllItems()
    {
        return inventory;
    }

    private List<InventoryNeedItem> shoppingNeedList = new List<InventoryNeedItem>();

    public void AddShoppingNeed(ItemData item, int num)
    {
        bool hasItem = false;
        for (int i = 0; i < shoppingNeedList.Count; i++)
        {
            if (shoppingNeedList[i].item == item)
            {
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
