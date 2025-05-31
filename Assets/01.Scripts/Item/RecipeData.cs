using System;
using System.Collections.Generic;

// 가공 레시피 변수. json key가 될 예정
[Serializable]
public class SerializableRecipe
{
    public string recipeCode;
    public string resultItemType;
    public string resultItemIcon;
    public string resultItemCode; // itemcode이자 addressableName
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