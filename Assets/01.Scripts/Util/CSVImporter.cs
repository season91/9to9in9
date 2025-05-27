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
        ParseCommonFields(item, cols); // 공통데이터 파싱
        
        // 소비아이템 데이터 파싱
        item.isStackable = bool.Parse(cols[8]);
        item.maxStack = int.Parse(cols[9]);
        item.consumableTypes = ParseEnums<ConsumableType>(cols[10]);
        item.amounts = ParseFloats(cols[11]);
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
        ParseCommonFields(item, cols); // 공통데이터 파싱
        
        // 장착 아이템 데이터 파싱 작성
        return item;
    }
    
    #endregion

    #region ResourceItemData 생성

    [MenuItem("Tools/Import/Resource Items")]
    public static void ImportResourceItems()
    {
        ImportItemData<ResourceItemData>("ResourceItemData", ParseResourceItem);
    }
    
    public static ResourceItemData ParseResourceItem(string[] cols)
    {
        var item = ScriptableObject.CreateInstance<ResourceItemData>();
        ParseCommonFields(item, cols); // 공통데이터 파싱
        
        // 자원아이템 데이터 파싱
        return item;
    }
    
    #endregion
    
    #region BuildItemData 생성

    [MenuItem("Tools/Import/Build Items")]
    public static void ImportBuildItems()
    {
        ImportItemData<BuildItemData>("BuildItemData", ParseBuildItem);
    }
    
    public static BuildItemData ParseBuildItem(string[] cols)
    {
        var item = ScriptableObject.CreateInstance<BuildItemData>();
        ParseCommonFields(item, cols); // 공통데이터 파싱
        
        // 건축아이템 데이터 파싱
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
        
        string targetFolder = $"Assets/Resources/Item/Data/";
        if (!Directory.Exists(targetFolder)) {
            Directory.CreateDirectory(targetFolder);
        }
        
        for (int i = 1; i < lines.Length; i++) {
            string[] cols = lines[i].Split(',');
            T item = parseFunc(cols);
            
            // itemType 기반 경로 분기
            string typeFolder = item.type.ToString(); 
            string assetPath = $"{targetFolder}/{typeFolder}/{item.name}.asset";
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
            Debug.Log($"Imported item: {item.name}");
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
        item.icon = IconParse(cols[5], item.type);
        item.prefab = PrefabParse(cols[6], item.type);
        item.functions = ParseEnums<ItemFunction>(cols[7]);
    }

    private static Sprite IconParse(string iconName, ItemType type)
    {
        string path = $"Item/Icons/{type}/{iconName.Trim()}";
        Sprite icon = Resources.Load<Sprite>(path);
        if (icon == null)
            Debug.LogWarning($"[IconParse] Icon not found: {path}");
        return icon;
    }

    private static GameObject PrefabParse(string prefabName, ItemType type)
    {
        string path = $"Item/Prefabs/{type}/{prefabName.Trim()}";
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
            Debug.LogWarning($"[PrefabParse] Prefab not found: {path}");
        return prefab;
    }

    private static TEnum[] ParseEnums<TEnum>(string value) where TEnum : struct
    {
        Debug.Log(value);
        string[] tokens = value.Split('|');
        // 공백제거한 후 조회
        return tokens.Select(t => t.Trim())
                     .Where(t=> Enum.TryParse<TEnum>(t, out _))
                     .Select(t => Enum.Parse<TEnum>(t))
                     .ToArray();
    }

    private static float[] ParseFloats(string value)
    {
        return value.Split('|').Select(float.Parse).ToArray();
    }
 
    #endregion
}
#endif
