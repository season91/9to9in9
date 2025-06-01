using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class UICampfirePopup : MonoBehaviour, IGUI
{
    [SerializeField] private RectTransform rectTransf;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GUIItemSlotStation[] materialSlots;
    [SerializeField] private GUIItemSlotStation resultSlot;
    [SerializeField] private RectTransform rectTrGauge;
    [SerializeField] private float maxWidthGauge;

    private List<CraftableItemInfo> craftableItemInfos;
    
    Coroutine curGaugeCoroutine;
    Coroutine curUpdateUICoroutine;
    
    public GameObject GUIObject => gameObject;

    private void Reset()
    {
        rectTransf = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        materialSlots = transform.Find("Layout_ItemSlots")?.GetComponentsInChildren<GUIItemSlotStation>();
        resultSlot = transform.Find("GUI_ItemSlot (Result)")?.GetComponent<GUIItemSlotStation>();
        rectTrGauge = transform.Find("Group_Gauge/Img_Gauge")?.GetComponent<RectTransform>();
        maxWidthGauge = transform.Find("Group_Gauge/Img_GaugeBG").GetComponent<RectTransform>().sizeDelta.x;
    }

    public void Initialization()
    {
        Close();
        
        for (int i = 0; i < materialSlots.Length; i++)
        {
            materialSlots[i].Initialization();
            materialSlots[i].SetTitle(""); // 타이틀이 없음
        }
        
        resultSlot.Initialization();
        resultSlot.SetTitle("Result");
        
        rectTrGauge.gameObject.SetActive(false);
    }
    
    public void Open()
    {
        // key: 중분류 ("Tool", "Armor", "Weapon", 그외 "Default")
        // value: 중분류에 따른 레시피 결과 아이템 리스트
        if (craftableItemInfos == null)
        {
            craftableItemInfos = new List<CraftableItemInfo>();
            craftableItemInfos = CraftManager.Instance.GetRecipeOfStationType(StationType.Campfire)["Default"]; // static class에 넣으셈 나중에
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
    
    bool CheckAllSlotIsEmpty() => materialSlots.All(slot => slot.IsEmpty);
    
    public bool TryPlaceItem(ItemData item) 
    {
        ConsumableItemData consumableItem = item as ConsumableItemData;

        if (consumableItem == null)
        {
            MyDebug.Log("Casting Failed! Is not Consumable!");
            return false;
        }
        
        bool placed = false;

        // 1. 동일한 아이템이 있는 슬롯에 개수만 증가
        foreach (var slot in materialSlots)
        {
            if (!slot.IsEmpty && slot.IsPlacePossible(consumableItem.icon))
            {
                slot.SetPcs(1);
                placed = true;
                break;
            }
        }

        // 2. 동일한 게 없고, 빈 슬롯이 있다면 거기에 배치
        if (!placed)
        {
            foreach (var slot in materialSlots)
            {
                if (slot.IsEmpty)
                {
                    slot.Show(item.icon, 1, item);
                    placed = true;
                    break;
                }
            }
        }

        // 3. 성공했을 경우에만 코루틴 실행
        if (placed && curUpdateUICoroutine == null)
        {
            curUpdateUICoroutine = StartCoroutine(UpdateUIWhileCraftable());
        }

        return placed;
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
            while (!CheckAllSlotIsEmpty())
            {
                // 제작 가능한 아이템만 필터링
                var craftables = craftableItemInfos
                    .Where(itemInfo => 
                    {
                        var recipe = CraftManager.Instance.GetRecipe(itemInfo.itemData.itemName);
                        return CraftManager.Instance.CanCraftFromSlots(materialSlots, recipe.ingredients);
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
                            resultSlot.Show(itemInfo.itemData.icon, 1, itemInfo.itemData);

                            SerializableRecipe recipe = CraftManager.Instance.GetRecipe(itemInfo.itemData.itemName);

                            foreach (var ingredient in recipe.ingredients)
                            {
                                foreach (var slot in materialSlots)
                                {
                                    slot.DiscountPcsOfName(ingredient.itemName ,ingredient.amount);
                                }
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

            while (elapsedTime <= maxTime && !CheckAllSlotIsEmpty())
            {
                // 재료가 충분한지 매 프레임 확인
                bool enoughMaterials = craftableItemInfos != null && craftableItemInfos.Any(itemInfo =>
                {
                    var recipe = CraftManager.Instance.GetRecipe(itemInfo.itemData.itemName);
                    return CraftManager.Instance.CanCraftFromSlots(materialSlots, recipe.ingredients);
                });

                if (!enoughMaterials)
                {
                    // 재료 부족 시 즉시 실패 처리
                    MyDebug.Log("재료 부족으로 제작 중단");
                    onComplete?.Invoke(false);
                    yield break;
                }

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
