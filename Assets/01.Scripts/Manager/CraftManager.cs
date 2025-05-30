using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 아이템 제작 흐름 조율
/// </summary>
///


public class CraftableItemInfo
{
    public ItemData itemData;
    public float craftTime;
}

public class CraftManager : MonoBehaviour
{
    private static CraftManager instance;
    public static CraftManager Instance
    {
        get
        {
            if (instance == null)
                return null;

            return instance;
        }
    }
    
    private Dictionary<StationType, Dictionary<string, List<SerializableRecipe>>> parsedRecipes;

    private SerializableRecipe currentRecipe;
    
    private PlayerInventoryController playerInventory;
    
    private RecipeHandler recipeHandler;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        playerInventory = CharacterManager.Player.inventoryController;
        recipeHandler = new RecipeHandler(playerInventory);
    }

    private async void Start()
    {
        await ReloadRecipes(); // 완료될 때까지 기다려야함. awake는 안됨

        Debug.Log("레시피 로딩 완료 후 후속 로직 가능");
        
        // test
        // GetRecipeOfStationType(StationType.Anvil);
    }
    
    private async Task ReloadRecipes()
    {
        var recipeLoader = new RecipeLoader();
        parsedRecipes = await recipeLoader.LoadRecipesAsync();
    }

    /// <summary>
    /// StationType에 해당하는 레시피들 목록을 조회해
    /// Player 인벤토리에 있는 아이템 중 위 레시피 중 제작 가능 여부 판단해서 리턴
    /// key: 중분류 이름 ("Tool", "Armor", "Weapon", 그외 "Default")
    /// </summary>
    public Dictionary<string, List<CraftableItemInfo>> GetRecipeOfStationType(StationType stationType)
    {
        if (parsedRecipes == null)
        {
            MyDebug.LogWarning("parsedRecipes is null, is not ReloadRecipes yet...................ㅠ");
            return null;
        }
        if (!parsedRecipes.ContainsKey(stationType))
        {
            Debug.LogWarning($"parsedRecipes에 {stationType} 데이터가 없음");
            return null;
        }

        var itemDataByCategory = new Dictionary<string, List<CraftableItemInfo>>();
        var categoryDict = parsedRecipes[stationType];

        foreach (var categoryPair in categoryDict)
        {
            string category = string.IsNullOrEmpty(categoryPair.Key) ? "Default" : categoryPair.Key;

            if (!itemDataByCategory.ContainsKey(category))
            {
                itemDataByCategory[category] = new List<CraftableItemInfo>();
            }
            foreach (var recipe in categoryPair.Value)
            {
                var itemData = GetItemData(recipe.addressableName);
                if (itemData == null)
                {
                    Debug.LogWarning($"[GetRecipeOfStationType] itemData 없음: {recipe.addressableName}");
                    continue;
                }

                itemData.isCraftable = CanCraft(recipe);

                itemDataByCategory[category].Add(new CraftableItemInfo
                {
                    itemData = itemData,
                    craftTime = recipe.craftTime
                });
            }
        }

        return itemDataByCategory;
    }

    private bool CanCraft(SerializableRecipe recipe) => recipeHandler.CanCraft(recipe.ingredients);
    
    /// <summary>
    /// 제작 조건 검사 확인결과에 따라 실행 결정
    /// </summary>
    private bool TryCraft(SerializableRecipe recipe)
    {
        if (!CanCraft(recipe))
        {
            Debug.Log("재료 부족으로 제작 실패");
            return false;
        }

        // 제작 시간 확인 후 실행
        StartCoroutine(CraftWithDelay(recipe));
        return true;
    }
    
    // 제작시간에 따른 코루틴
    private IEnumerator CraftWithDelay(SerializableRecipe recipe)
    {
        Debug.Log($" 제작 시작: {recipe.resultItemCode} (소요 시간: {recipe.craftTime}초)");
        yield return new WaitForSeconds(recipe.craftTime);

        ExecuteCraft(recipe);
    }
    
    
    // GetRecipe
    /// <summary>
    /// 레시피 정보 기준으로 재료 차감 및 아이템 추가
    /// </summary>
    public void ExecuteCraft(SerializableRecipe recipe)
    {
        // 재료 차감 string, 수량 
        foreach (var ingredient in recipe.ingredients)
        {
            playerInventory.RemoveItem(ingredient.itemName, ingredient.amount);
        }
        
        // 새로운 결과 아이템 지급 처리 필요
        // addressbale name 파싱
        string adrName = StringUtils.KebabToPascal(recipe.addressableName);
        ItemData itemData = GetItemData(adrName);

        if (itemData == null)
        {
            Debug.LogError($"[ExecuteCraft] 결과 아이템 데이터 정보가 없음 : {adrName}");
            return;
        }
        
        playerInventory.AddItem(itemData, recipe.resultAmount);
    }

    public ItemData GetItemData(string addressName)
    {
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            var item = ResourceManager.Instance.GetResource<ItemData>(addressName);
            if (item != null) return item;
        }

        return null;
    }
    
    /// <summary>
    /// 레시피 단건 정보로 isCratable 판단하는 함수
    /// </summary>
    public bool CanCraftByItemName(string itemName)
    {
        // 레시피 정보 조회 -> currentRecipe 에 적재
        if (currentRecipe == null || currentRecipe.resultItemName != itemName)
        {
            currentRecipe = GetRecipe(itemName);
            if (currentRecipe == null)
            {
                Debug.LogWarning($"[CanCraftCachedRecipe] 레시피 없음: {itemName}");
                return false;
            }
        }
        
        // 적재된 레시피 정보로 제작 가능한지 확인
        return CanCraft(currentRecipe);
    }

    /// <summary>
    /// itemName으로 레시피 단건 조회, 레시피 건수가 많아진다면 캐싱 리팩토링 고려
    /// </summary>
    public SerializableRecipe GetRecipe(string itemName)
    {
        foreach (var stationPair in parsedRecipes)
        {
            foreach (var categoryPair in stationPair.Value)
            {
                foreach (var recipe in categoryPair.Value)
                {
                    if (recipe.resultItemName == itemName)
                    {
                        return recipe;
                    }
                }
            }
        }
        
        return null;
    }
    
    public bool CanCraftFromSlots(GUIItemSlotStation[] slots, List<SerializableIngredient> ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            int totalAmount = 0;

            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && slot.ItemData.itemName == ingredient.itemName) 
                {
                    totalAmount += slot.GetPcs();
                }
            }

            if (totalAmount < ingredient.amount)
                return false;
        }

        return true;
    }
    
}
