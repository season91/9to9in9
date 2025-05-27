using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// [제어] Inventory 아이템 추가, 제거, 사용 등 기능 구현
/// </summary>
public class PlayerInventoryController : MonoBehaviour
{
    private List<ItemData> items;

    public List<ItemData> Items
    {
        get { return items; }
    }
    void Init()
    {
        //TODO
        //세이브/로드 기능 추가 시 json(혹은 파일) 읽어올 것
    }

    public void AddItem(ItemData item)
    {
        items.Add(item);
    }

    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
    }

    public ItemData GetItem(int index)
    {
        return items[index];
    }

    public ItemData GetItem(string name)
    {
        return items.Find(x => x.name == name);
    }
}
