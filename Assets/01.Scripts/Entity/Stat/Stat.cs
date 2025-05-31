using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float baseValue; // 기본 스탯
    [SerializeField] private float bonusValue; // 추가 스탯
    [SerializeField] private float maxValue;
    [SerializeField] private float passiveValue;

    public float CurValue => Mathf.Clamp(baseValue + bonusValue, 0f, maxValue);
    public float MaxValue => maxValue;
    public float PassiveValue => passiveValue;
    public bool IsEmpty => CurValue <= 0f;

    public void Init(Stat baseStat)
    {
        baseValue = baseStat.baseValue;
        bonusValue = 0f;
        maxValue = baseStat.maxValue;
        passiveValue = baseStat.passiveValue;
    }

    public void Change(float amount)
    {
        baseValue = Mathf.Clamp(baseValue + amount, 0f, maxValue);
    }

    public void ChangeBonus(float amount)
    {
        bonusValue = amount < 0f ? 0f : amount;
    }

    public float GetPercentage()
    {
        return CurValue / maxValue;
    }
}