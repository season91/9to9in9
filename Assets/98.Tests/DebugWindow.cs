using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class MyDebug
{
#if UNITY_EDITOR
    public static void Log(string msg) => Debug.Log(msg);
    public static void LogError(string msg) => Debug.LogError(msg);
    public static void LogWarning(string msg) => Debug.LogWarning(msg);
#endif
}

public class DebugWindow : EditorWindow  
{
    private string itemTypeName;
    
    [MenuItem("Window/My Debug Tool")]
    public static void ShowWindow()    
    {
        GetWindow<DebugWindow>("Debug Tool");
    }

    void OnGUI()
    {
        if (GUILayout.Button("AddItemsToInventory"))
        {
            // 아이템 19개 추가 => Consumable, Equipable, Resource
            List<string> addressList = new List<string>();

            FieldInfo[] fields;
            
            // Field의 public static으로 선언된 변수들을 가져옴
            fields = typeof(StringAdrItemDataConsumable)
                .GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                // IsLiteral => const로 선언된지 확인
                // FieldType == typeof(string) => 자료형이 string인 필드인지 확인
                if (field.IsLiteral && field.FieldType == typeof(string))
                {
                    // 해당 const string 필드의 값을 가져옴
                    // const를 가져오기 위해 GetRawConstantValue를 사용해야함
                    string value = field.GetRawConstantValue() as string;
                    addressList.Add(value);
                }
            }
            
            fields = typeof(StringAdrItemDataEquipable)
                .GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                if (field.IsLiteral && field.FieldType == typeof(string))
                {
                    string value = field.GetRawConstantValue() as string;
                    addressList.Add(value);
                }
            }
            
            fields = typeof(StringAdrItemDataResource)
                .GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                if (field.IsLiteral && field.FieldType == typeof(string))
                {
                    string value = field.GetRawConstantValue() as string;
                    addressList.Add(value);
                }
            }

            foreach (var address in addressList)
            {
                var item = ResourceManager.Instance.GetResource<ItemData>(address);
                CharacterManager.Player.inventoryController.AddItem(item);
            }
            
        }
        
        if (GUILayout.Button("AddResourceItemToInventory"))
        {
            LoadItemAndAddToInventory(StringAdrItemDataResource.Stone);
            LoadItemAndAddToInventory(StringAdrItemDataResource.Wood);
            LoadItemAndAddToInventory(StringAdrItemDataResource.Iron);
        }
        
        if (GUILayout.Button("AddEquipItemToInventory"))
        {
            List<string> addressList = new List<string>();
            
            FieldInfo[] fields = typeof(StringAdrItemDataEquipable)
                .GetFields(BindingFlags.Public | BindingFlags.Static);
            
            foreach (FieldInfo field in fields)
            {
                if (field.IsLiteral && field.FieldType == typeof(string))
                {
                    string value = field.GetRawConstantValue() as string;
                    addressList.Add(value);
                }
            }
            
            foreach (var address in addressList)
            {
                var item = ResourceManager.Instance.GetResource<ItemData>(address);
                CharacterManager.Player.inventoryController.AddItem(item);
            }
        }
        
        if (GUILayout.Button("AddConsumableItemToInventory"))
        {
            List<string> addressList = new List<string>();
            
            FieldInfo[] fields = typeof(StringAdrItemDataConsumable)
                .GetFields(BindingFlags.Public | BindingFlags.Static);
            
            foreach (FieldInfo field in fields)
            {
                if (field.IsLiteral && field.FieldType == typeof(string))
                {
                    string value = field.GetRawConstantValue() as string;
                    addressList.Add(value);
                }
            }
            
            foreach (var address in addressList)
            {
                var item = ResourceManager.Instance.GetResource<ItemData>(address);
                CharacterManager.Player.inventoryController.AddItem(item);
            }
        }
        
        if (GUILayout.Button("CraftAnvilTest"))
        {
            CraftManager.Instance.GetRecipeOfStationType(StationType.Anvil);
        }

        if (GUILayout.Button("AllStatUp"))
        {
            CharacterManager.Player.statHandler.Modify(StatType.Health, 10);
            CharacterManager.Player.statHandler.Modify(StatType.Hunger, 10);
            CharacterManager.Player.statHandler.Modify(StatType.Stamina,10);
        }
        if (GUILayout.Button("AllStatDown"))
        {
            CharacterManager.Player.statHandler.Modify(StatType.Health, -10);
            CharacterManager.Player.statHandler.Modify(StatType.Hunger, -10);
            CharacterManager.Player.statHandler.Modify(StatType.Stamina, -10);
        }

        if (GUILayout.Button("StatUITest"))
        {
            UIManager.Instance.UpdateStatUI(StatType.Health);
            UIManager.Instance.UpdateStatUI(StatType.Hunger);
            UIManager.Instance.UpdateStatUI(StatType.Stamina);
        }
    }
    
    // OnGUI는 async를 못붙이는 함수여서 await 사용 불가능
    // async void로 해야 정상 동작 - 버튼 누를 때 잠깐 사용하는 예외 상황으로 async void 허용
    private async void LoadItemAndAddToInventory(string address)
    {
        try
        {
            AsyncOperationHandle<ItemData> handle = Addressables.LoadAssetAsync<ItemData>(address);
            ItemData item = await handle.Task;

            if (item != null)
            {
                CharacterManager.Player.inventoryController.AddItem(item);
                Debug.Log($"{item.displayName} 인벤토리에 추가됨.");
            }
            else
            {
                Debug.LogError($"[{address}]에서 null 반환됨.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Addressables] 불러오기 안됨 : {ex.Message}");
        }
    }
}
