using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

        public ItemSlot(ItemSlot slot)
        {
            item = slot.item;
            Quantity = slot.Quantity;
        }
        
        public void InitSlot(ItemData item = null,int quantity=0)
        {
            this.item = item;
            this.Quantity = quantity;
        }
        public bool isMax => Quantity >= item.maxStack;
    
        public bool isItemExsit(ItemData item) => this.item == item;

        public bool isItemExsit(string name) => this.item.itemName == name;
        public bool CanStack() => item.isStackable && !isMax;
        public bool isSlotEmpty() => Quantity == 0;
    }
    
    private List<ItemSlot> inventoryItems;
    private List<ItemData> items;
    private List<EquipableItemData> equippedItems;
    private List<ItemSlot> quickSlotItems;
    
    //외부 읽기 전용 list 반환
    public IReadOnlyList<ItemData> Items => items;

    [SerializeField] private int inventorySize = 21;
    [SerializeField] private int quickSlotSize = 7;
    
    public Action UpdateInventory;
    
    private void Awake()
    {
        inventoryItems = new List<ItemSlot>();
        items = new List<ItemData>();
        equippedItems = new List<EquipableItemData>();
        quickSlotItems = new List<ItemSlot>();
    }

    void Init()
    {
        //TODO
        //세이브/로드 기능 추가 시 json(혹은 파일) 읽어올 것
    }

    public void AddItem(ItemData item, int quantity = 1)
    {
        if (items.Count == inventorySize)
        {
            #if UNITY_EDITOR
            Debug.LogWarning("인벤토리 최대 칸 수 넘어감!!");
                #endif
            return;
        }
        for (int i = 0; i < items.Count; ++i)
        {
            if (inventoryItems[i].isItemExsit(item) && inventoryItems[i].CanStack())
            {
                inventoryItems[i].Quantity += quantity;
                UpdateInventory?.Invoke();
                return;
            }
        }
        items.Add(item);
        inventoryItems.Add(new ItemSlot(item, quantity));
        UpdateInventory?.Invoke();
    }
    
    
    //UI에서 아이템 사용 혹은 제거 시 불러올 메서드
    public void RemoveItem(int index)
    {
        if (index >= inventoryItems.Count) return;
        if(items[index].isStackable)
        {
            if ((--inventoryItems[index].Quantity) <= 0) 
            { 
                items.RemoveAt(index);
                inventoryItems.RemoveAt(index);
            }
        }
        else
        {
            items.RemoveAt(index);
            inventoryItems.RemoveAt(index);
        }

        UpdateInventory?.Invoke();
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
                break;
            }
            else
            {
                quantity -= inventoryItems[i].Quantity;
                inventoryItems.RemoveAt(i);
                items.RemoveAt(i);
            }
        }
        UpdateInventory?.Invoke();
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
                UpdateInventory?.Invoke();
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
        if(index > inventoryItems.Count) return -1;
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
        if(index >= inventoryItems.Count) {return null;}
        return items[index];
    }

    public void EquipItem(EquipableItemData equipItem)
    {
        //RemoveItem(equipItem);    --UI에서 remove(int index)를 호출하기 때문에 한 번 더 지우는 행위를 하지 않아도 됨
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
        
        // 오른손이면 장비 화면에 띄우기
        if (equipItem.equipSlot == EquipSlot.RightHand)
            CharacterManager.Player.equipHandler.Equip(equipItem);
        
        // 스탯 업데이트
        CharacterManager.Player.equipHandler.UpdateStat(equipItem, equipItem.power);
        UpdateInventory?.Invoke();
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
            
            // 오른손이면 장비 화면에서 지우기
            if (equipItem.equipSlot == EquipSlot.RightHand)
                CharacterManager.Player.equipHandler.UnEquip();
            
            // 스탯 업데이트
            CharacterManager.Player.equipHandler.UpdateStat(equipItem, 0);
            UpdateInventory?.Invoke();
            return true;
        }
        return false;
    }
    
    
    //------퀵슬롯 이동 함수
    public void MoveItemToQuickSlot(int itemIndex)
    {
        if (quickSlotItems.Count >= quickSlotSize)
        {
            return;
        }

        foreach (ItemSlot slot in quickSlotItems)
        {
            if (inventoryItems[itemIndex].item == slot.item)
            {
                if (slot.CanStack())
                {
                    ++slot.Quantity;
                }
                else
                {
                    quickSlotItems.Add(new ItemSlot(inventoryItems[itemIndex]));
                } 
                RemoveItem(itemIndex);
                break;
            }
        }
        UpdateInventory?.Invoke();
    }

    public ItemData UseItemInQuickSlot(int quickSlotIndex)
    {
        if (quickSlotIndex >= quickSlotSize) return null;
        ItemData temp = quickSlotItems[quickSlotIndex].item;
        if (--quickSlotItems[quickSlotIndex].Quantity <= 0)
        {
            quickSlotItems.RemoveAt(quickSlotIndex);
        }
        UpdateInventory?.Invoke();
        return temp;
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
