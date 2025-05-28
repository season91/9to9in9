using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// [제어] Inventory 아이템 추가, 제거, 사용 등 기능 구현
/// </summary>
///

//인벤토리 내에서 사용할 아이템 슬롯 클래스


public class PlayerInventoryController : MonoBehaviour
{
    class ItemSlot
    {
        private ItemData item;
        public int Quantity;

        public ItemSlot()
        {
            item = null;
            Quantity = 0;
        }

        public void InitSlot(ItemData item = null,int quantity=0)
        {
            this.item = item;
            this.Quantity = quantity;
        }
        public bool isMax => Quantity >= item.maxStack;
    
        public bool isItemExsit(ItemData item) => this.item == item;
        public bool CanStack() => item.isStackable && !isMax;
        public bool isSlotEmpty() => item == null;
    }
    
    private List<ItemSlot> inventoryItems;
    private List<ItemData> items;
    private List<EquipableItemData> equippedItems;

    [SerializeField] private int inventorySize = 21;
    
    public Action UpdateInventory;
    
    private void Awake()
    {
        inventoryItems = new List<ItemSlot>();
        items = new List<ItemData>();
        for (int i = 0; i < inventorySize; ++i)
        {
            inventoryItems.Add(new ItemSlot());
            items.Add(null);
        }
        equippedItems = new List<EquipableItemData>();
    }

    void Init()
    {
        //TODO
        //세이브/로드 기능 추가 시 json(혹은 파일) 읽어올 것
    }

    public void AddItem(ItemData item)
    {
        for (int i = 0; i < inventorySize; ++i)
        {
            if (items[i] == item && item.isStackable && inventoryItems[i].CanStack())
            {
                ++inventoryItems[i].Quantity;
                return;
            }
        }
        for (int j = 0; j < inventorySize; ++j)
        {
            if (inventoryItems[j].isSlotEmpty())
            {
                items[j] = item;
                inventoryItems[j].InitSlot(item, 1);
                return;
            }
        }
    }

    public void RemoveItem(int index)
    {
        if (items[index] == null) return;
        if(items[index].isStackable)
        {
            if (--inventoryItems[index].Quantity > 0) return;
        }
        items[index] = null;
        inventoryItems[index].InitSlot();
    }

    public int GetAllItemCount()
    {
        return items.Count;
    }
    
    /*public List<EquipableItemData> GetAllEquippedItems()
    {
        return equippedItems;
    }*/

    public Sprite GetIcon(EquipSlot slot)
    {
        foreach (EquipableItemData item in equippedItems)
        {
            if (item.equipSlot == slot) return item.icon;
        }
        return null;
    }
    
    public Sprite GetIcon(int index)
    {
        if (items[index] != null)
        {
            return items[index].icon;
        }
        return null;
    }

    public int GetPcs(int index)
    {
        return inventoryItems[index].Quantity;
    }
    
    public ItemData GetItem(int index)
    {
        return items[index];
    }

    public ItemData EquipItem(ItemData item,EquipType type)
    {
        /*int idx;
        for (int i = 0; i < inventorySize; ++i)
        {
            if(items[i] == item)
        }*/
        return item;
    }
    //
    // unequip 에서 enum type 넘어올 경우 해당 장비가 있는지 확인하고 제거
    //
    
    //
    // 
    //
    
    //list getallitem
    //itemdata getitem(int index)
    //Icon geticon(int index)
    //int getpcs(int index)
    //void additem(itemdata item)
    //void removeitem(int index)    
}
