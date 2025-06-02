using System;
using UnityEngine;

/// <summary>
/// [제어] 인풋시스템 바인딩하고 움직임 구현 (이동, 점프) 
/// </summary>
public class PlayerController : MonoBehaviour, IMoveable, IJumpable
{
    [Header("Movement")]
    [SerializeField] private LayerMask groundLayerMask;
    private Vector2 curMoveInput;

    [Header("CameraLook")]
    [SerializeField] private float minXLook = -85f;
    [SerializeField] private float maxXLook = 85f;
    [SerializeField] private float lookSensitivity = 0.1f;
    private Transform cameraContainer;
    private const string cameraContainerName = "CameraContainer";
    
    private float camCurXRot;
    private Vector2 mouseDelta;
    private bool canLook = true;
    
    [Header("Run")]
    [SerializeField] private float drainInterval = 0.2f;
    
    public bool isRunning;
    private float staminaDrainRate;
    private float runBonus = 1f;
    private float drainTimer;
    
    private Rigidbody rigidBody;
    private StatHandler statHandler;
    
    public Action inventoryAction;
    
    public GameObject jumpParticlePrefab;
    private bool wasGrounded = true; // 착지 체크 용

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
        
        staminaDrainRate = (CharacterManager.Player.statHandler.GetPassive(StatType.Stamina) + 2f);
    }

    private void Update()
    {
        bool isGrounded = IsGrounded();
        if (!wasGrounded && isGrounded)
        {
            Instantiate(jumpParticlePrefab, transform.position, Quaternion.identity);
        }

        wasGrounded = isGrounded; // 항상 마지막에 상태 갱신
        
        if (!isRunning) return;
        
        drainTimer += Time.deltaTime;
        if (drainTimer >= drainInterval)
        {
            drainTimer = 0f;
            CharacterManager.Player.statHandler.Modify(StatType.Stamina, -staminaDrainRate * drainInterval);

            UIManager.Instance.UpdateStatUI(StatType.Stamina);

            // 스태미나 0이면 자동으로 걷기로 전환
            if (CharacterManager.Player.statHandler.IsEmpty(StatType.Stamina))
            {
                StopRunning();
            }
        }
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
            SoundManager.Instance.PlayStepSfx();
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
        // ToggleCursor();
    }
    
    public void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    public void OnQuickSlot(int index)
    {
        if (index != -1)
        {
            Debug.Log("pressed key index :: "+index);
            ItemData useItem = CharacterManager.Player.inventoryController.UseItemInQuickSlot(index);
            if (useItem != null)
            {
                UseQuickSlotItem(useItem);
            }
        }
    }
    
    public void UseQuickSlotItem(ItemData itemData)
    {
        switch (itemData.type)
        {
            case ItemType.Build:
                BuildManager.Instance.EnterBuildMode(itemData as BuildItemData);
                break;

            case ItemType.Consumable:
                UseConsumableItem(itemData as ConsumableItemData);
                break;
        }
    }
    
    private void UseConsumableItem(ConsumableItemData item)
    {
        for (int i = 0; i < item.consumableTypes.Length; i++)
        {
            float value = item.amounts[i];
            switch (item.consumableTypes[i])
            {
                case ConsumableType.Health:
                    CharacterManager.Player.statHandler.Modify(StatType.Health, value);
                    UIManager.Instance.UpdateStatUI(StatType.Health);
                    break;

                case ConsumableType.Hunger:
                    CharacterManager.Player.statHandler.Modify(StatType.Hunger, value);
                    UIManager.Instance.UpdateStatUI(StatType.Hunger);
                    break;

                case ConsumableType.Stamina:
                    CharacterManager.Player.statHandler.Modify(StatType.Stamina, value);
                    UIManager.Instance.UpdateStatUI(StatType.Stamina);
                    break;
            }
        }
    }
    
    public void StartRunning()
    {
        isRunning = true;
        CharacterManager.Player.statHandler.ModifyBonus(StatType.MoveSpeed, runBonus);
    }

    public void StopRunning()
    {
        isRunning = false;
        CharacterManager.Player.statHandler.ModifyBonus(StatType.MoveSpeed, 0);
    }
}
