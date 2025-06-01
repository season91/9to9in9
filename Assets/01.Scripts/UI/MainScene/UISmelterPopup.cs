using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class UISmelterPopup : MonoBehaviour, IGUI
{
    [SerializeField] private RectTransform rectTransf;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GUIItemSlotStation fuelSlot;
    [SerializeField] private GUIItemSlotStation metalSlot;
    [SerializeField] private GUIItemSlotStation resultSlot;
    [SerializeField] private RectTransform rectTrGauge;
    [SerializeField] private float maxWidthGauge;

    private Dictionary<ResourceType, GUIItemSlotStation> slotStationDict;
    private List<CraftableItemInfo> craftableItemInfos;
    
    Coroutine curGaugeCoroutine;
    Coroutine curUpdateUICoroutine;
    
    public GameObject GUIObject => gameObject;

    private void Reset()
    {
        rectTransf = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        fuelSlot = transform.Find("GUI_ItemSlot (Fuel)").GetComponent<GUIItemSlotStation>();
        metalSlot = transform.Find("GUI_ItemSlot (Metal)").GetComponent<GUIItemSlotStation>();
        resultSlot = transform.Find("GUI_ItemSlot (Result)").GetComponent<GUIItemSlotStation>();
        rectTrGauge = transform.Find("Group_Gauge/Img_Gauge").GetComponent<RectTransform>();
        maxWidthGauge = transform.Find("Group_Gauge/Img_GaugeBG").GetComponent<RectTransform>().sizeDelta.x;
    }

    public void Initialization()
    {
        slotStationDict = new Dictionary<ResourceType, GUIItemSlotStation>();
        
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
        
        rectTrGauge.gameObject.SetActive(false);

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    
    public void Open()
    {
        // key: 중분류 ("Tool", "Armor", "Weapon", 그외 "Default")
        // value: 중분류에 따른 레시피 결과 아이템 리스트
        if (craftableItemInfos == null)
        {
            craftableItemInfos = new List<CraftableItemInfo>();
            craftableItemInfos = CraftManager.Instance.GetRecipeOfStationType(StationType.Smelter)["Default"];
        }
        
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
    
    bool CheckAllSlotFull() => !slotStationDict[ResourceType.Fuel].IsEmpty 
                               && !slotStationDict[ResourceType.Metal].IsEmpty;
    
    public bool TryPlaceItem(ItemData item) 
    {
        // ItemData을 상속받는 ResourceItemData에 있는 정보를 가져오고 싶은데 어떻게 하면 될까?
        ResourceItemData resourceItem = item as ResourceItemData; // 비용이 클지 고민

        if (resourceItem == null)
        {
            MyDebug.Log("Casting Failed! Is not Resource!");
            return false;
        }

        ResourceType curResourceType = resourceItem.resourceType;
        if (!slotStationDict.TryGetValue(resourceItem.resourceType, out GUIItemSlotStation slot))
        {
            MyDebug.Log($"No slot found for resourceType: {resourceItem.resourceType}");
            return false;
        }
        
        if (curResourceType != ResourceType.Fuel && curResourceType != ResourceType.Metal )
        {
            MyDebug.Log($"This resource type({curResourceType}) does not apply to smelters!");
            return false;
        }
        
        if (!slot.IsPlacePossible(resourceItem.icon))
        {
            MyDebug.Log($"different item than {resourceItem.itemName} already exists.");
            return false;
        }
        
        slot.Show(item.icon, 1, item);
        
        if (curUpdateUICoroutine == null)
        {
            curUpdateUICoroutine = StartCoroutine(UpdateUIWhileCraftable());
        }
        
        return true;
    }
    
    // 제작 가능한 항목이 있을 동안 계속 돌아가도록!
    IEnumerator UpdateUIWhileCraftable()
    {
        try
        {
            if (craftableItemInfos == null)
            {
                MyDebug.LogWarning("craftableItemInfos is null!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                yield break;
            }
            while (CheckAllSlotFull())
            {
                // 제작 가능한 아이템만 필터링
                var craftables = craftableItemInfos
                    .Where(itemInfo =>
                    {
                        var recipe = CraftManager.Instance.GetRecipe(itemInfo.itemData.itemName);
                        GUIItemSlotStation[] stationSlots = new []{ fuelSlot, metalSlot};
                        return CraftManager.Instance.CanCraftFromSlots(stationSlots, recipe.ingredients);
                    })
                    .ToList();

                if (craftables.Count == 0)
                    break; // 더 이상 제작 가능한 아이템 없음 → 루프 종료

                foreach (var itemInfo in craftables)
                {
                    if (curGaugeCoroutine == null)
                    {
                        bool craftingSuccess = false;
                        
                        curGaugeCoroutine = StartCoroutine
                        (
                            UpdateGaugeGUI
                            (
                                itemInfo.craftTime,
                                success => { craftingSuccess = success; }
                            )
                        );
                        
                        // 게이지 끝날 때까지 대기
                        yield return curGaugeCoroutine;
                        
                        if (craftingSuccess)
                        {
                            slotStationDict[ResourceType.None].Show(itemInfo.itemData.icon, 1, itemInfo.itemData);
                            
                            foreach (var slotKvp in slotStationDict)
                            {
                                if (slotKvp.Key == ResourceType.None)
                                    break;

                                slotKvp.Value.SetPcs(-1);
                            }
                        }
                        else
                        {
                            MyDebug.Log("Crafting Failed! Slot is not full...");
                            yield break;
                        }
                    }
                }

                yield return null; // 다음 프레임까지 대기
            }
        }
        finally
        {
            curUpdateUICoroutine = null;
        }
    }
    
    // 제작 시간 동안 게이지 업데이트
    IEnumerator UpdateGaugeGUI(float maxTime, Action<bool> onComplete)
    {
        try
        {
            rectTrGauge.gameObject.SetActive(true);
            float elapsedTime = 0;
            bool isCraftingSuccessful = false;
        
            while (elapsedTime <= maxTime && CheckAllSlotFull())
            {
                float timeRatio = elapsedTime / maxTime;
                rectTrGauge.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, timeRatio * maxWidthGauge);
            
                yield return new WaitForSeconds(0.5f);
                elapsedTime += 0.5f;
            }
            
            isCraftingSuccessful = elapsedTime >= maxTime;
            onComplete?.Invoke(isCraftingSuccessful);
        }
        finally
        {
            curGaugeCoroutine = null;
            rectTrGauge.gameObject.SetActive(false);
        }
    }
    
#if UNITY_EDITOR
    public float CanvasAlpha() => canvasGroup.alpha;
#endif
}
