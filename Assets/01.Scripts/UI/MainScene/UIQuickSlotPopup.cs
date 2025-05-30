using UnityEngine;

public class UIQuickSlotPopup : MonoBehaviour, IGUI
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] GUIItemSlotQuick[] quickSlots;
    
    PlayerInventoryController inventoryCtrlr;
    private int curSlotItemCount;
    
    private void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        quickSlots = GetComponentsInChildren<GUIItemSlotQuick>();
    }

    public GameObject GUIObject { get; }

    public void Initialization()
    {
        InitializationSlotsGUI();
        Open();
    }

    public void Open()
    {
        inventoryCtrlr = CharacterManager.Player.inventoryController;
        
        inventoryCtrlr.UpdateInventory -= SettingSlotsUI;
        inventoryCtrlr.UpdateInventory += SettingSlotsUI;
        
        SettingSlotsUI();
        
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    
    public void Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    
    void InitializationSlotsGUI()
    {
        for (int i = 0; i < quickSlots.Length; i++)
        {
            quickSlots[i].Initialization();
            quickSlots[i].SetItemIndex(i);
        }
    }
    
    void SettingSlotsUI()
    {
        PlayerInventoryController inventoryCtrlr = CharacterManager.Player.inventoryController;
        int slotsCount = inventoryCtrlr.GetQuickCount();
        
        if (curSlotItemCount != slotsCount)
        {
            InitializationSlotsGUI();
        }
        
        curSlotItemCount = slotsCount;
        
        for (int i = 0; i < slotsCount; i++)
        {
            if (i < slotsCount)
            {
                int pcs = inventoryCtrlr.GetQuickPcs(i);
                Sprite icon = inventoryCtrlr.GetQuickIcon(i);
                if(icon == null)
                    return;
                quickSlots[i].Show(icon, pcs);
            } 
        }   
    }
}
