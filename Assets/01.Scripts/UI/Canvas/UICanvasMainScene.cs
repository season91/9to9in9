using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasMainScene : MonoBehaviour, IGUI
{
    public StationType currentStation;
    
    [SerializeField] private UIInventoryPopup inventoryPopup;
    
    [SerializeField] private UIEquipmentPopup equipmentPopup;
    [SerializeField] private UISmelterPopup smelterPopup;
    [SerializeField] private UICampfirePopup campfirePopup;
    [SerializeField] private UIWorkbenchPopup workbenchPopup;
    [SerializeField] private UIAnvilPopup anvilPopup;
    [SerializeField] private UIStateGroup stateGroup;

    [SerializeField] private TextMeshProUGUI tmpInformation;
    [SerializeField] private TextMeshProUGUI tmpDay;
    
    public GameObject GUIObject => gameObject;

    private void Reset()
    {
        inventoryPopup = GetComponentInChildren<UIInventoryPopup>();
        
        equipmentPopup = GetComponentInChildren<UIEquipmentPopup>();
        smelterPopup = GetComponentInChildren<UISmelterPopup>();
        campfirePopup = GetComponentInChildren<UICampfirePopup>();
        workbenchPopup = GetComponentInChildren<UIWorkbenchPopup>();
        anvilPopup = GetComponentInChildren<UIAnvilPopup>();

        stateGroup = GetComponentInChildren<UIStateGroup>();

        tmpInformation = transform.Find("Tmp_Information").GetComponent<TextMeshProUGUI>();
        tmpDay = transform.Find("Tmp_Day").GetComponent<TextMeshProUGUI>();

    }

    public void Initialization()
    {
        inventoryPopup.Initialization();
        
        equipmentPopup.Initialization();
        smelterPopup.Initialization();
        
        tmpInformation.gameObject.SetActive(false);
    }

    public void Open()
    {
    }

    public void Close()
    {
    }
    
    public bool TrySlotClickWithStation(ItemData item) 
    {
        switch (currentStation) 
        {
            case StationType.Smelter:
                // 클릭한 것이 연료 계열이면 연료 칸에 장착
                // 클릭한 것이 음식이면 음식 칸에 장착
                // 둘 다 아니면 무시
                // 칸이 전부 찼으면 무시
                return smelterPopup.TryPlaceItem(item);
            case StationType.Campfire:
                // 클릭한 것이 연료 계열이면 연료 칸에 장착
                // 클릭한 것이 광물이면 광물 칸에 장착
                // 칸이 전부 찼으면 무시
                // 둘 다 아니면 무시
                // return campfirePopup.TryPlaceItem(item);
            case StationType.None:
                // None이면 장비 장착 창
                // 장착, 장착된 장비도 Select하면 장착 해제
                // 장착된 장비가 있으면 교체
                // 교체될 때 인벤토리로 다시 들어와야 됨 해당 아이템이
                return equipmentPopup.TryPlaceItem(item);
            case StationType.Workbench:
            case StationType.Anvil:
            default:
                return false;
        }
    }

    public void OpenStation(StationType type)
    {
        currentStation = type;
        switch (type) 
        {
            case StationType.None:
                equipmentPopup.Open();
                break;
            case StationType.Smelter:
                smelterPopup.Open();
                break;
            case StationType.Campfire:
                campfirePopup.Open();
                break;
            case StationType.Workbench:
                workbenchPopup.Open();
                break;
            case StationType.Anvil:
                anvilPopup.Open();
                break;
            default:
                Debug.Log("Unknown Station Type");
                break;
        }
        
        inventoryPopup.Open();
    }

    public void ShowItemName(string information)
    {
        tmpInformation.text = information;
        tmpInformation.gameObject.SetActive(true);
    }
    
    public void HideItemName()
    {
        if (tmpInformation.IsActive())
        {
            tmpInformation.text = "";
            tmpInformation.gameObject.SetActive(false);
        }
    }

    #region  TestCode
#if  UNITY_EDITOR
    public void TestOpenEquipmentPopup()
    {
        if (equipmentPopup.gameObject.activeSelf)
        {
            equipmentPopup.Close();
            inventoryPopup.Close();
            return;
        }
        
        currentStation = StationType.None;
        equipmentPopup.Open();
        inventoryPopup.Open();
    }

    public void TestOpenSmelterPopup()
    {
        if (smelterPopup.gameObject.activeSelf)
        {
            smelterPopup.Close();
            inventoryPopup.Close();
            return;
        }
        
        currentStation = StationType.Smelter;
        smelterPopup.Open();
        inventoryPopup.Open();
    }
#endif

    #endregion
}
