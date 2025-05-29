using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
/// <summary>
/// item 조합
/// </summary>
public class CraftManager : MonoBehaviour
{
    // 레시피 공식 데이터 json 으로 선택한 이유
    // 1. 복잡한 공식은 json 작성이 유지보수가 용이
    // 2. 공식 수정 후 동적 재로드 가능 
    
    // json data load
    private Dictionary<StationType, Dictionary<string, List<SerializableRecipe>>> parsedRecipes;

    private PlayerInventoryController playerInventory;

    private void Awake()
    {
        ReloadRecipes();
        playerInventory = CharacterManager.Player.inventoryController;
    }

    private void Start()
    {
        // test
        // GetRecipeOfStationType("Anvil");
    }

    // json 레시피 data load
    private void ReloadRecipes()
    {
        // Addressables 로 조회하는 거로 수정 필요
        TextAsset jsonAsset = Resources.Load<TextAsset>("Item/Crafting/recipes"); // 동기로 가져오기
        if (jsonAsset == null)
        {
            Debug.LogError("recipes.json을 찾을 수 없습니다.");
            return;
        }
        
        Debug.Log("Reloading recipes.json");

        JObject root = JObject.Parse(jsonAsset.text);
        JArray recipeArray = (JArray)root["recipes"];

        parsedRecipes = new Dictionary<StationType, Dictionary<string, List<SerializableRecipe>>>();

        foreach (var stationEntry in recipeArray)
        {
            foreach (var kv in (JObject)stationEntry)
            {
                string stationKey = kv.Key;

                if (!Enum.TryParse<StationType>(stationKey, out StationType stationEnum))
                {
                    Debug.LogWarning($"알 수 없는 StationType: {stationKey}");
                    continue;
                }

                if (kv.Value.Type == JTokenType.Array)
                {
                    // Campfire, Smelter처럼 중분류 없이 바로 list일 경우
                    var recipeList = kv.Value.ToObject<List<SerializableRecipe>>();
                    parsedRecipes[stationEnum] = new Dictionary<string, List<SerializableRecipe>>
                    {
                        { "Default", recipeList }
                    };
                }
                else if (kv.Value.Type == JTokenType.Object)
                {
                    // Anvil처럼 중분류가 있는 구조
                    var categoryObject = (JObject)kv.Value;
                    var categoryDict = new Dictionary<string, List<SerializableRecipe>>();

                    foreach (var cat in categoryObject)
                    {
                        string category = cat.Key;
                        var recipeList = cat.Value.ToObject<List<SerializableRecipe>>();
                        categoryDict[category] = recipeList;
                    }

                    parsedRecipes[stationEnum] = categoryDict;
                }
            }
        }

        Debug.Log($"총 제작소 종류: {parsedRecipes.Count}개 로드됨");
    }

    /// <summary>
    /// StationType에 해당하는 레시피들 목록을 조회해
    /// Player 인벤토리에 있는 아이템 중 위 레시피 중 제작 가능 여부 판단해서 리턴
    /// key: 중분류 이름 ("Tool", "Armor", "Weapon", 그외 "Default")
    /// </summary>
    private Dictionary<string, Dictionary<Sprite, bool>> GetRecipeOfStationType(string stationType)
    {
        if (string.IsNullOrEmpty(stationType)) return null;

        if (!Enum.TryParse(stationType, out StationType stationEnum)) return null;

        if (!parsedRecipes.ContainsKey(stationEnum))
        {
            Debug.LogWarning($"parsedRecipes에 {stationEnum} 데이터가 없음");
            return null;
        }

        var iconCraftableByCategory = new Dictionary<string, Dictionary<Sprite, bool>>();
        var categoryDict = parsedRecipes[stationEnum];

        foreach (var categoryPair in categoryDict)
        {
            string category = string.IsNullOrEmpty(categoryPair.Key) ? "Default" : categoryPair.Key;

            if (!iconCraftableByCategory.ContainsKey(category))
            {
                iconCraftableByCategory[category] = new Dictionary<Sprite, bool>();
            }

            foreach (var recipe in categoryPair.Value)
            {
                Sprite iconSprite = Resources.Load<Sprite>($"Item/Icons/{recipe.resultItemType}/{recipe.resultItemIcon}");
                if (iconSprite == null)
                {
                    Debug.LogWarning($"아이콘 누락 확인필요 : Item/Icons/{recipe.resultItemType}/{recipe.resultItemIcon}");
                    continue;
                }

                bool isCraftable = CanCraft(recipe.ingredients);

                var iconDict = iconCraftableByCategory[category];
                if (!iconDict.ContainsKey(iconSprite))
                {
                    iconDict.Add(iconSprite, isCraftable);
                }
            }
        }

        return iconCraftableByCategory;
    }

    /// <summary>
    /// 레시피에 필요한 재료가 모두 있어야 true
    /// 하나라도 없으면 false
    /// </summary>
    private bool CanCraft(List<SerializableIngredient> ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            var amount = playerInventory.GetPcs(ingredient.itemName);
            if (amount <= 0)
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 제작 조건 검사 확인결과에 따라 실행 결정
    /// </summary>
    /// <param name="recipe"></param>
    /// <returns></returns>
    private bool TryCraft(SerializableRecipe recipe)
    {
        if (!CanCraft(recipe.ingredients))
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
    
    // 제작 실행만 하는 함수
    // 어떤식으로 inventory에 정보를 전달할건지? 차감할 아이템과 추가할 아이템정보
    // return 재료 배열과 결과물아이템... itemName or itemCode 일부정보만 넘겨주면 itemData에서 찾아서 처리
    // 추가 구현 필요
    private void ExecuteCraft(SerializableRecipe recipe)
    {
        // test code로 인벤토리 생기면 수정
        // 재료 차감 string, 수량 
        foreach (var ingredient in recipe.ingredients)
        {
            // inventory[ingredient.itemCode] -= ingredient.amount;
        }
        // 새로운 결과 아이템 지급 처리 필요
        // itemData만들어서 Add는해주구
    }
}
