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
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GUIItemSlotEquipment[] equipmentSlots;
    
    private Dictionary<EquipSlot, GUIItemSlotEquipment> equipmentSlotDict;
    
    public GameObject GUIObject => gameObject;
    
    void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        equipmentSlots = GetComponentsInChildren<GUIItemSlotEquipment>();
    }

    public void Initialization()
    {
        equipmentSlotDict = new Dictionary<EquipSlot, GUIItemSlotEquipment>();
        
        int slotIndex = 0;
        foreach (EquipSlot slotType in Enum.GetValues(typeof(EquipSlot)))
        {
            if (equipmentSlotDict.ContainsKey(slotType))
            {
                Debug.LogError($"already contains {slotType} in equipment slot");
            }
            equipmentSlots[slotIndex].Initialization();
            equipmentSlots[slotIndex].SetType(slotType);
            equipmentSlotDict[slotType] = equipmentSlots[slotIndex++];
            // 나중에 실루엣도 해당 타입에 맞는 이미지들로 변경해주기
        }
        
        Close();
    }

    public void Open()
    {
        foreach (var slot in equipmentSlotDict)
        {
            PlayerInventoryController inventoryController = CharacterManager.Player.inventoryController;
            var icon = inventoryController.GetIcon(slot.Key);

            if (icon)
            {
                slot.Value.Show(icon);
            }
        }
        
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

    public bool TryPlaceItem(ItemData item)
    {
        EquipableItemData equipableItem = item as EquipableItemData;
        
        if (equipableItem == null)
        {
            Debug.Log("Casting Failed! Is not Equipable!");
            return false;
        }        
        
        equipmentSlotDict[equipableItem.equipSlot].Show(equipableItem.icon);
        
        CharacterManager.Player.inventoryController.EquipItem(equipableItem);
        
        return true;
    }
    
    #if UNITY_EDITOR
    public float CanvasAlpha() => canvasGroup.alpha;
    #endif
}
