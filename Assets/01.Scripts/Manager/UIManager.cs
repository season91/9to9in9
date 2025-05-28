using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Start,
    Main,
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

    // 고정 UI
    [SerializeField] UICanvasLoading canvasLoading;
    
    // 재할당 필요 UI
    UICanvasMainScene canvasMainScene;

    private void Reset()
    {
        canvasLoading = GetComponentInChildren<UICanvasLoading>();
    }

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
        
        // 게임 최초 시작 시 Start씬의 UI를 등록해주기 위함 (초기화 단계)
        _ = OpenScene(SceneType.Start);
    }
    
    // 비동기 Open Scene
    public async Task OpenScene(SceneType type)
    {
        // 활성화된 모든 UI 제거
        UnloadAllGUIs();
        
        // Loading 준비
        canvasLoading.Initialization();
        canvasLoading.SetProgressTitle(LoadType.Scene);
        
        // Addressable 불러 올 프리팹 주소 저장할 곳
        string[] addresses;
        
        switch (type)
        {
            case SceneType.Start:
                addresses = new[] { StringAddressable.StartScene };
                await LoadGUIWithProgress(addresses);
                break;
            case SceneType.Main:
                addresses = new[] { StringAddressable.MainScene };
                await LoadGUIWithProgress(addresses);
                await LoadSceneWithProgress(StringScene.MainScene);
                canvasMainScene = GetComponentInChildren<UICanvasMainScene>();
                break;
            case SceneType.Option:
            default:
                Debug.Log("Implementation in progress.");
                break;
        }
        
        canvasLoading.SetProgressBar(1f);
        canvasLoading.gameObject.SetActive(false);
    }

    // 비동기 Load Scene with Loading Progress
    async Task LoadSceneWithProgress(string sceneName)
    {
        var sceneLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!sceneLoad.isDone)
        {
            canvasLoading.SetProgressBar(sceneLoad.progress);
            await Task.Yield();
        }
    }
    
    // 비동기 Load UI with Loading Progress
    async Task LoadGUIWithProgress(string[] addresses)
    { 
        float currentProgress = 1f;
        canvasLoading.SetProgressTitle(LoadType.GUI);
        
        foreach (var address in addresses)
        {
            canvasLoading.SetProgressBar(0);
            string progressStatus = $"{currentProgress++}/{addresses.Length}";
            canvasLoading.SetProgressStatus(progressStatus);
            
            // 로드 하려는 Canvas의 프리펩 주소를 통해 비동기로 프리팹 생성
            var handle = Addressables.InstantiateAsync(address, transform);
            
            // 생성이 완료될 때까지 대기
            while (!handle.IsDone)
            {
                canvasLoading.SetProgressBar(handle.PercentComplete);
                await Task.Yield(); // == yield return null;
            }

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
    
    // Scene 전환 시, 모든 UI 제거
    // 이후 모든 씬 공통 UI 있으면 그건 제거X
    void UnloadAllGUIs()
    {
        if(activeGUIs.Count <= 0)
            return;
        
        foreach (var kvp in activeGUIs)
        {
            Addressables.ReleaseInstance(kvp.Value.GUIObject);
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
    
    // Main Scene
    public StationType CurrentStation() => canvasMainScene.currentStation;

    public bool TrySlotClickWithStation(ItemData item) => canvasMainScene.TrySlotClickWithStation(item);

    public void OpenStation(StationType type) => canvasMainScene.OpenStation(type);
}