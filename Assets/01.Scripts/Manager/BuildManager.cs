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
    [SerializeField] private LayerMask buildableLayer; // 건축 가능한 레이어 (ex.Ground)
    [SerializeField] private LayerMask structureLayer; // 건축물 공통 레이어
    
    [SerializeField] private Transform playerTransform; // 기준 플레이어 위치
    [SerializeField] private float maxBuildDistance; // 플레이어 기준 건축 가능 범위
    
    private readonly Color validColor = new Color(0f, 1f, 0f, 0.4f);
    private readonly Color invalidColor = new Color(1f, 0f, 0f, 0.4f);

    private BuildItemData currentItem; // 선택한 건축 아이템 정보
    private GameObject currentPreview; // 현재 화면에 보여지는 프리뷰 오브젝트
    private MeshRenderer[] previewRenderers; // 색상 변경용
    private Vector3 halfExtents; // 콜라이더 겹침 검사
    
    private Camera cam;
    
    #region Unity Events

    private void Reset()
    {
        buildableLayer = LayerMask.GetMask("Ground");
        structureLayer = LayerMask.GetMask("Structure");
        
        maxBuildDistance = 2f;
        
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

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (!Physics.Raycast(ray, out RaycastHit hit, maxBuildDistance, buildableLayer)) return;
        
        // Ray 닿는 곳으로 프리뷰 위치 정함. 프리뷰 위치가 곧 설치 위치
        SetPreviewPosition(hit); 
        Vector3 targetPos = currentPreview.transform.position;
        
        // 건축 가능한지 여부 판단, 초록색 or 빨간색 표시
        bool isValid = IsInBuildRange(targetPos) && !IsOverlappingStructure(targetPos);
        SetPreviewColor(isValid ? validColor : invalidColor);
        
        // [RMB] 설치 확인
        if (Input.GetMouseButtonDown(1) && isValid)
        {
            //Debug.Log("RMB Clicked");
            PlaceBuildItem(targetPos);
        }
            
        // 인풋시스템 등록으로 리팩토링 필요
        // [R] 회전
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Debug.Log("rotation clicked");
            currentPreview.transform.Rotate(Vector3.up, 45f);
        }

        // [ESC] 설치 취소
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     Debug.Log("build canceled");
        //     CancelBuild();
        // }
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
        
        // Station이 아닌 경우 스냅 실행
        if (currentItem.stationType == StationType.None)
        {
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            if (Physics.Raycast(ray, out RaycastHit structureHit, maxBuildDistance, structureLayer))
            {
                BoxCollider box = structureHit.collider as BoxCollider;
                if (!box) return;
                
                Vector3 basePos = box.transform.position;
                Vector3 normal = structureHit.normal.normalized;
                Vector3 snapDirection = GetRoundedDirection(normal);
                
                Vector3 size = box.size;
                Vector3 offset = box.transform.rotation * Vector3.Scale(snapDirection, size);
                
                finalPos = basePos + offset;
            }
        }
        
        // Station이면 그냥 Ray 닿는 곳으로 설정
        currentPreview.transform.position = finalPos;
    }
    
    /// <summary>
    /// 건축 가능 범위 내인지 체크
    /// </summary>
    private bool IsInBuildRange(Vector3 targetPos)
    {
        return Vector3.Distance(playerTransform.position, targetPos) <= maxBuildDistance;
    }

    /// <summary>
    /// 겹치는지 체크
    /// </summary>
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
            //Debug.Log($"set color to {color}");
        }
    }

    /// <summary>
    /// 건축물을 스냅할 방향을 반환해줌
    /// </summary>
    private Vector3 GetRoundedDirection(Vector3 normal)
    {
        Vector3 dir = Vector3.zero;

        if (Mathf.Abs(normal.z) > Mathf.Abs(normal.x))
            dir.z = Mathf.Sign(normal.z);
        else
            dir.x = Mathf.Sign(normal.x);

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
