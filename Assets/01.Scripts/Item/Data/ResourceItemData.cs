using UnityEngine;

// 자원 사용 타입
public enum ResourceType
{
    None,
    Fuel, //연료
    Metal //금속
}
public class ResourceItemData  : ItemData
{
    public ResourceType resourceType;
}
