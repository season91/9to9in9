using UnityEngine;

public enum BuildType
{
    Wall,
    Floor,
    Station,
}


public enum StationType
{
    Workbench, // 제작대
    Campfire, // 모닥불
    Smelter, // 제련대
    Anvil, // 모루
    None, // 상호작용 불가
}

public class BuildItemData : ItemData
{
    public GameObject previewPrefab; // 프리뷰용 프리팹
    public BuildType buildType;
    public StationType stationType;
    
    // 여러 개 가지는 것에 대한 표시
    public bool isStackable; // 여러개 가질 수 있는 아이템인지 구분
    public int maxStack;
}