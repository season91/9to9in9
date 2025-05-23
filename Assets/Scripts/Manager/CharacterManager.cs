using UnityEngine;
/// <summary>
/// [전역] 캐릭터 등록, 캐릭터 조회
/// </summary>
public static class CharacterManager
{
    // 캐릭터 등록 전용 클래스
    // 호출 하는 곳에서 초기화 
    public static Player Player { get; private set; }
    public static void Register(Player player) => Player = player;
}
