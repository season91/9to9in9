using System;
using UnityEngine;
/// <summary>
/// [요청 처리] 아이템 상호작용관리
/// </summary>
public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] private float checkRate = 0.05f;
    [SerializeField] private float lastCheckTime;
    [SerializeField] private float maxCheckDistance = 1f;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private GameObject curInteractGameObject;
    private IInteractable curInteractable;
    private IInspectable curInspectable;
    
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if(Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            Debug.DrawRay(ray.origin, ray.direction * maxCheckDistance, Color.red);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if(hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    // 인터페이스 검사
                    curInteractable = curInteractGameObject.GetComponent<IInteractable>();
                    curInspectable = curInteractGameObject.GetComponent<IInspectable>();
                    if (curInspectable == null)
                    {
                        Debug.Log("curInspectable 정보가 null!"); return;
                    }
                    UIManager.Instance.ShowItemInformation(curInspectable.GetPromptText());
                }
            }
            else
            {
                curInteractGameObject = null;
                curInspectable = null;
                curInteractable = null;
                
                if(UIManager.Instance != null) UIManager.Instance.HideItemInformation();
            }
        }
    }

    public void OnInteract()
    {
        if (curInteractable == null) return;
        curInteractable.OnInteract();
    }
}