using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Reflection;
using UnityEngine.ResourceManagement.AsyncOperations;


public class ResourceManager
{
    static ResourceManager instance = null;
    
    public static ResourceManager Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new ResourceManager();
            }
            return instance;
        }
    }
    
    private readonly Dictionary<string, Object> resources = new(); // key: resource.name
    private readonly Dictionary<string, List<AsyncOperationHandle>> loadedHandlesByLabel = new(); // key: sceneLabel
    
    #region Lable사용O
    /// <summary>
    /// Addressable 리소스를 Label 기준으로 로드하고 Progress UI 업데이트
    /// </summary>
    public async Task LoadSceneResourcesWithProgress(string sceneLabel, UICanvasLoading canvasLoading)
    {
        var handle = Addressables.LoadAssetsAsync<Object>(sceneLabel, null); // null 콜백: 로드만
        float lastProgress = -1f;

        // UI 초기화
        if (canvasLoading != null)
        {
            canvasLoading.SetProgressTitle(LoadType.Resource);
            canvasLoading.SetProgressStatus("0%");
            canvasLoading.SetProgressBar(0f);
        }

        while (!handle.IsDone)
        {
            float progress = handle.PercentComplete;
            if (canvasLoading != null && Mathf.Abs(progress - lastProgress) > 0.01f)
            {
                lastProgress = progress;
                canvasLoading.SetProgressBar(progress);
                canvasLoading.SetProgressStatus($"{Mathf.FloorToInt(progress * 100f)}%");
            }
            await Task.Yield();
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var resource in handle.Result)
            {
                if (resource != null && !resources.ContainsKey(resource.name))
                    resources[resource.name] = resource;
            }

            // 로드된 핸들 저장
            if (!loadedHandlesByLabel.ContainsKey(sceneLabel))
                loadedHandlesByLabel[sceneLabel] = new List<AsyncOperationHandle>();

            loadedHandlesByLabel[sceneLabel].Add(handle);
        }
        else
        {
            MyDebug.LogError($"[Addressables] Failed to load assets with label: {sceneLabel}");
        }

        // UI 마무리
        if (canvasLoading != null)
        {
            canvasLoading.SetProgressBar(1f);
            canvasLoading.SetProgressStatus("100%");
        }
    }

    /// <summary>
    /// 해당 sceneLabel의 모든 Addressable 리소스 핸들을 해제
    /// </summary>
    public void UnloadSceneResources(string sceneLabel)
    {
        if (!loadedHandlesByLabel.TryGetValue(sceneLabel, out var handleList))
        {
            MyDebug.LogWarning($"[Addressables] No resources to unload for label: {sceneLabel}");
            return;
        }

        foreach (var handle in handleList)
        {
            if (handle.IsValid())
            {
                if (handle.Result is IList<Object> objects)
                {
                    foreach (var obj in objects)
                    {
                        if (obj != null)
                            resources.Remove(obj.name);
                    }
                }

                Addressables.Release(handle);
            }
        }

        loadedHandlesByLabel.Remove(sceneLabel);
        MyDebug.Log($"[Addressables] Unloaded all resources for label: {sceneLabel}");
    }
    #endregion
    
    /// <summary>
    /// 해당 sceneLabel의 모든 Addressable 리소스 핸들을 해제
    /// Resource 불러올 때, ResourceManager.Instance.GetResource<T>(); 해주면 됨!
    /// </summary>
    public T GetResource<T>(string key) where T : Object
    {
        if (!resources.TryGetValue(key, out var obj))
        {
            MyDebug.LogWarning($"this Resource is not exist: Key = {key}");
            return null;
        }

        T objAsT = obj as T;
        if (objAsT == null)
        {
            MyDebug.LogWarning($"this Resource is not this Type: Key = {key}, Type = {typeof(T)}");
            return null;
        }
        return objAsT;
    }
    
    #region Lable사용X
    /*
    public async Task SetAllAddressList()
    {
        List<string> addressList = new List<string>();

        // Field의 public static으로 선언된 변수들을 가져옴
        FieldInfo[] fields = typeof(StringAddressable)
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
        // 여기에 StringAddress추가할 거 있으면 추가할 것 위와 같이!

        await LoadPrefab(addressList);
    }

    async Task LoadPrefab(List<string> addressList)
    {
        // resources = new Dictionary<string, Object>();

        foreach (string path in addressList)
        {
            var handle = Addressables.LoadAssetAsync<Object>(path);
            Object prefab = await handle.Task;

            if (prefab != null)
            {
                resources.Add(path, prefab);
            }
            else
            {
                Debug.LogWarning($"[Addressable] Prefab Load Failed: {path}");
            }
        }
    }
    */
    #endregion
}
