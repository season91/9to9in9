using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject 형태의 스탯 프로파일
/// (플레이어/적/직업별 설정값 저장용)
/// </summary>

public enum StatType
{
    Health, // 체력
    Hunger, // 배고픔
    Stamina, // 스태미나
    MoveSpeed, // 걸음 속도
    JumpPower, // 점프 높이
    AttackPower, // 공격력
    AttackSpeed, // 공격 주기
    CriticalChance, // 치명타 확률
    DefensePower, // 방어력
}

[CreateAssetMenu(menuName = "Stats/StatProfile")]
public class StatProfile : ScriptableObject
{
    [System.Serializable]
    public class StatEntry
    {
        public StatType statType;
        public Stat stat; // 초기값, 최대값, 패시브 회복량 등 포함
    }

    [SerializeField]
    private List<StatEntry> statEntries;
    
    /// <summary>
    /// 런타임 Dictionary 형태로 변환
    /// </summary>
    public Dictionary<StatType, Stat> ToDictionary()
    {
        var dict = new Dictionary<StatType, Stat>();
        foreach (var entry in statEntries)
        {
            dict[entry.statType] = entry.stat;
        }

        return dict;
    }
}
