using System;
using System.Collections.Generic;
using UnityEngine;

public class UIEquipmentPopup : MonoBehaviour, IGUI
{
    // 필요한 것
    // 보유 스탯, 스탯 이름
    // 캐릭터 이미지 
    // key enum value equip dictionary로 하자!
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GUIItemSlotEquipment[] equipmentSlots;
    [SerializeField] private GUIItemSlotCraft craftSlot;
    
    private SerializableRecipe workbenchRecipe;
    private ItemData workbenchData;
    private bool isCraftable;
    
    private Dictionary<EquipSlot, GUIItemSlotEquipment> equipmentSlotDict;
    
    public GameObject GUIObject => gameObject;
    
    void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        equipmentSlots = GetComponentsInChildren<GUIItemSlotEquipment>();
        craftSlot = GetComponentInChildren<GUIItemSlotCraft>();
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
        
        workbenchData = ResourceManager.Instance.GetResource<ItemData>(StringAdrItemDataBuild.Workbench);
        craftSlot.Show(workbenchData.icon, 0, workbenchData);
        craftSlot.SetClickEvent(TryCraftWorkbench, 0);
        craftSlot.SetImageToSilhouette(false);

        isCraftable = false;
        
        // 비용이 너무 큼 ㅠ 그냥 팝업 껐다 키슈 ㅠ
        // CharacterManager.Player.inventoryController.UpdateInventory -= UpdateCraftSlotUI;
        // CharacterManager.Player.inventoryController.UpdateInventory += UpdateCraftSlotUI;
        
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
        
        workbenchRecipe = CraftManager.Instance.GetRecipe(workbenchData.itemName);

        UpdateCraftSlotUI();
        
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void UpdateCraftSlotUI()
    {
        isCraftable = CraftManager.Instance.CanCraftByItemName(workbenchData.itemName);
        craftSlot.SetImageToSilhouette(isCraftable);
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
    
    void TryCraftWorkbench(int index)
    {
        if (isCraftable)
        {
            // isMax 인지 체크 후 제작하기
            if (!CharacterManager.Player.inventoryController.IsInventoryFull())
            {
                CraftManager.Instance.ExecuteCraft(workbenchRecipe);
                UpdateCraftSlotUI();
            }
            else
            {
                MyDebug.Log("Inventory is Full! Please clear 1 slot in your inventory and try again.");
            }
        }
        else
        {
            MyDebug.Log("you can not craft workbench! because you have not ingredient.");
        }
    }
    
    #if UNITY_EDITOR
    public float CanvasAlpha() => canvasGroup.alpha;
    #endif
}
