using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// [요청처리] 레시피 관련된 내용. 제작 가능 여부 요청 받아 조건 검사하여 처리
/// </summary>
public class RecipeHandler
{
    private PlayerInventoryController playerInventory;

    public RecipeHandler(PlayerInventoryController playerInventory)
    {
        this.playerInventory = playerInventory;
    }
    
    /// <summary>
    /// 레시피에 필요한 재료가 모두 있어야 true
    /// 하나라도 없으면 false
    /// </summary>
    public bool CanCraft(List<SerializableIngredient> ingredients)
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
}
