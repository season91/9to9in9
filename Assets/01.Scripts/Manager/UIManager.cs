using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum CanvasType
{
    Start,
    Game,
    
}

public class UIManager : MonoBehaviour
{
    
    // Test
    public void Awake()
    {
        Addressables.InstantiateAsync(StringAddressable.Environment, transform)
            .Completed += OnPopupLoaded;
    }
    
    private void OnPopupLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Popup loaded");
        }
        else
        {
            Debug.LogError("Failed to load popup");
        }
    }

    void OpenCanvas()
    {
        
    }
}