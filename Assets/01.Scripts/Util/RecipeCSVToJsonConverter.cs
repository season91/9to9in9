using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 가공 데이터 json으로 사용하기 위한 csv converter
/// 배열, 중첩 표현 및 동적 조합/필터링을 위해 json으로 사용
/// </summary>
public class RecipeCSVToJsonConverter : EditorWindow
{
    [MenuItem("Tools/Recipe CSV to JSON Convert")]
    public static void Convert()
    {
        string path = EditorUtility.OpenFilePanel("Select CSV", "", "csv");
        if (string.IsNullOrEmpty(path)) return;

        string[] lines = File.ReadAllLines(path);
        if (lines.Length <= 1) return;

        List<SerializableRecipe> recipes = new List<SerializableRecipe>();
        
        for (int i = 1; i < lines.Length; i++) 
        {
            string[] cols = lines[i].Split(',');
            if (cols.Length < 9)
            {
                Debug.LogWarning($"[Line {i+1}] 칼럼 개수 부족: {cols.Length}개 → {lines[i]}");
                continue;
            }

            var recipe = new SerializableRecipe
            {
                recipeCode = cols[0],
                category = cols[1],
                resultItemCode = cols[2],
                resultItemName = cols[3],
                resultAmount = int.Parse(cols[4]),
                ingredients = new List<SerializableIngredient>(),
                craftTime = float.Parse(cols[7]),
                workstationEnumType = cols[8]
            };
            
            string[] ingredientCodes = cols[5].Split('|');
            string[] amountValues = cols[6].Split('|');

            for (int j = 0; j < ingredientCodes.Length; j++)
            {
                if (string.IsNullOrWhiteSpace(ingredientCodes[j])) continue;

                int amount = int.Parse(amountValues[j]);
                SerializableIngredient ingredient = new SerializableIngredient
                {
                    itemCode = ingredientCodes[j],
                    amount = amount
                };
                Debug.Log("ingredientCodes " + ingredientCodes[j] + amount);
                recipe.ingredients.Add(ingredient);
            }
            recipes.Add(recipe);
            
        }

        Debug.Log($"레시피 변환 완료: {recipes.Count}개");
        string json = JsonUtility.ToJson(new SerializableRecipeList { recipes = recipes }, true);
        // string savePath = EditorUtility.SaveFilePanel("JSON 저장", Application.dataPath, "recipes", "json");
        string savePath = Path.Combine(Application.dataPath, "99.Documents/JSON/recipes.json");
        Directory.CreateDirectory(Path.GetDirectoryName(savePath)); // 경로 없으면 생성

        // 덮어쓰기 전에 백업
        if (File.Exists(savePath))
        {
            string backupPath = savePath.Replace(".json", "_backup.json");
            File.Copy(savePath, backupPath, true);
        }
        
        if (!string.IsNullOrEmpty(savePath))
        {
            File.WriteAllText(savePath, json);
            Debug.Log("convert complete! " + savePath);
            AssetDatabase.Refresh();
        }
    }
    [Serializable] //직렬화 대상임을 표시 
    public class SerializableRecipeList
    {
        public List<SerializableRecipe> recipes;
    }
    
    // 가공 레시피 변수. json key가 될 예정
    [Serializable]
    public class SerializableRecipe
    {
        public string recipeCode; 
        public string category; // 가공 레시피 타입 구분값으로 enum 대체 예정
        public string resultItemCode;
        public string resultItemName;
        public int resultAmount;
        public List<SerializableIngredient> ingredients;
        public float craftTime;
        public string workstationEnumType; // 필요한 가공 수단으로 enum 대체 예정
    }

    // 가공 레시피 필요한 아이템과 개수
    [Serializable]
    public class SerializableIngredient
    {
        public string itemCode;
        public int amount;
    }
}
