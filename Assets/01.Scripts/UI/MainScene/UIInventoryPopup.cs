using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIInventoryPopup : MonoBehaviour, IGUI
{
    [SerializeField] private List<GUIItemSlotInventory> inventorySlots;
    [SerializeField] private TextMeshProUGUI tmpTitle;
    
    PlayerInventoryController inventoryCtrlr;
    
    public GameObject GUIObject => gameObject;

    void Reset()
    {
        inventorySlots = transform.Find("Layout_PlayerItems")?.GetComponentsInChildren<GUIItemSlotInventory>().ToList();
        tmpTitle = transform.Find("Tmp_InventoryTitle")?.GetComponent<TextMeshProUGUI>();
    }
    
    public void Initialization()
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
        gameObject.SetActive(true);
        inventoryCtrlr = CharacterManager.Player.inventoryController;
        
        SettingInventoryUI();
    }
    
    public void Close()
    {
        gameObject.SetActive(false);
    }

    void SettingInventoryUI()
    {
        var items = inventoryCtrlr.GetAllItems();
        for (int i = 0; i < items.Count; i++)
        {
            if (i < items.Count)
            {
                int pcs = CharacterManager.Player.inventoryController.GetPcs(i);
                Sprite icon = inventoryCtrlr.GetIcon(i);
                inventorySlots[i].Show(icon, pcs);
            } 
        }   
    }

    void SettingSlotUI(int index)
    {
        int pcs = CharacterManager.Player.inventoryController.GetPcs(index);
        inventorySlots[index].SetPcs(pcs);
    }

    public void OnItemSlotSelected(int index)
    {
        ItemData item = inventoryCtrlr.GetItem(index);

        bool isSuccess = UIManager.Instance.TrySlotClickWithStation(item);

        if (isSuccess) {
            inventoryCtrlr.RemoveItem(index);
            SettingSlotUI(index);
        } else {
            Debug.Log("You can't place item in the slot! Slot is full!");
        }
    }
}
