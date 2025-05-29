using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class DebugWindow : EditorWindow  
{
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
    }
}
