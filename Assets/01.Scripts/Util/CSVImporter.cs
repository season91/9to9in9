using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
/// <summary>
/// [공통] ItemData csv import. 에디터 에서 scriptable화 하는 것
/// 타입별로 나눈 이유는 유지보수 및 타입별로 유연하게 확장하기 위함
/// </summary>
public class CSVImporter
{
    #region ConsumableItemData 생성

    [MenuItem("Tools/Import/Consumable Items")]
    public static void ImportConsumableItems()
    {
        ImportItemData<ConsumableItemData>("ConsumableItemData", ParseConsumableItem);
    }
    
    public static ConsumableItemData ParseConsumableItem(string[] cols)
    {
        var item = ScriptableObject.CreateInstance<ConsumableItemData>();
        ParseCommonFields(item, cols);
        
        item.isStackable = bool.Parse(cols[7]);
        item.maxStack = int.Parse(cols[8]);
        item.consumableTypes = ParseEnums<ConsumableType>(cols[9]);
        item.amounts = ParseFloats(cols[10]);
        return item;
    }
    
    #endregion
    
    #region EquipableItemData 생성
    [MenuItem("Tools/Import/Equipable Items")]
    public static void ImportEquipableItems()
    {
        ImportItemData<EquipableItemData>("EquipableItemData", ParseEquipableItem);
    }
    
    public static EquipableItemData ParseEquipableItem(string[] cols)
    {
        var item = ScriptableObject.CreateInstance<EquipableItemData>();
        ParseCommonFields(item, cols);
        
        return item;
    }

    #endregion

    #region 공통 Parse
    public static void ImportItemData<T>(string csvName, Func<string[], T> parseFunc) where T : ItemData
    {
        string path = EditorUtility.OpenFilePanel($"Select {csvName} CSV", "", "csv");
        if (string.IsNullOrEmpty(path)) return;

        string[] lines = File.ReadAllLines(path);
        if (lines.Length <= 1) {
            Debug.LogWarning("CSV has no data.");
            return;
        }
        
        // string targetFolder = $"Assets/Resources/Item/{typeof(T).Name}";
        string targetFolder = $"Assets/Resources/Item/Data/";
        if (!Directory.Exists(targetFolder)) {
            Directory.CreateDirectory(targetFolder);
        }
        
        for (int i = 1; i < lines.Length; i++) {
            string[] cols = lines[i].Split(',');
            T item = parseFunc(cols);

            string assetPath = $"{targetFolder}/{item.name}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (existing != null)
            {
                EditorUtility.CopySerialized(item, existing);
                EditorUtility.SetDirty(existing);
            }
            else
            {
                AssetDatabase.CreateAsset(item, assetPath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"{csvName} CSV import completed.");
    }

    private static void ParseCommonFields(ItemData item, string[] cols)
    {
        item.name = cols[1]; // 파일명
        item.itemCode = cols[0];
        item.displayName = cols[2];
        item.description = cols[3];
        item.type = (ItemType)Enum.Parse(typeof(ItemType), cols[4]);
        item.icon = IconParse(cols[5]);
        item.prefab = PrefabParse(cols[6]);
    }
    
    private static Sprite IconParse(string iconName) => Resources.Load<Sprite>($"Item/Icons/{iconName}");

    private static GameObject PrefabParse(string prefabName) => Resources.Load<GameObject>($"Item/Prefabs/{prefabName}");

    private static TEnum[] ParseEnums<TEnum>(string value) where TEnum : struct
    {
        string[] tokens = value.Split('|');
        return tokens.Select(t => (TEnum)Enum.Parse(typeof(TEnum), t)).ToArray();
    }

    private static float[] ParseFloats(string value)
    {
        return value.Split('|').Select(float.Parse).ToArray();
    }
    #endregion
}
#endif
