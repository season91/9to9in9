using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

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
        if (GUILayout.Button("AddItemToInventory"))
        {
            // ResourceManager.Instance.GetResource<ItemData>()
            // CharacterManager.Player.inventoryController.AddItem();
            // CharacterManager.Player.inventoryController.AddItem();
        }
        
        if (GUILayout.Button("AddResourceItemToInventory"))
        {
            LoadItemAndAddToInventory(StringAdrItemDataResource.Stone);
            LoadItemAndAddToInventory(StringAdrItemDataResource.Wood);
            LoadItemAndAddToInventory(StringAdrItemDataResource.Iron);
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
