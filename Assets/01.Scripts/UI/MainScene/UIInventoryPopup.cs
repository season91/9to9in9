using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIInventoryPopup : MonoBehaviour, IGUI
{
    [SerializeField] private RectTransform rectTransf;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private List<GUIItemSlotInventory> inventorySlots;
    [SerializeField] private TextMeshProUGUI tmpTitle;

    [SerializeField] private UIQuickSlotPopup quickSlotPopup;

    private int curInventoryCount;
    
    PlayerInventoryController inventoryCtrlr;
    
    public GameObject GUIObject => gameObject;

    void Reset()
    {
        rectTransf = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        inventorySlots = transform.Find("Layout_PlayerItems")?.GetComponentsInChildren<GUIItemSlotInventory>().ToList();
        tmpTitle = transform.Find("Tmp_InventoryTitle")?.GetComponent<TextMeshProUGUI>();
        quickSlotPopup = GetComponentInChildren<UIQuickSlotPopup>();
    }
    
    public void Initialization()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        InitializationInventoryGUI();
    }

    void InitializationInventoryGUI()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            int index = i; // 캡쳐 문제 회피
            inventorySlots[i].Initialization();
            inventorySlots[i].SetClickEvent(OnItemSlotSelected, index);
        }
        quickSlotPopup.Initialization();
        quickSlotPopup.SettingInventoryQuickUI();
    }

    public void Open()
    {
        StationType currentStation = UIManager.Instance.CurrentStation();
        
        switch (currentStation)
        {
            case StationType.None:
                tmpTitle.text = "Inventory";
                break;
            case StationType.Campfire:
                tmpTitle.text = "Campfire";
                break;
            case StationType.Smelter:
                tmpTitle.text = "Smelter";
                break;
            case StationType.Workbench:
                tmpTitle.text = "Workbench";
                break;
            case StationType.Anvil:
                tmpTitle.text = "Anvil";
                break;
            default:
                break;
        }
        
        inventoryCtrlr = CharacterManager.Player.inventoryController;
        
        inventoryCtrlr.UpdateInventory -= SettingInventoryUI;
        inventoryCtrlr.UpdateInventory += SettingInventoryUI;
        
        SettingInventoryUI();
        
        canvasGroup.DOFade(1, 0.2f);
        rectTransf.DOAnchorPosY(0, 0.3f);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    
    public void Close()
    {
        canvasGroup.DOFade(0, 0.2f);
        rectTransf.DOAnchorPosY(-300, 0.3f);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void SettingInventoryUI()
    {
        int inventoryCount = inventoryCtrlr.GetAllItemCount();
        
        if (curInventoryCount != inventoryCount)
        {
            InitializationInventoryGUI();
        }
        
        curInventoryCount = inventoryCount;
        
        for (int i = 0; i < inventoryCount; i++)
        {
            if (i < inventoryCount)
            {
                int pcs = inventoryCtrlr.GetPcs(i);
                Sprite icon = inventoryCtrlr.GetIcon(i);
                if(icon == null)
                    return;
                inventorySlots[i].Show(icon, pcs);
            } 
        }   
    }

    void SettingSlotUI(int index)
    {
        int pcs = CharacterManager.Player.inventoryController.GetPcs(index);
        inventorySlots[index].SetPcs(pcs);
    }

    void OnItemSlotSelected(int index)
    {
        ItemData item = inventoryCtrlr.GetItem(index);

        bool isSuccess = UIManager.Instance.TrySlotClickWithStation(item);

        if (isSuccess) 
        {
            inventoryCtrlr.RemoveItem(index);
            // inventorySlots[index].Select();
        }
        else 
        {
            Debug.Log("You can't place item in the slot! Slot is full!");
        }
    }
    
#if UNITY_EDITOR
    public float CanvasAlpha() => canvasGroup.alpha;
#endif
}
