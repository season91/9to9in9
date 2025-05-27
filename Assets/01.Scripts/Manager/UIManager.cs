using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Start,
    Game,
    Option,
}

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
                return null;
            return instance;
        }
    }
    
    private readonly Dictionary<string, IGUI> activeGUIs = new();

    UICanvasGameScene canvasGameScene;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    
    public async Task OpenScene(SceneType type)
    {
        // AsyncOperation 타입을 Task로 반환해줌으로 await 해주기
        await SceneManager.LoadSceneAsync(StringScene.LoadingScene).ToTask();
        
        string[] addresses;
        
        switch (type)
        {
            case SceneType.Game:
                addresses = new[] { StringAddressable.Environment, StringAddressable.GameScene };
                await LoadUI(addresses);
                
                await SceneManager.LoadSceneAsync(StringScene.GameScene).ToTask();
                break;
            case SceneType.Start:
            case SceneType.Option:
            default:
                Debug.Log("Implementation in progress.");
                break;
        }
    }
    
    async Task LoadUI(string[] addresses)
    {
        foreach (var address in addresses)
        {
            // 로드 하려는 Canvas의 프리펩 주소를 통해 비동기로 프리팹 생성
            var handle = Addressables.InstantiateAsync(address, transform);
            // 생성이 완료될 때까지 대기
            await handle.Task;
        
            // 생성 완료 시,
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 해당 Addressable의 IGUI를 상속 받는 Canvas 혹은 Popup을 초기화 시켜 줌.
                var obj = handle.Result;
                var gui = obj.GetComponent<IGUI>();

                if (gui == null)
                {
                    Debug.LogError($"IGUI not found on {address}");
                    return;
                }

                activeGUIs[address] = gui;
                gui.Initialization();
            }
        }
    }

    // Scene 전환 시, 모든 UI 제거       // 이후 모든 씬 공통 UI 있으면 그건 제거X
    public void UnloadAllCanvases()
    {
        foreach (var kvp in activeGUIs)
        {
            Addressables.ReleaseInstance(kvp.Value.UIObject);
        }

        activeGUIs.Clear();
    }
    
    
    // Test
    // public void Awake()
    // {
    //     Addressables.InstantiateAsync(StringAddressable.Environment, transform)
    //         .Completed += OnPopupLoaded;
    // }
    
    // private void OnPopupLoaded(AsyncOperationHandle<GameObject> handle)
    // {
    //     if (handle.Status == AsyncOperationStatus.Succeeded)
    //     {
    //         Debug.Log("Popup loaded");
    //     }
    //     else
    //     {
    //         Debug.LogError("Failed to load popup");
    //     }
    // }
}