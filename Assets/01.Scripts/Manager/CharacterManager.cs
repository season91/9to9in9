using UnityEngine;
/// <summary>
/// [전역] 캐릭터 등록, 캐릭터 조회, Player를 저장해서 다른곳에서 쓸 수 있게
/// </summary>
public static class CharacterManager
{
    // 호출 하는 곳에서 초기화 
    public static Player Player { get; private set; }
    public static void Register(Player player) => Player = player;
}
