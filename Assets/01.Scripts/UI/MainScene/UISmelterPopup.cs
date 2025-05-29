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
        
        // fuelSlot.Initialization();
        // metalSlot.Initialization();
        // resultSlot.Initialization();
        
        slotStationDict[ResourceType.Fuel] = fuelSlot;
        slotStationDict[ResourceType.Metal] = metalSlot;
        slotStationDict[ResourceType.None] = resultSlot;
        
        foreach (var slotStation in slotStationDict)
        {
            slotStation.Value.Initialization();
            switch (slotStation.Key)
            {
                case ResourceType.Fuel:
                    fuelSlot.SetTitle("Fuel");
                    break;
                case ResourceType.Metal:
                    metalSlot.SetTitle("Metal");
                    break;
                case ResourceType.None:
                    resultSlot.SetTitle("Result");
                    break;
                default:
                    break;
            }
        }
        
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
            Debug.Log("Casting Failed! Is not Resource!");
            return false;
        }

        switch (resourceItem.resourceType)
        {
            case ResourceType.Fuel when fuelSlot.IsPlacePossible(resourceItem.icon):
                fuelSlot.Show(item.icon, 1, item);
                return true;
            case ResourceType.Metal when metalSlot.IsPlacePossible(resourceItem.icon):
                metalSlot.Show(item.icon, 1, item);
                return true;
            default:
                Debug.Log("Slot is full!");
                return false;
        }
    }
}
