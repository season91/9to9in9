using UnityEngine;

// 소비 타입
public enum EquipableType
{
    Weapon,
    Armor,
    GatheringTool
}
public class EquipableItemData : ItemData
{
    // Test. 사용하시는 분 바꿔서 쓰셔요
    public EquipableType equipableType;
    public int power;
}