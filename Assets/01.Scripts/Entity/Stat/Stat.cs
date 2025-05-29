using System;
using UnityEngine;

/// <summary>
/// [공통스탯값] 런타임에서 변화하는 스탯 데이터
/// </summary>
[System.Serializable]
public class Stat
{
    [SerializeField] private float curValue;
    public float CurValue
    {
        get => curValue;
        private set => curValue = Mathf.Clamp(value, 0f, maxValue);
    }
    
    [SerializeField] private float maxValue;
    public float MaxValue => maxValue;

    [SerializeField] private float passiveValue;
    public float PassiveValue => passiveValue;

    public bool IsEmpty => CurValue <= 0f;

    public void Init(Stat baseStat)
    {
        maxValue = baseStat.maxValue;
        passiveValue = baseStat.passiveValue;
        curValue = baseStat.curValue;
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