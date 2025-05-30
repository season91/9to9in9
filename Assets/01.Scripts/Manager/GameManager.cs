using System;
using UnityEngine;
/// <summary>
/// [전역] 게임 진행
/// GameStart, GameOver
/// </summary>
public class GameManager : MonoBehaviour
{
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


    private void Start()
    {
        
    }

    // test code
    public void InitMainScene()
    {
        Debug.Log("MainScene 초기화 시작");
        ResourceSpawn();
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
    }

}
