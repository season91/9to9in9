using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public SerializableRecipeList jsonRecipes;
    
    // inventory test code
    Dictionary<string, int> inventory = new Dictionary<string, int>
    {
        { "whole-ham", 1 },
        { "apple", 1 },
        { "wood", 5 },
    };

    private void Awake()
    {
        ReloadRecipes();
    }

    private void Update()
    {
        // 실시간 테스트용: R 키로 레시피 재로드
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("L key pressed");
            ReloadRecipes();
        }
    }

    private void Start()
    {
        // test code
        string targetRecipeCode = "rf1";

        SerializableRecipe recipe = jsonRecipes.recipes.FirstOrDefault(r => r.recipeCode == targetRecipeCode);
        
        if (recipe == null)
        {
            Debug.LogWarning($"레시피 코드 '{targetRecipeCode}'를 찾을 수 없습니다.");
            return;
        }

        TryCraft(recipe);
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
        jsonRecipes = JsonUtility.FromJson<SerializableRecipeList>(jsonAsset.text);
    }
    
    // 제작 조건 확인만 하는 함수
    private bool CanCraft(SerializableRecipe recipe)
    {
        // 제작대 type 조건 추가 필요
        
        foreach (var ingredient in recipe.ingredients)
        {
            // itemName으로 비교할지 itemCode로 비교할지 고민
            if (!inventory.ContainsKey(ingredient.itemName) || inventory[ingredient.itemName] < ingredient.amount)
            {
                return false;
            }
        }
        return true;
    }
    
    // 제작 조건 검사 확인결과에 따라 실행 결정
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
    
    // 제작 실행만 하는 함수
    private void ExecuteCraft(SerializableRecipe recipe)
    {
        // test code로 인벤토리 생기면 수정
        // 재료 차감 
        foreach (var ingredient in recipe.ingredients)
        {
            inventory[ingredient.itemName] -= ingredient.amount;
        }
        // 새로운 결과 아이템 지급 처리 필요
    }
}
