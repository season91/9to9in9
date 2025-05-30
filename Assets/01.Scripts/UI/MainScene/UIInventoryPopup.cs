using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIInventoryPopup : MonoBehaviour, IGUI
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private List<GUIItemSlotInventory> inventorySlots;
    [SerializeField] private TextMeshProUGUI tmpTitle;

    private int curInventoryCount;
    
    PlayerInventoryController inventoryCtrlr;
    
    public GameObject GUIObject => gameObject;

    void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        inventorySlots = transform.Find("Layout_PlayerItems")?.GetComponentsInChildren<GUIItemSlotInventory>().ToList();
        tmpTitle = transform.Find("Tmp_InventoryTitle")?.GetComponent<TextMeshProUGUI>();
    }
    
    public void Initialization()
    {
        Close();
        InitializationInventoryGUI();
    }

    void InitializationInventoryGUI()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            int index = i; // 캡쳐 문제 회피
            inventorySlots[i].Initialization();
            inventorySlots[i].SetClickEvent(OnItemSlotSelected, index);
        }
    }

    public void Open()
    {
        StationType currentStation = UIManager.Instance.CurrentStation();
        
        switch (currentStation)
        {
            case StationType.None:
                tmpTitle.text = "Inventory";
                break;
            case StationType.Campfire:
                tmpTitle.text = "Campfire";
                break;
            case StationType.Smelter:
                tmpTitle.text = "Smelter";
                break;
            case StationType.Workbench:
            case StationType.Anvil:
            default:
                break;
        }
        
        inventoryCtrlr = CharacterManager.Player.inventoryController;
        
        inventoryCtrlr.UpdateInventory -= SettingInventoryUI;
        inventoryCtrlr.UpdateInventory += SettingInventoryUI;
        
        SettingInventoryUI();
        
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    
    public void Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void SettingInventoryUI()
    {
        int inventoryCount = inventoryCtrlr.GetAllItemCount();
        
        if (curInventoryCount != inventoryCount)
        {
            InitializationInventoryGUI();
        }
        
        curInventoryCount = inventoryCount;
        
        for (int i = 0; i < inventoryCount; i++)
        {
            if (i < inventoryCount)
            {
                int pcs = CharacterManager.Player.inventoryController.GetPcs(i);
                Sprite icon = inventoryCtrlr.GetIcon(i);
                if(icon == null)
                    return;
                inventorySlots[i].Show(icon, pcs);
            } 
        }   
    }

    void SettingSlotUI(int index)
    {
        int pcs = CharacterManager.Player.inventoryController.GetPcs(index);
        inventorySlots[index].SetPcs(pcs);
    }

    void OnItemSlotSelected(int index)
    {
        ItemData item = inventoryCtrlr.GetItem(index);

        bool isSuccess = UIManager.Instance.TrySlotClickWithStation(item);

        if (isSuccess) 
        {
            inventoryCtrlr.RemoveItem(index);
            // inventorySlots[index].Select();
        }
        else 
        {
            Debug.Log("You can't place item in the slot! Slot is full!");
        }
    }
    
#if UNITY_EDITOR
    public void TestOpen()
    {
        if (canvasGroup.alpha >= 0.5)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
#endif
}
