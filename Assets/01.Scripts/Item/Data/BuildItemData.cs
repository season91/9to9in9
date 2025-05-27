using UnityEngine;

public enum BuildType
{
    Wall,
    Floor,
}

public class BuildItemData : ItemData
{
    public GameObject previewPrefab; // 프리뷰용 프리팹
    public BuildType buildType;
    
    // 여러 개 가지는 것에 대한 표시
    public bool isStackable; // 여러개 가질 수 있는 아이템인지 구분
    public int maxStack;
}