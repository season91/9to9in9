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
        if (GUILayout.Button("AddItemToInventory"))
        {
            // ResourceManager.Instance.GetResource<ItemData>()
            // CharacterManager.Player.inventoryController.AddItem();
            // CharacterManager.Player.inventoryController.AddItem();
        }
    }
}
