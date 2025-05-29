using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UISmelterPopup : MonoBehaviour, IGUI
{
    [SerializeField] private GUIItemSlotStation fuelSlot;
    [SerializeField] private GUIItemSlotStation metalSlot;
    [SerializeField] private GUIItemSlotStation resultSlot;

    private Dictionary<ResourceType, GUIItemSlotStation> slotStationDict;
    
    public GameObject GUIObject => gameObject;

    private void Reset()
    {
        fuelSlot = transform.Find("GUI_ItemSlot (Fuel)").GetComponent<GUIItemSlotStation>();
        metalSlot = transform.Find("GUI_ItemSlot (Metal)").GetComponent<GUIItemSlotStation>();
        resultSlot = transform.Find("GUI_ItemSlot (Result)").GetComponent<GUIItemSlotStation>();
    }

    public void Initialization()
    {
        slotStationDict = new Dictionary<ResourceType, GUIItemSlotStation>();

        fuelSlot.SetTitle("Fuel");
        metalSlot.SetTitle("Metal");
        resultSlot.SetTitle("Result");
        
        slotStationDict[ResourceType.Fuel] = fuelSlot;
        slotStationDict[ResourceType.Metal] = metalSlot;
        slotStationDict[ResourceType.None] = resultSlot;
        
        foreach (var slotStation in slotStationDict)
        {
            slotStation.Value.Initialization();
            switch (slotStation.Key)
            {
                case ResourceType.Fuel:
                case ResourceType.Metal:
                case ResourceType.None:
                    break;
            }
            slotStation.Value.SetTitle("Fuel");
        }
        
        fuelSlot.Initialization();
        metalSlot.Initialization();
        resultSlot.Initialization();
        
        gameObject.SetActive(false);
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
        // ItemData을 상속받는 ResourceItemData에 있는 정보를 가져오고 싶은데 어떻게 하면 될까?
        ResourceItemData resourceItem = item as ResourceItemData; // 비용이 클지 고민

        if (resourceItem == null)
        {
            Debug.LogError("Casting Failed");
            return false;
        }

        if (resourceItem.resourceType == ResourceType.Fuel && fuelSlot.IsEmpty())
        {
            fuelSlot.Show(item.icon);
            return true;
        }
        if (resourceItem.resourceType == ResourceType.Metal && metalSlot.IsEmpty()) 
        {
            metalSlot.Show(item.icon);
            return true;
        }

        Debug.Log("Slot is full!");
        return false;
    }
}
