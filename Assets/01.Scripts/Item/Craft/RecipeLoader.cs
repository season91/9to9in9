using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json.Linq;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// [로드] 레시피 json 로드 및 json 파싱
/// </summary>
public class RecipeLoader
{
    private Dictionary<StationType, Dictionary<string, List<SerializableRecipe>>> parsedRecipes;
    
    // json 레시피 data load
    public async Task<Dictionary<StationType, Dictionary<string, List<SerializableRecipe>>>> LoadRecipesAsync()
    {
        try
        {
            // Addressables 로 조회하는 거로 수정 필요
            var handle = Addressables.LoadAssetAsync<TextAsset>(StringAdrCraftRecipe.Recipes);
            TextAsset jsonAsset = await handle.Task;
    
            if (handle.Status != AsyncOperationStatus.Succeeded || jsonAsset == null)
            {
                Debug.LogError($"[Craft] Recipes 로딩 실패: {handle.OperationException?.Message}");
                return null;
            }
            
            JObject recipeJson = JObject.Parse(jsonAsset.text);
            var parsed = ParseRecipeJson(recipeJson);
            Debug.Log($"총 제작소 종류: {parsed.Count}개 레시피 로드");

            return parsed;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Craft] LoadRecipesAsync 예외 발생: {e.Message}\n{e.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Json 레시피 Statsion Type 별로 형태 분리
    /// </summary>
    private Dictionary<StationType, Dictionary<string, List<SerializableRecipe>>> ParseRecipeJson(JObject recipeJson)
    {
        JArray recipeArray = (JArray) recipeJson["recipes"];
        
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

        return parsedRecipes;
    }
}
