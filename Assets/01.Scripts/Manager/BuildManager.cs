using System;
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
    [SerializeField] private LayerMask structureLayer;
    
    [SerializeField] private float maxBuildDistance;
    [SerializeField] private float snapRadius;
    
    [SerializeField] private Transform playerTransform;
    
    private readonly Color validColor = new Color(0f, 1f, 0f, 0.4f);
    private readonly Color invalidColor = new Color(1f, 0f, 0f, 0.4f);

    private BuildItemData currentItem;
    private GameObject currentPreview;
    private MeshRenderer[] previewRenderers;
    private Vector3 halfExtents;
    
    private Camera cam;
    
    #region Unity Events

    private void Reset()
    {
        buildableLayer = LayerMask.GetMask("Ground");
        structureLayer = LayerMask.GetMask("Structure");
        
        maxBuildDistance = 2f;
        snapRadius = 0.5f;
        
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    
    private void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }


    private void Update()
    {
        if (!currentItem || !currentPreview) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (!Physics.Raycast(ray, out RaycastHit hit, maxBuildDistance, buildableLayer)) return;
        
        SetPreviewPosition(hit); // Ray 닿는 곳으로 프리뷰 위치 정함
        Vector3 targetPos = currentPreview.transform.position;
        
        bool isValid = IsInBuildRange(targetPos) && !IsOverlappingStructure(targetPos);
        SetPreviewColor(isValid ? validColor : invalidColor); // 건축 가능하면 초록색, 불가능하면 빨간색
        
        // [RMB] 설치 확인
        if (Input.GetMouseButtonDown(1) && isValid)
        {
            Debug.Log("RMB Clicked");
            PlaceBuildItem(targetPos);
        }
            
        // [R] 회전
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("rotation clicked");
            currentPreview.transform.Rotate(Vector3.up, 45f);
        }

        // [ESC] 설치 취소
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("build canceled");
            CancelBuild();
        }
    }

    #endregion
    
    /// <summary>
    /// 건축 프리뷰 모드 진입 함수
    /// </summary>
    /// <param name="item">선택한 건축 아이템 데이터</param>
    public void EnterBuildMode(BuildItemData item)
    {
        currentItem = item;
        
        if (currentPreview) Destroy(currentPreview);
        if (!item.previewPrefab) return;
        
        currentPreview = Instantiate(item.previewPrefab);
        previewRenderers = currentPreview.GetComponentsInChildren<MeshRenderer>();
    }
    
    /// <summary>
    /// 프리뷰 위치를 타겟 좌표로 이동시킴
    /// </summary>
    private void SetPreviewPosition(RaycastHit hit)
    {
        if (!currentPreview) return;

        Vector3 finalPos = hit.point;
        
        if (currentItem.stationType == StationType.None)
        {
            if (Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out RaycastHit structureHit,
                maxBuildDistance,
                structureLayer))
            {
                Vector3 basePos = structureHit.collider.transform.position;
                Vector3 normal = structureHit.normal.normalized;
                Vector3 size = structureHit.collider.bounds.size;
                Vector3 snapDirection = GetRoundedDirection(normal);

                finalPos = basePos + Vector3.Scale(snapDirection, size);
            }
        }

        currentPreview.transform.position = finalPos;
    }
    
    /// <summary>
    /// 건축 가능 범위 내인지 체크
    /// </summary>
    private bool IsInBuildRange(Vector3 targetPos)
    {
        return Vector3.Distance(playerTransform.position, targetPos) <= maxBuildDistance;
    }


    private bool IsOverlappingStructure(Vector3 position)
    {
        Quaternion rotation = currentPreview.transform.rotation;
        return Physics.CheckBox(position, halfExtents, rotation, structureLayer);
    }

    /// <summary>
    /// 프리뷰 오브젝트의 머티리얼 색상 변경
    /// </summary>
    private void SetPreviewColor(Color color)
    {
        foreach (var _renderer in previewRenderers)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(block);
            block.SetColor(MeshColor, color);
            _renderer.SetPropertyBlock(block);
            Debug.Log($"set color to {color}");
        }
    }


    private Vector3 GetRoundedDirection(Vector3 normal)
    {
        Vector3 dir = Vector3.zero;

        float x = Mathf.Round(normal.x);
        float z = Mathf.Round(normal.z);

        if (Mathf.Abs(x) > 0.5f) dir.x = Mathf.Sign(x);
        if (Mathf.Abs(z) > 0.5f) dir.z = Mathf.Sign(z);

        return dir;
    }

    /// <summary>
    /// 건축 확정
    /// </summary>
    private void PlaceBuildItem(Vector3 position)
    {
        Debug.Log("Place Build Item");
        Instantiate(currentItem.prefab, position, currentPreview.transform.rotation);
        Destroy(currentPreview);
        currentPreview = null;
    }

    /// <summary>
    /// 건축 취소
    /// </summary>
    private void CancelBuild()
    {
        if (currentPreview) Destroy(currentPreview);
        currentItem = null;
        currentPreview = null;
    }
}
