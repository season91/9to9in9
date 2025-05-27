using System;
using UnityEngine;
/// <summary>
/// [공통스탯값] 모든 Character는 해당 Stat으로 사용. PlayerStat, MonsterStat 등등
/// </summary>

[System.Serializable]
public class Stat
{
    [SerializeField] private float maxValue;
    public float MaxValue => maxValue;

    [SerializeField] private float passiveValue;
    public float PassiveValue => passiveValue;

    [SerializeField] private float curValue;
    public float CurValue
    {
        get => curValue;
        private set => curValue = Mathf.Clamp(value, 0f, maxValue);
    }

    public bool IsEmpty => CurValue <= 0f;

    public void Init(float StartValue, float MaxValue, float PassiveValue)
    {
        maxValue = MaxValue;
        passiveValue = PassiveValue;
        CurValue = StartValue;
    }

    public void Change(float amount)
    {
        CurValue += amount;
    }

    public float GetPercentage()
    {
        return CurValue / maxValue;
    }
}

