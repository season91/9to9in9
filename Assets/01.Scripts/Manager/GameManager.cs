using System;
using UnityEngine;
/// <summary>
/// [전역] 게임 진행
/// GameStart, GameOver
/// </summary>
public class GameManager : MonoBehaviour
{
    
    [SerializeField] private GameObject fireflyPrefab;
    private GameObject spawnedFirefly;
    public static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                return null;
            return instance;
        }
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnDestroy()
    {
        DayNightCycle.OnDayStarted -= ResourceSpawn;
        DayNightCycle.OnNightStarted -= EnemySpawn;
    }

    public void InitMainScene()
    {
        Debug.Log("MainScene 초기화 시작");
        ResourceSpawn(); // 처음만
        
        DayNightCycle.OnDayStarted += ResourceSpawn;
        DayNightCycle.OnDayStarted += DespawnFirefly;
        
        DayNightCycle.OnNightStarted += EnemySpawn;
        DayNightCycle.OnNightStarted += SpawnFirefly;
    }

    public void ResourceSpawn()
    {
        if (SpawnManager.Instance == null)
        {
            Debug.LogWarning("SpawnManager 인스턴스가 null입니다!");
            return;
        }
        
        foreach (var pair in StringSpawnDic.resourceSpawnTable)
        {
            for (int i = 0; i < pair.Value; i++)
            {
                SpawnManager.Instance.ResourceSpawn(pair.Key);
            }
        }
        
        Debug.Log("리소스 소환");
    }

    public void EnemySpawn()
    {
        foreach (var pair in StringSpawnDic.enemyResourceSpawnTable)
        {
            for (int i = 0; i < pair.Value; i++)
            {
                SpawnManager.Instance.ResourceSpawn(pair.Key);
            }
        }
        
        Debug.Log("적 소환");
    }
    
    private void SpawnFirefly()
    {
        Vector3 fireflySpawnPosition = new Vector3(0, 2, 0);
        if (spawnedFirefly == null)
        {
            spawnedFirefly = Instantiate(fireflyPrefab, fireflySpawnPosition, Quaternion.identity);
        }
        else
        {
            spawnedFirefly.SetActive(true);
        }
    }

    private void DespawnFirefly()
    {
        if (spawnedFirefly != null)
        {
            spawnedFirefly.SetActive(false);
        }
    }

    public void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        UIManager.Instance.GameOver();
    }
    
}
