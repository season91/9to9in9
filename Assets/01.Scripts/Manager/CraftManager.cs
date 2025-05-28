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
        string targetRecipeCode = "rfc1";
        
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
    // 조합법 리스트
    // 모루라면 모루에 장비, 툴 등이 있는데 장비레시피리스트, 툴레시피리스트 에서 ui매니저가 받아올때는 sprite, bool 받아갈건데
    // slot 클릭시 craftManager에게 index 넘겨줄거임 (위 레시피리스트의 index)
    // craftItem 메서드에다가 enum으로 어떤 레시피, 스테이션, 장비인지도구인지 타입과 index -> 받으면 해당 장비type 종류의 list에다가 index를 넣어서 만들었다는 걸 알 수 있게
    // json을 조금 더 세분화, 스테이션과 장비종류기준으로 장비에대한 레시피 리스트/ 모루-무기리스트, 모루-툴리스트, 모루-방어구리스트
    // 인벤토리에 추가할 때도 결과와 재료를 아니깐 인벤토리에 넘겨 줄 것 
    
    
    // 모든 레시피의 아이콘과 해당 아이콘을 제작할수있는가?
    // <요청> 파라미터 제작대타입 
    // 따로 인벤토리정보를 가져와서보면되고 
    
    // 모든 레시피 정보에서 요청값 기준으로 가능한지 아닌지에대한 모든 결과를 UI매니저에게 준다
    
    // <결과> uimanager에 전달드려야하는 내용
    // 딕셔너리로 key-icon sprite / value- 제작가능한지에 대한 bool
    // 제작가능하면 icon 활성화
    // 제작 조건 확인만 하는 함수. (레시피, 제작대타입) 
    
    private bool CanCraft(SerializableRecipe recipe)
    {
        // 제작대 type 조건 추가 필요
        
        foreach (var ingredient in recipe.ingredients)
        {
            if (!inventory.ContainsKey(ingredient.itemCode) || inventory[ingredient.itemCode] < ingredient.amount)
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
    // 어떤식으로 inventory에 정보를 전달할건지? 차감할 아이템과 추가할 아이템정보
    // return 재료 배열과 결과물아이템... itemName or itemCode 일부정보만 넘겨주면 itemData에서 찾아서 처리
    private void ExecuteCraft(SerializableRecipe recipe)
    {
        // test code로 인벤토리 생기면 수정
        // 재료 차감 
        foreach (var ingredient in recipe.ingredients)
        {
            inventory[ingredient.itemCode] -= ingredient.amount;
        }
        // 새로운 결과 아이템 지급 처리 필요
    }
}
