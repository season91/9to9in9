using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIWorkbenchPopup : MonoBehaviour, IGUI
{
    // CraftType 
    // 조합
    // bool 제작할 수 있는가?
    // 조합법 
    // 모든 레시피 아이콘과 해당 아이콘이 제작할 수 있는가?
    // 딕셔너리  key: sprite, value: bool
    // sprite 
    // int형으로 index를
    // 모루 (type, 어떤 타입(장비,도구), index)
    [SerializeField] private RectTransform rectTransf;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GUIButtonCraftType[] btnCraftTypes;
    [SerializeField] private GUIItemSlotCraft[] itemSlots;

    private string curCraftType;
    private Dictionary<string, List<CraftableItemInfo>> craftableItemInfos;
    // private Dictionary<string, GUIButtonCraftType> btnCraftTypeDict;
    
    public GameObject GUIObject => gameObject;

    void Reset()
    {
        rectTransf = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        btnCraftTypes = transform.Find("Group_Buttons")?.GetComponentsInChildren<GUIButtonCraftType>();
        itemSlots = transform.Find("Group_Recipes")?.GetComponentsInChildren<GUIItemSlotCraft>();
    }

    public void Initialization()
    {
        // 리소스 매니저 리팩토링 후
        // 생성으로 리팩토링 해야 됨
        // All Button Disable
        for (int i = 0; i < btnCraftTypes.Length; i++)
        {
            btnCraftTypes[i].Initialization();
        }
        // All Slot Disable  
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].Initialization();
        }
        
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    
    public void Open()
    {
        craftableItemInfos = new();
        craftableItemInfos = CraftManager.Instance.GetRecipeOfStationType(StationType.Workbench);
        
        // Setting Button Craft Type => Enable
        int index = 0;
        foreach (var infoKvp in craftableItemInfos)
        {
            btnCraftTypes[index].Setting(infoKvp.Key);
            btnCraftTypes[index].SetClickEvent(SettingUIByClickType);
            index++;
        }
        
        SettingUIByClickType(btnCraftTypes[0].GetCraftType());
        curCraftType = btnCraftTypes[0].GetCraftType();
        
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

    void SettingUIByClickType(string type)
    {
        curCraftType = type;
        
        // 비용이 너무 큼 ㅠ 그냥 팝업 껐다 키슈 ㅠ
        // CharacterManager.Player.inventoryController.UpdateInventory -= () => SettingUIByClickType(curCraftType);
        // CharacterManager.Player.inventoryController.UpdateInventory += () => SettingUIByClickType(curCraftType);
        
        List<CraftableItemInfo> craftableInfoList = craftableItemInfos[type];

        // All Slot Disable  
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].Initialization();
        }
        
        for (int i = 0; i < craftableInfoList.Count; i++)
        {
            ItemData itemData = craftableInfoList[i].itemData;
            // 하나로 고정
            itemSlots[i].Show(itemData.icon, 0, itemData);
            itemSlots[i].SetClickEvent(SelectItemSlot, i);
            if (!CheckCraftableItem(itemData.itemName))
            {
                itemSlots[i].SetImageToSilhouette(false);
            }
        }
    }

    bool CheckCraftableItem(string itemName) => CraftManager.Instance.CanCraftByItemName(itemName);

    void SelectItemSlot(int index)
    {
        ItemData itemData = itemSlots[index].ItemData();
        
        if (!itemData)
        {
            MyDebug.Log("You can't Craft Item in this slot.");
            return;
        }
        
        // 만들 수 있는지 체크 후 제작
        if (CheckCraftableItem(itemData.itemName))
        {
            SerializableRecipe recipe = CraftManager.Instance.GetRecipe(itemData.itemName);
            CraftManager.Instance.ExecuteCraft(recipe);
            
            // GUI 업데이트
            SettingUIByClickType(curCraftType);
        }
        else
        {
            MyDebug.Log($"You can not Craft {itemData.itemName}! because you have not ingredient!!!");
        }
    }
    
#if UNITY_EDITOR
    public float CanvasAlpha() => canvasGroup.alpha;
#endif
}
