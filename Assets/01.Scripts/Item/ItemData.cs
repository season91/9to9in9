using System;
using UnityEngine;

// 아이템 분류
public enum ItemType
{
    Consumable, // 소비 하는
    Equipable, // 장착이 가능한 
    Resource, // 단순 자원
    Build, // 건축
    Station 
}

// 아이템 기능 명시
public enum ItemFunction
{
    Inspectable,
    Interactable,
}
public abstract class ItemData : ScriptableObject
{
    [Header("아이템에 들어갈 정보 Info")]
    public string itemCode;
    public string displayName; // 아이템 이름
    public string description; // 아이템 설명
    public ItemType type; // 회복, 공격, 채집 등으로 인한 각 아이템 타입
    public Sprite icon;
    public GameObject prefab; // 프리팹 정보
    
    // 여러 개 가지는 것에 대한 표시
    public bool isStackable;
    public int maxStack;

    // 추가: 이 아이템이 어떤 기능을 가져야 하는지 표시
    public ItemFunction[] functions;
}
