using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// [제어] Inventory 아이템 추가, 제거, 사용 등 기능 구현
/// </summary>
///

public class PlayerInventoryController : MonoBehaviour
{
    //인벤토리 내에서 사용할 아이템 슬롯 클래스
    class ItemSlot
    {
        public ItemData item { get; private set; }
        public int Quantity;

        public ItemSlot()
        {
            item = null;
            Quantity = 0;
        }
        
        public ItemSlot(ItemData item, int quantity = 1)
        {
            this.item = item;
            Quantity = quantity;
        }
        
        public void InitSlot(ItemData item = null,int quantity=0)
        {
            this.item = item;
            this.Quantity = quantity;
        }
        public bool isMax => Quantity >= item.maxStack;
    
        public bool isItemExsit(ItemData item) => this.item == item;

        public bool isItemExsit(string name) => this.item.name == name;
        public bool CanStack() => item.isStackable && !isMax;
        public bool isSlotEmpty() => item == null;
    }
    
    private List<ItemSlot> inventoryItems;
    private List<ItemData> items;
    private List<EquipableItemData> equippedItems;

    //외부 읽기 전용 list 반환
    public IReadOnlyList<ItemData> Items => items;

    //[SerializeField] private int inventorySize = 21;
    
    public Action UpdateInventory;
    
    private void Awake()
    {
        inventoryItems = new List<ItemSlot>();
        items = new List<ItemData>();
        equippedItems = new List<EquipableItemData>();
    }

    void Init()
    {
        //TODO
        //세이브/로드 기능 추가 시 json(혹은 파일) 읽어올 것
    }

    public void AddItem(ItemData item, int quantity = 1)
    {
        for (int i = 0; i < items.Count; ++i)
        {
            if (inventoryItems[i].isItemExsit(item) && inventoryItems[i].CanStack())
            {
                inventoryItems[i].Quantity += quantity;
                return;
            }
        }
        items.Add(item);
        inventoryItems.Add(new ItemSlot(item, quantity));
    }
    
    
    //UI에서 아이템 사용 혹은 제거 시 불러올 메서드
    public void RemoveItem(int index)
    {
        if (index > inventoryItems.Count) return;
        if(items[index].isStackable)
        {
            if (--inventoryItems[index].Quantity > 0) return;
        }
        items.RemoveAt(index);
        inventoryItems.RemoveAt(index);
    }

    //재료 아이템 소모 시 호출할 메서드
    public void RemoveItem(string name, int quantity = 1)
    {
        for (int i = inventoryItems.Count - 1; i >= 0 && quantity > 0 ; --i)
        {
            if (!inventoryItems[i].isItemExsit(name)) continue;
            
            if (inventoryItems[i].Quantity > quantity)
            {
                inventoryItems[i].Quantity -= quantity;
                return;
            }
            else
            {
                quantity -= inventoryItems[i].Quantity;
                inventoryItems.RemoveAt(i);
                items.RemoveAt(i);
            }
        }
    }
    
    //단일 아이템 제거의 경우 호출
    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].isItemExsit(item))
            {
                inventoryItems.RemoveAt(i);
                return;
            }
        }
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

    public int GetPcs(string name)
    {
        int totalQuantity = 0;
        foreach (ItemSlot slot in inventoryItems)
        {
            if (slot.isItemExsit(name))
            {
                totalQuantity += slot.Quantity;
            }
        }
        return totalQuantity;
    }

    
    public ItemData GetItem(int index)
    {
        return items[index];
    }

    public bool EquipItem(ItemData item)
    {
        if (item is EquipableItemData equipItem)
        {
            RemoveItem(item);
            int targetIdx = equippedItems.FindIndex(item => item.equipSlot == equipItem.equipSlot);
            if (targetIdx == -1)
            {
                equippedItems.Add(equipItem);
            }
            else
            {
                ItemData temp = equippedItems[targetIdx];
                equippedItems.RemoveAt(targetIdx);
                equippedItems.Add(equipItem);
                AddItem(temp);
            }
            return true;
        }

        return false;
    }
    //
    // unequip 에서 enum type 넘어올 경우 해당 장비가 있는지 확인하고 제거
    //

    public bool UnEquipItem(EquipSlot slotType)
    {
        foreach (EquipableItemData equipItem in equippedItems)
        {
            if (equipItem.equipSlot != slotType) continue;
            AddItem(equipItem);
            equippedItems.Remove(equipItem);
            return true;
        }
        return false;
    }
    
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
