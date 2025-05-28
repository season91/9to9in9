using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// [제어] Inventory 아이템 추가, 제거, 사용 등 기능 구현
/// </summary>
public class PlayerInventoryController : MonoBehaviour
{
    private List<ItemData> items;
    private List<ItemData> equippedItems;
    
    void Init()
    {
        //TODO
        //세이브/로드 기능 추가 시 json(혹은 파일) 읽어올 것
    }

    public void AddItem(ItemData item)
    {
        items.Add(item);
    }

    public void RemoveItem(int index)
    {
        items.RemoveAt(index);
    }

    public List<ItemData> GetAllItems()
    {
        return items;
    }
    
    public Sprite GetIcon(int index)
    {
        return items[index].icon;
    }

    /*public int GetPcs(int index)
    {
        if (items[index].isStackable)
        {
            return items[index].amount;
        }
    }*/
    
    //list getallitem
    //itemdata getitem(int index)
    //Icon geticon(int index)
    //int getpcs(int index)
    //void additem(itemdata item)
    //void removeitem(int index)    
}
