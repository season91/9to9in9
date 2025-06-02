using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// [요청 처리] InputSystem 이벤트 목적에 맞게 분배 관리
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    private InputActionAsset playerInputAsset;
    private PlayerController playerController;
    private PlayerEquipHandler playerEquipHandler;
    private PlayerInteractionHandler playerInteractionHandler;
    
    private void Awake()
    {
        playerInputAsset = GetComponent<PlayerInput>().actions;
        playerController = GetComponent<PlayerController>();
        playerEquipHandler = GetComponent<PlayerEquipHandler>();
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
        
        playerInputAsset["Run"].performed += OnRunStarted;
        playerInputAsset["Run"].canceled += OnRunCanceled;

        playerInputAsset["Interact"].performed += OnInteractInput;
        
        playerInputAsset["Inventory"].started += OnInventoryInput;
        
        playerInputAsset["Use"].performed += OnUseInput;
        
        playerInputAsset["Build"].started += OnBuildInput;
        
        playerInputAsset["QuickSlot"].started += OnQuickSlotInput;
        
        playerInputAsset["Close"].performed += OnClosePopupInput;
    }

    private void OnDisable()
    {
        playerInputAsset["Move"].performed -= OnMoveInput;
        playerInputAsset["Move"].canceled -= OnMoveInput;
        
        playerInputAsset["Jump"].started-= OnJumpInput;
        //playerInputAsset["Jump"].performed -= OnJumpInput;
        
        playerInputAsset["Look"].started -= OnLookInput;
        playerInputAsset["Look"].canceled -= OnLookInput;
        
        playerInputAsset["Run"].performed -= OnRunStarted;
        playerInputAsset["Run"].canceled -= OnRunCanceled;

        playerInputAsset["Interact"].performed -= OnInteractInput;
        
        playerInputAsset["Inventory"].started -= OnInventoryInput;
        
        playerInputAsset["Use"].performed -= OnUseInput;
        
        playerInputAsset["Build"].started -= OnBuildInput;
        
        playerInputAsset["QuickSlot"].started -= OnQuickSlotInput;
        
        playerInputAsset["Close"].performed -= OnClosePopupInput;
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
    
    private void OnRunStarted(InputAction.CallbackContext context)
    {
        playerController.StartRunning();
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        playerController.StopRunning();
    }

    private void OnInteractInput(InputAction.CallbackContext context)
    {
        playerInteractionHandler.OnInteract();
    }

    private void OnUseInput(InputAction.CallbackContext context)
    {
        playerEquipHandler.UseEquippedWeapon();
    }

    private void OnInventoryInput(InputAction.CallbackContext context)
    {
        // if (UIManager.Instance.IsStationOpened())
        // {
        //     UIManager.Instance.CloseStation();
        //     playerController.ToggleCursor();
        //     return;
        // }
        playerController.OnInventory();
        UIManager.Instance.OpenStation(StationType.None); // Inventory 열기
    }

    private void OnBuildInput(InputAction.CallbackContext context)
    {
        //TODO
    }

    private void OnQuickSlotInput(InputAction.CallbackContext context)
    {
        playerController.OnQuickSlot(GetPressedKeyValue());
    }

    private void OnClosePopupInput(InputAction.CallbackContext context)
    {
        if(UIManager.Instance.IsStationOpened())
            UIManager.Instance.CloseStation();
        else
        {
            UIManager.Instance.OnOffOption();
            playerController.ToggleCursor();
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
