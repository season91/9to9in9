using System;
using System.Collections.Generic;
using UnityEngine;

public class UIEquipmentPopup : MonoBehaviour, IGUI
{
    // 필요한 것
    // equipment 슬롯
    // 보유 스탯, 스탯 이름
    // 제작 시스템 (제작대 만들 재료 있으면 활성화)
    // 캐릭터 이미지 
    // 인벤토리 클릭 시 장착된 장비 있으면 해제 후 장착
    // 현재 장착 중인 아이템 리스트
    // key enum value equip dictionary로 하자!
    
    [SerializeField] private GUIItemSlotEquipment[] equipmentSlots;
    
    private Dictionary<EquipSlot, GUIItemSlotEquipment> equipmentSlotDict;
    
    public GameObject GUIObject => gameObject;
    
    void Reset()
    {
        equipmentSlots = GetComponentsInChildren<GUIItemSlotEquipment>();
    }

    public void Initialization()
    {
        equipmentSlotDict = new Dictionary<EquipSlot, GUIItemSlotEquipment>();
        
        int slotIndex = 0;
        foreach (EquipSlot slot in Enum.GetValues(typeof(EquipSlot)))
        {
            if (equipmentSlotDict.ContainsKey(slot))
            {
                Debug.LogError($"already contains {slot} in equipment slot");
            }
            equipmentSlots[slotIndex].Initialization();
            equipmentSlots[slotIndex].SetType(slot);
            equipmentSlots[slotIndex].SetClickEvent(OnEquipSlotSelected);
            equipmentSlotDict[slot] = equipmentSlots[slotIndex++];
            // 나중에 실루엣도 해당 타입에 맞는 이미지들로 변경해주기
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);

        foreach (var slot in equipmentSlotDict)
        {
            PlayerInventoryController inventoryController = CharacterManager.Player.inventoryController;
            var icon = inventoryController.GetIcon(slot.Key);
            
            if(icon == null)
                return;
            slot.Value.Show(icon);
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void OnEquipSlotSelected(EquipSlot type)
    {
        // CharacterManager.Player.inventoryController.Unequip(type);
        // Unequip에서 원래 비어있는지 체크해주고 비어 있으면 ㄴㄴ 
    }

    // public bool TryPlaceItem(ItemData item)
    // {
    //     CharacterManager.Player.inventoryController.Equip(item);
    // }
}
