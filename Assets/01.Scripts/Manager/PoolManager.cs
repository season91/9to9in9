using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 오브젝트 Pool Queue, 메모리 할당 해제. 활성화된 것 관리는 다른 곳에서.
/// </summary>
public class PoolManager
{
    private ObjectPool<GameObject> pool;
    private List<GameObject> spawnedObject = new List<GameObject>();
    
    private int defaultCapacity = 50;
    private int maxCapacity = 500;

    public PoolManager(GameObject prefab, Transform parent = null)
    {
        pool = new ObjectPool<GameObject>(
            createFunc: () => UnityEngine.Object.Instantiate(prefab, parent),
            actionOnGet: item => item.gameObject.SetActive(true),
            actionOnRelease: item => item.gameObject.SetActive(false),
            actionOnDestroy: item => UnityEngine.Object.Destroy(item.gameObject),
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxCapacity);

        List<GameObject> temp = new List<GameObject>();

        for (int i = 0; i < defaultCapacity; ++i)
        {
            GameObject obj = this.Get();
            temp.Add(obj);
        }

        foreach (GameObject obj in temp)
        {
            this.Release(obj);
        }
}

    public void Init()
    {
        if (spawnedObject == null)
        {
            spawnedObject = new List<GameObject>();
        }

        else if (spawnedObject.Count > 0)
        {
            foreach (GameObject obj in spawnedObject)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    pool.Release(obj);
                }
            }
            spawnedObject.Clear();
        }
    }
    
    public GameObject Get()
    {
        GameObject obj= pool.Get();
        spawnedObject.Add(obj);
        return obj;
    }
    
    public void Release(GameObject obj)
    {
        spawnedObject.Remove(obj);
        obj.SetActive(false);
        pool.Release(obj);
    }
}