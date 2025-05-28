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
}