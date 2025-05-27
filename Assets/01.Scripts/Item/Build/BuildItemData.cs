using UnityEngine;

public enum BuildType
{
    Wall,
    Floor,
}

[CreateAssetMenu(menuName = "Item/Build Item")]
public class BuildItemData : ItemData
{
    [SerializeField] private BuildType buildType;
    [SerializeField] private GameObject previewPrefab;
    public GameObject PreviewPrefab => previewPrefab;
}