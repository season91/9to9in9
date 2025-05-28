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

    private Dictionary<string, object> pools = new Dictionary<string, object>();
    
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
        //CreatePool<Item>("Item", Item);
        //CreatePool<Enemy>("Enemy",Enemy);
        var rock = Resources.Load<GameObject>("Item/Prefabs/Resource/stone");
        var wood = Resources.Load<GameObject>("Item/Prefabs/Resource/wood");
        if (rock == null)
        {
            Debug.LogError("rockPrefab is null! 경로 확인 필요.");
        }
        else
        {
            CreatePool("Rock", rock);
        }
        if (wood == null)
        {
            Debug.LogError("woodPrefab is null! 경로 확인 필요.");
        }
        else
        {
            CreatePool("Wood", wood);
        }

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

    public GameObject GetObject(string key, GameObject Prefab)
    {
        return GetPool(key).Get();
    }

    public void ReleaseObject(string key, GameObject obj)
    {
        GetPool(key).Release(obj);
    }
    
    // 필드
    // ItemPool<Item>
    // EnemyPool<Enemy>
    
    // class Pool <T>로 사용 
    
    // Item SpawnItem(enum-어떤아이템인지) 초기화와 생성을 여기서할건지? 호출주체가 할건지?
    // SpawnMonster
    // Spawn~~
}
