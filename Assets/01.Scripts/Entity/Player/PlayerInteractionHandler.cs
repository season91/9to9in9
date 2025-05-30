using System;
using UnityEngine;
/// <summary>
/// [요청 처리] 아이템 상호작용관리
/// </summary>
public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] private float checkRate = 0.05f;
    [SerializeField] private float lastCheckTime;
    [SerializeField] private float maxCheckDistance = 5f;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private GameObject curInteractGameObject;
    private IInteractable curInteractable;
    
    private Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        if(Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if(hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                }
            }
            else
            {
                curInteractGameObject = null;
                curInteractable = null;
            }
        }
    }

    public void OnInteract()
    {
        if (curInteractable == null) return;
        
        Debug.Log("OnInteract : " + curInteractGameObject.name);
        curInteractable.OnInteract();
    }
}