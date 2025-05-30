using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
/// PoolManager를 Get. Pool을 활용해 Object 생성 처리 담당
/// </summary>

public class 
    SpawnManager : MonoBehaviour
{
    private static SpawnManager instance;

    private Dictionary<string, PoolManager> pools = new Dictionary<string, PoolManager>();
    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    
    // 자원, 몬스터 랜덤 생성을 정보
    private Transform  spawnCenter;
    private float spawnRange = 30f;

    public static SpawnManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SpawnManager>();
                {
                    if (instance == null)
                    {
                        GameObject obj = new GameObject("SpawnManager");
                        instance = obj.AddComponent<SpawnManager>();
                        DontDestroyOnLoad(obj);
                    }
                }
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        //코드 생성으로도 가능하고, 배치해도 문제 없도록 - 싱글톤이 유지되도록
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    //-------SpawnManager 수정 필요 : 오브젝트 호출 시 받아올 경로나 어드레서블 적용이 필요해 보임

    public void Init()
    {
        foreach (string key in StringAdrSpawnManagerKey.PoolItem)
        {
            ItemData data = ResourceManager.Instance.GetResource<ItemData>(key);
            if (data != null)
            {
                if (!pools.ContainsKey(key))
                {
                    CreatePool(key, data.prefab);
                }
                else
                {
                    //TODO - 프리팹 재설정해야할 시 필요한 코드
                }
            }
            else
            {
                Debug.Log($"{key} 프리팹 찾을 수 없음!");
            }
        }

        foreach (string key in StringAdrSpawnManagerKey.PrefabItem)
        {
            ItemData data = ResourceManager.Instance.GetResource<ItemData>(key);
            if (data != null)
            {
                prefabs[key] = data.prefab;
            }
            else
            {
                Debug.Log($"{key} 프리팹 찾을 수 없음!");
            }
        }
        
        spawnCenter = FindObjectOfType<Player>().transform;
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


    public void ResourceSpawn(string key)
    {
        Vector3 spawnPos = GetRandomPositionOnNavMesh(spawnCenter.position, 20f);
        GameObject obj = GetObject(key);
        obj.transform.position = spawnPos;
    }
    
    private Vector3 GetRandomPositionOnNavMesh(Vector3 center, float range)
    {
        for (int i = 0; i < 10; i++) // 최대 20번 시도
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            randomPoint.y = 0.0f; // y 바닥 고정 
            
            //  가깝게 생성 가능한 지형 찾기
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
            {
                // y 바닥 고정
                return new Vector3(hit.position.x, 0f, hit.position.z);
            }
            // Debug.LogWarning($"[Spawn] 실패 위치: {randomPoint} - NavMesh 못 찾음");
        }

        return new Vector3(center.x, 0f, center.z);
    }
}
