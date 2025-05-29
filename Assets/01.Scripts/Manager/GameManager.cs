using UnityEngine;
/// <summary>
/// [전역] 게임 진행
/// GameStart, GameOver
/// </summary>
public static class GameManager
{
    // test code
    public static void InitMainScene()
    {
        Debug.Log("MainScene 초기화 시작");
        ResourceSpawn();
    }

    public static void ResourceSpawn()
    {
        if (SpawnManager.Instance == null)
        {
            Debug.LogWarning("SpawnManager 인스턴스가 null입니다!");
            return;
        }

        SpawnManager.Instance.GetObject(StringItemName.Tree);
    }

}
