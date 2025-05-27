using UnityEngine;

/// <summary>
/// 건축
/// </summary>
public class BuildManager : MonoBehaviour
{
    private static readonly int MeshColor = Shader.PropertyToID("_Color");
    public static BuildManager Instance { get; private set; }
    
    [Header("설정")]
    [SerializeField] private LayerMask buildableLayer;
    [SerializeField] private float maxBuildDistance = 5f;
    [SerializeField] private Transform playerTransform;
    
    private Color validColor = new Color(0f, 1f, 0f, 0.4f);
    private Color invalidColor = new Color(1f, 0f, 0f, 0.4f);

    private BuildItemData currentItem;
    private GameObject currentPreview;
    private MeshRenderer[] previewRenderers;
    
    private Camera cam;
    
#region Unity Events
    
    private void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }


    private void Update()
    {
        if (!currentItem || !currentPreview) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        //if (!Physics.Raycast(ray, out RaycastHit hit, 10f, buildableLayer)) return;
        //Vector3 targetPos = hit.point;
        
        //TestCode
        Vector3 targetPos = playerTransform.position;
        currentPreview.transform.position = targetPos;

        bool isValid = IsInBuildRange(targetPos);

        SetPreviewColor(isValid ? validColor : invalidColor);

        if (Input.GetMouseButtonDown(1) && isValid)
            PlaceBuildItem(targetPos);

        if (Input.GetKeyDown(KeyCode.R))
            currentPreview.transform.Rotate(Vector3.up, 90f);

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelBuild();
    }

#endregion
    
    /// <summary>
    /// 건축 프리뷰 모드 진입 함수
    /// </summary>
    public void EnterBuildMode(BuildItemData item)
    {
        currentItem = item;
        
        if (currentPreview) Destroy(currentPreview);
        if (!item.PreviewPrefab) return;
        
        currentPreview = Instantiate(item.PreviewPrefab);
        previewRenderers = currentPreview.GetComponentsInChildren<MeshRenderer>();
    }


    private bool IsInBuildRange(Vector3 targetPos)
    {
        return Vector3.Distance(playerTransform.position, targetPos) <= maxBuildDistance;
    }
    
    
    private void SetPreviewColor(Color color)
    {
        foreach (var renderer in previewRenderers)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            block.SetColor(MeshColor, color);
            renderer.SetPropertyBlock(block);
        }
    }


    private void PlaceBuildItem(Vector3 position)
    {
        Debug.Log("Place Build Item");
        Instantiate(currentItem.prefab, position, currentPreview.transform.rotation);
        Destroy(currentPreview);
        currentPreview = null;
    }


    private void CancelBuild()
    {
        if (currentPreview) Destroy(currentPreview);
        currentItem = null;
        currentPreview = null;
    }
}
