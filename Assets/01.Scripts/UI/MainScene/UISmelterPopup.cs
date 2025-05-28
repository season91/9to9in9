using System;
using UnityEngine;

public class UISmelterPopup : MonoBehaviour, IGUI
{
    [SerializeField] private GUIItemSlotBase fuelSlot;
    [SerializeField] private GUIItemSlotBase metalSlot;
    [SerializeField] private GUIItemSlotBase resultSlot;

    public GameObject GUIObject => gameObject;

    private void Reset()
    {
        fuelSlot = transform.Find("GUI_ItemSlot (Fuel)").GetComponent<GUIItemSlotBase>();
        metalSlot = transform.Find("GUI_ItemSlot (Metal)").GetComponent<GUIItemSlotBase>();
        resultSlot = transform.Find("GUI_ItemSlot (Result)").GetComponent<GUIItemSlotBase>();
    }

    public void Initialization()
    {
        
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
    
    public bool TryPlaceItem(ItemData item) 
    {
        // if (item.resourceType == ItemType.Fuel && fuelSlot.IsEmpty()) 
        // {
        //     fuelSlot.Show(item.icon);
        //     return true;
        // }
        // if (item.resourceType == ItemType.Metal && metalSlot.IsEmpty()) 
        // {
        //     metalSlot.Show(item.icon);
        //     return true;
        // }

        Debug.Log("슬롯이 가득 찼습니다.");
        return false;
    }
}
