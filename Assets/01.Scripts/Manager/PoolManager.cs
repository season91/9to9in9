using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 오브젝트 Pool Queue, 메모리 할당 해제. 활성화된 것 관리는 다른 곳에서.
/// </summary>
public class PoolManager<T> where T : Component
{
    private ObjectPool<T> pool;
    
    private int defaultCapacity = 50;
    private int maxCapacity = 500;

    public PoolManager(T prefab, Transform parent = null)
    {
        pool = new ObjectPool<T>(
            createFunc: () => UnityEngine.Object.Instantiate(prefab, parent),
            actionOnGet: item => item.gameObject.SetActive(true),
            actionOnRelease: item => item.gameObject.SetActive(false),
            actionOnDestroy: item => UnityEngine.Object.Destroy(item.gameObject),
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxCapacity);
    }

    public T Get()
    {
        return pool.Get();
    }
    
    public void Release(T obj){
        pool.Release(obj);
    }
}