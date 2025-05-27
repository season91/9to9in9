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
    
    // 여러 개 가지는 것에 대한 표시
    public bool isStackable; // 여러개 가질 수 있는 아이템인지 구분
    public int maxStack; // 얼마나

    //Object 스크립트 어떻게 할지 물어보기
}
