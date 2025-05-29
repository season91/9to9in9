using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/// <summary>
/// PoolManager를 Get. Pool을 활용해 Object 생성 처리 담당
/// </summary>

public class SpawnManager : MonoBehaviour
{
    private static SpawnManager instance;

    private Dictionary<string, PoolManager> pools = new Dictionary<string, PoolManager>();
    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    
    
    public static SpawnManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("SpawnManager").AddComponent<SpawnManager>();
            }
            return instance;
        }
    }
    //-------SpawnManager 수정 필요 : 오브젝트 호출 시 받아올 경로나 어드레서블 적용이 필요해 보임
    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        //foreach (string key in StringItemName.oftenUseKeys)
        //{
        ItemData data = ResourceManager.Instance.GetResource<ItemData>(StringAdrItemDataResource.Wood);
        if (data != null)
        {
            CreatePool(StringAdrItemDataResource.Wood,data.prefab);
        }
        else
        {
            Debug.Log($"{StringAdrItemDataResource.Wood} 프리팹 찾을 수 없음!");
        }
        //}

        //foreach (string key in StringItemName.prefabKeys)
        //{
        data = ResourceManager.Instance.GetResource<ItemData>(StringAdrItemDataEquipable.ArmorChest);
        if (data != null)
        {
            prefabs[StringAdrItemDataEquipable.ArmorChest] = data.prefab;
        }
        else
        {
            Debug.Log($"{StringAdrItemDataEquipable.ArmorChest} 프리팹 찾을 수 없음!");
        }
        Debug.Log($"{prefabs[StringAdrItemDataEquipable.ArmorChest].name}");
        //}
    }
    private void CreatePool(string key, GameObject prefab)
    {
        GameObject poolParent = new GameObject($"{key}Pool");
        poolParent.transform.SetParent(transform);
        
        PoolManager pool = new PoolManager(prefab, poolParent.transform);
        pools[key] = pool;
    }

    private PoolManager GetPool(string key)
    {
        return pools[key] as PoolManager;
    }

    // object pool 대상 활성화. 예) 자원인 경우 이름(wood), 프리팹명으로 호출(wood)
    public GameObject GetObject(string key)
    {
        if (pools.ContainsKey(key))
        {
            return GetPool(key).Get();
        }
        else if(prefabs.ContainsKey(key))
        {
            return Instantiate(prefabs[key]);
        }
        
        Debug.Log("프리팹 없음!!! 심각한 버그!!");
        throw new Exception();
        return null;
    }

    // object pool 대상 비활성화
    public void ReleaseObject(string key, GameObject obj)
    {
        if (pools.ContainsKey(key))
        {
            GetPool(key).Release(obj);
        }
        else
        {
            Debug.Log($"SpawnManager.Prefabs에서 {key} 프리팹 찾을 수 없음!");
            Destroy(obj);
        }
    }
    
    // 필드
    // ItemPool<Item>
    // EnemyPool<Enemy>
    
    // class Pool <T>로 사용 
    
    // Item SpawnItem(enum-어떤아이템인지) 초기화와 생성을 여기서할건지? 호출주체가 할건지?
    // SpawnMonster
    // Spawn~~
}
