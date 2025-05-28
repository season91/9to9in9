using UnityEngine;

// 소비 타입
public enum ConsumableType
{
    Health,
    Hunger,
    Stamina
}

public class ConsumableItemData : ItemData
{
    // 소비 타입 아이템인 경우 회복량
    public ConsumableType[] consumableTypes;
    public float[] amounts;
    
}
