using System.Collections.Generic;
using UnityEngine;

public class StatHandler: MonoBehaviour
{
    private Dictionary<StatType, Stat> stats = new();
    
    [SerializeField] private float baseValue;
    [SerializeField] private float bonusValue;

    /// <summary>
    /// 초기 스탯 데이터로부터 내부 스탯을 생성하여 초기화함!!
    /// ScriptableObject로부터 전달받은 StatProfile을 바탕으로 호출함
    /// </summary>
    /// <param name="baseStats">초기 스탯 값들</param>
    public void Initialize(Dictionary<StatType, Stat> baseStats)
    {
        stats = new Dictionary<StatType, Stat>();
        foreach (var pair in baseStats)
        {
            Stat stat = new Stat();
            stat.Init(pair.Value);
            stats[pair.Key] = stat;
            
            UIManager.Instance.UpdateStatUI(pair.Key);
        }
    }

    /// <summary>
    /// 지정한 스탯 타입의 현재 값을 반환
    /// 해당 스탯이 없으면 0을 반환함
    /// </summary>
    /// <param name="type">스탯 타입</param>
    public float Get(StatType type)
    {
        return stats.TryGetValue(type, out var stat) ? stat.CurValue : 0f;
    }

    /// <summary>
    /// 지정한 스탯 타입의 패시브 값을 반환
    /// 해당 스탯이 없으면 0을 반환함
    /// </summary>
    /// <param name="type">스탯 타입</param>
    public float GetPassive(StatType type)
    {
        return stats.TryGetValue(type, out var stat) ? stat.PassiveValue : 0f;
    }

    /// <summary>
    /// 전체 스탯을 딕셔너리형으로 반환
    /// </summary>
    public Dictionary<string, float> GetStatValues()
    {
        Dictionary<string, float> result = new();

        foreach (var pair in stats)
        {
            result[pair.Key.ToString()] = pair.Value.CurValue;
        }

        return result;
    }

    /// <summary>
    /// 지정한 스탯을 변화.
    /// 음수 값은 감소, 양수 값은 증가
    /// </summary>
    /// <param name="type">스탯 타입</param>
    /// <param name="amount">변경할 수치</param>
    public void Modify(StatType type, float amount)
    {
        if (stats.TryGetValue(type, out var stat))
        {
            stat.Change(amount);
        }
    }
    
    /// <summary>
    /// 지정한 스탯의 추가값을 변화.
    /// 양수 값만 사용
    /// </summary>
    /// <param name="type">스탯 타입</param>
    /// <param name="amount">변경할 수치</param>
    public void ModifyBonus(StatType type, float amount)
    {
        if (stats.TryGetValue(type, out var stat))
            stat.ChangeBonus(amount);
    }
    
    /// <summary>
    /// 지정한 스탯의 현재 값이 0 이하인지 확인
    /// </summary>
    /// <param name="type">스탯 타입</param>
    public bool IsEmpty(StatType type)
    {
        return stats.TryGetValue(type, out var stat) && stat.IsEmpty;
    }
        

    /// <summary>
    /// 지정한 스탯의 현재 비율 (0~1)을 반환
    /// UI에서 사용
    /// </summary>
    /// <param name="type">스탯 타입</param>
    public float GetPercentage(StatType type)
    {
        return stats.TryGetValue(type, out var stat) ? stat.GetPercentage() : 0f;
    }
}
