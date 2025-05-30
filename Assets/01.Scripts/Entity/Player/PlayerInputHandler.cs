using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// [요청 처리] InputSystem 이벤트 목적에 맞게 분배 관리
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    private InputActionAsset playerInputAsset;
    private PlayerController playerController;
    private PlayerInteractionHandler playerInteractionHandler;
    
    private void Awake()
    {
        playerInputAsset = GetComponent<PlayerInput>().actions;
        playerController = GetComponent<PlayerController>();
        playerInteractionHandler = GetComponent<PlayerInteractionHandler>();
    }

    private void OnEnable()
    {
        playerInputAsset["Move"].performed += OnMoveInput;
        playerInputAsset["Move"].canceled += OnMoveInput;

        playerInputAsset["Jump"].started += OnJumpInput;
        //playerInputAsset["Jump"].performed += OnJumpInput;

        playerInputAsset["Look"].started += OnLookInput;
        playerInputAsset["Look"].canceled += OnLookInput;

        playerInputAsset["Interact"].started += OnInteractInput;
        
        playerInputAsset["Inventory"].started += OnInventoryInput;
        
        playerInputAsset["Use"].started += OnUseInput;
        
        playerInputAsset["Build"].started += OnBuildInput;
        
        playerInputAsset["QuickSlot"].started += OnQuickSlotInput;
    }

    private void OnDisable()
    {
        playerInputAsset["Move"].performed -= OnMoveInput;
        playerInputAsset["Move"].canceled -= OnMoveInput;
        
        playerInputAsset["Jump"].started-= OnJumpInput;
        //playerInputAsset["Jump"].performed -= OnJumpInput;
        
        playerInputAsset["Look"].started -= OnLookInput;
        playerInputAsset["Look"].canceled -= OnLookInput;

        playerInputAsset["Interact"].started -= OnInteractInput;
        
        playerInputAsset["Inventory"].started -= OnInventoryInput;
        
        playerInputAsset["Use"].started -= OnUseInput;
        
        playerInputAsset["Build"].started -= OnBuildInput;
        
        playerInputAsset["QuickSlot"].started -= OnQuickSlotInput;
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        playerController.SetCurMove(context.ReadValue<Vector2>());
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        playerController.SetMouseDelta(context.ReadValue<Vector2>());
    }
    
    private void OnJumpInput(InputAction.CallbackContext context)
    {
        playerController.Jumping();
    }

    private void OnInteractInput(InputAction.CallbackContext context)
    {
        playerInteractionHandler.OnInteract();
    }

    private void OnUseInput(InputAction.CallbackContext context)
    {
        //TODO
        //공격 모션 시 콜백 함수 호출 필요
        //함수 위치 조정 필요
    }

    private void OnInventoryInput(InputAction.CallbackContext context)
    {
        playerController.OnInventory();
    }

    private void OnBuildInput(InputAction.CallbackContext context)
    {
        //TODO
    }

    private void OnQuickSlotInput(InputAction.CallbackContext context)
    {
        int index = GetPressedKeyValue();
        if (index != -1)
        {
            Debug.Log("pressed key index :: "+index);
            //QuickSlotUse(index);
        }
    }

    private int GetPressedKeyValue()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) return 0;
        if (Keyboard.current.digit2Key.wasPressedThisFrame) return 1;
        if (Keyboard.current.digit3Key.wasPressedThisFrame) return 2;
        if (Keyboard.current.digit4Key.wasPressedThisFrame) return 3;
        if (Keyboard.current.digit5Key.wasPressedThisFrame) return 4;
        if (Keyboard.current.digit6Key.wasPressedThisFrame) return 5;
        if (Keyboard.current.digit7Key.wasPressedThisFrame) return 6;
        return -1;
    }
}
