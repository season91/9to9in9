using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIEquipmentPopup : MonoBehaviour, IGUI
{
    // 필요한 것
    // 보유 스탯, 스탯 이름
    [SerializeField] private RectTransform rectTransf;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GUIItemSlotEquipment[] equipmentSlots;
    [SerializeField] private GUIItemSlotCraft craftSlot;
    [SerializeField] private GUIStat[] stats;
    
    private SerializableRecipe workbenchRecipe;
    private ItemData workbenchData;
    private bool isCraftable;
    
    private Dictionary<EquipSlot, GUIItemSlotEquipment> equipmentSlotDict;
    
    public GameObject GUIObject => gameObject;
    
    void Reset()
    {
        rectTransf = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        equipmentSlots = GetComponentsInChildren<GUIItemSlotEquipment>();
        craftSlot = GetComponentInChildren<GUIItemSlotCraft>();
        stats = transform.Find("Layout_Stats")?.GetComponentsInChildren<GUIStat>();
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

        for (int i = 0; i < stats.Length; i++)
        {
            stats[i].Initialization();
        }
        
        workbenchData = ResourceManager.Instance.GetResource<ItemData>(StringAdrItemDataBuild.Workbench);
        craftSlot.Show(workbenchData.icon, 0, workbenchData);
        craftSlot.SetClickEvent(TryCraftWorkbench, 0);
        craftSlot.SetImageToSilhouette(false);

        isCraftable = false;
        
        // 비용이 너무 큼 ㅠ 그냥 팝업 껐다 키슈 ㅠ
        // CharacterManager.Player.inventoryController.UpdateInventory -= UpdateCraftSlotUI;
        // CharacterManager.Player.inventoryController.UpdateInventory += UpdateCraftSlotUI;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
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
        
        UpdateStatUI();
        UpdateCraftSlotUI();
        
        canvasGroup.DOFade(1, 0.2f);
        rectTransf.DOAnchorPosY(0, 0.3f);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Close()
    {
        canvasGroup.DOFade(0, 0.2f);
        rectTransf.DOAnchorPosY(300, 0.3f);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void UpdateStatUI()
    {
        var playerStatDict = CharacterManager.Player.statHandler.GetStatValues();

        int index = 0;
        foreach (var stat in playerStatDict)
        {
            stats[index++].Show(stat.Key, stat.Value.ToString("F0"));
        }
    }
    
    void UpdateCraftSlotUI()
    {
        isCraftable = CraftManager.Instance.CanCraftByItemName(workbenchData.itemName);
        craftSlot.SetImageToSilhouette(isCraftable);
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
