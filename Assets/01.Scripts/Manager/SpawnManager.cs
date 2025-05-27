using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

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

    private void Awake()
    {
        //CreatePool<Item>("Item", Item);
        //CreatePool<Enemy>("Enemy",Enemy);
    }

    private void CreatePool<T>(string key, T prefab) where T : Component
    {
        GameObject poolParent = new GameObject($"{key}Pool");
        poolParent.transform.SetParent(transform);
        
        PoolManager<T> pool = new PoolManager<T>(prefab, poolParent.transform);
        pools[key] = pool;
    }

    private PoolManager<T> GetPool<T>(string key) where T : Component
    {
        return pools[key] as PoolManager<T>;
    }

    public T GetObject<T>(string key, T Prefab) where T : Component
    {
        return GetPool<T>(key).Get();
    }

    public void ReleaseObject<T>(string key, T obj) where T : Component
    {
        GetPool<T>(key).Release(obj);
    }
    
    // 필드
    // ItemPool<Item>
    // EnemyPool<Enemy>
    
    // class Pool <T>로 사용 
    
    // Item SpawnItem(enum-어떤아이템인지) 초기화와 생성을 여기서할건지? 호출주체가 할건지?
    // SpawnMonster
    // Spawn~~
}
