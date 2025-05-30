using System;
using UnityEngine;
using UnityEngine.InputSystem.Editor;

/// <summary>
/// [제어] 인풋시스템 바인딩하고 움직임 구현 (이동, 점프) 
/// </summary>
public class PlayerController : MonoBehaviour, IMoveable, IJumpable
{
    [Header("Movement")]
    [SerializeField] private LayerMask groundLayerMask;
    private Vector2 curMoveInput;
    
    // [SerializeField] private float moveSpeed = 10f;
    // [SerializeField] private float minMoveSpeed = 3f;
    // [SerializeField] private float maxMoveSpeed = 20f;
    // [SerializeField] private float jumpPower = 7f;


    [Header("CameraLook")]
    [SerializeField] private float minXLook = -85f;
    [SerializeField] private float maxXLook = 85f;
    [SerializeField] private float lookSensitivity = 0.1f;
    private Transform cameraContainer;
    private const string cameraContainerName = "CameraContainer";
    
    private float camCurXRot;
    private Vector2 mouseDelta;
    private bool canLook = true;
    
    private Rigidbody rigidBody;
    private StatHandler statHandler;
    
    public Action inventoryAction;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        cameraContainer = transform.Find(cameraContainerName).transform;
        
        // null 체크
        if (rigidBody == null) Debug.LogError("Rigidbody not found");
        if (cameraContainer == null) Debug.LogError("CameraContainer not found");
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        statHandler = CharacterManager.Player.statHandler;
        if (statHandler == null) Debug.LogError("StatHandler not found");
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    public void SetCurMove(Vector2 input)
    {
        curMoveInput = input;
    }

    public void SetMouseDelta(Vector2 input)
    {
        mouseDelta = input;
    }
    
    public void Move()
    {
        Vector3 dir = transform.forward * curMoveInput.y + transform.right * curMoveInput.x;
        dir *= statHandler.Get(StatType.MoveSpeed);
        dir.y = rigidBody.velocity.y;
        
        rigidBody.velocity = dir;

        if (rigidBody.velocity.magnitude > 0.1f)
        {
            // Debug.Log($"{rigidBody.velocity.magnitude}");
            // // SoundManager.Instance.PlayStepSfx();
        }
    }

    private void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }
    #region IJumpable
    public void Jumping()
    {
        if (IsGrounded())
        {
            rigidBody.AddForce(Vector2.up * statHandler.Get(StatType.JumpPower), ForceMode.Impulse);
        }
    }
    
    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) +(transform.up * 0.01f), Vector3.down)
        };

        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    public void OnInventory()
    {
        inventoryAction?.Invoke();
        ToggleCursor();
    }
    
    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
