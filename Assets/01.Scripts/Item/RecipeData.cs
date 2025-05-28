using System;
using System.Collections.Generic;

// 가공 레시피 목록
[Serializable] // json 직렬화 대상임을 표시
public class SerializableRecipeList
{
    public List<SerializableRecipe> recipes;
}
    
// 가공 레시피 변수. json key가 될 예정
[Serializable]
public class SerializableRecipe
{
    public string recipeCode; 
    public string category; // 가공 레시피 타입 구분값으로 enum 대체 예정.필요없나?
    public string resultItemCode;
    public string resultItemName;
    public int resultAmount;
    public List<SerializableIngredient> ingredients;
    public float craftTime;
    public StationType workstationType; // 제작수단
}

// 가공 레시피 필요한 아이템과 개수
[Serializable]
public class SerializableIngredient
{
    public string itemCode;
    public string itemName;
    public int amount;
}