using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    UICanvasGameScene canvasGameScene;
    [SerializeField] private GameObject canvasLoading;
    [SerializeField] private Image imgLoadingProgress;
    [SerializeField] private TextMeshProUGUI txtLoadingProgress;
    [SerializeField] private TextMeshProUGUI txtLoaingProgressTitle;
    private float loadingProgressMax = 850f;
    

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
        
        _ = OpenScene(SceneType.Start);
    }
    
    public async Task OpenScene(SceneType type)
    {
        // AsyncOperation 타입을 Task로 반환해줌으로 await 해주기
        // await SceneManager.LoadSceneAsync(StringScene.LoadingScene).ToTask();
        
        // 활성화된 캔버스들 제거
        UnloadAllCanvases();
        // 0으로 초기화
        canvasLoading.SetActive(true);
        SetLoadingProgress(0);
        txtLoaingProgressTitle.text = "세계 속으로 들어가는 중...";
        
        string[] addresses;
        
        switch (type)
        {
            case SceneType.Main:
                addresses = new[] { StringAddressable.MainScene };
                await LoadUIWithProgress(addresses);
                await LoadSceneWithProgress(StringScene.MainScene);
                
                // await LoadUI(addresses); 
                // await SceneManager.LoadSceneAsync(StringScene.GameScene).ToTask();
                break;
            case SceneType.Start:
                addresses = new[] { StringAddressable.StartScene };
                await LoadUIWithProgress(addresses);
                break;
            case SceneType.Option:
            default:
                Debug.Log("Implementation in progress.");
                break;
        }
        
        SetLoadingProgress(1f);
        canvasLoading.SetActive(false);
    }

    async Task LoadSceneWithProgress(string sceneName)
    {
        var sceneLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!sceneLoad.isDone)
        {
            SetLoadingProgress(sceneLoad.progress);
            await Task.Yield();
        }
    }
    
    async Task LoadUIWithProgress(string[] addresses)
    { 
        float currentProgress = 1f;
        txtLoaingProgressTitle.text = "세계 구축 중...";
        
        foreach (var address in addresses)
        {
            SetLoadingProgress(0);
            txtLoadingProgress.text = $"{currentProgress++}/{addresses.Length}";
            
            // 로드 하려는 Canvas의 프리펩 주소를 통해 비동기로 프리팹 생성
            var handle = Addressables.InstantiateAsync(address, transform);
            
            // 생성이 완료될 때까지 대기
            while (!handle.IsDone)
            {
                SetLoadingProgress(handle.PercentComplete);
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
    
    // Scene 전환 시, 모든 UI 제거       // 이후 모든 씬 공통 UI 있으면 그건 제거X
    void UnloadAllCanvases()
    {
        if(activeGUIs.Count <= 0)
            return;
        
        foreach (var kvp in activeGUIs)
        {
            Addressables.ReleaseInstance(kvp.Value.UIObject);
        }

        activeGUIs.Clear();
    }
    
    void SetLoadingProgress(float normalizedValue)
    {
        float width = Mathf.Clamp01(normalizedValue) * loadingProgressMax;

        if (imgLoadingProgress != null)
        {
            imgLoadingProgress.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }
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