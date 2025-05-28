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
        GameObject obj;
        int i;
        for (i = 0; i < defaultCapacity; ++i)
        {
            obj = this.Get();
        }
    }

    public GameObject Get()
    {
        return pool.Get();
    }
    
    public void Release(GameObject obj){
        obj.gameObject.SetActive(false);
        pool.Release(obj);
    }
}