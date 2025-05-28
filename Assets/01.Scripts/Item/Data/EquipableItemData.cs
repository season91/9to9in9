using UnityEngine;

// 장착 타입
public enum EquipType
{
    Weapon,
    Armor,
    GatheringTool
}
// 장착 부위
public enum EquipSlot
{
    Head,       // 머리
    Chest,      // 상의
    Legs,       // 하의
    LeftHand,   // 왼손
    RightHand,  // 오른손
    Feet        // 신발
}
public class EquipableItemData : ItemData
{
    public EquipSlot equipSlot;
    public EquipType equipType;
    public int power;
}