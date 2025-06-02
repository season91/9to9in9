using System.Collections;
using DG.Tweening;
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
    [SerializeField] private UIDialoguePopup dialoguePopup;
    
    [SerializeField] private UIStateGroup stateGroup;
    [SerializeField] private UIQuickSlotPopup quickSlotPopup;
    [SerializeField] private UIDamageIndicator damageIndicator;
    [SerializeField] private UIEndingPopup endingPopup;

    [SerializeField] private TextMeshProUGUI tmpInformation;
    [SerializeField] private TextMeshProUGUI tmpDay;

    [SerializeField] private Image imgLogo; 
    
    private bool isStationOpened;

    public bool IsStationOpened() => isStationOpened;
    
    public GameObject GUIObject => gameObject;

    private void Reset()
    {
        inventoryPopup = GetComponentInChildren<UIInventoryPopup>();
        
        equipmentPopup = GetComponentInChildren<UIEquipmentPopup>();
        smelterPopup = GetComponentInChildren<UISmelterPopup>();
        campfirePopup = GetComponentInChildren<UICampfirePopup>();
        workbenchPopup = GetComponentInChildren<UIWorkbenchPopup>();
        anvilPopup = GetComponentInChildren<UIAnvilPopup>();
        dialoguePopup = GetComponentInChildren<UIDialoguePopup>();
        stateGroup = GetComponentInChildren<UIStateGroup>();

        stateGroup = GetComponentInChildren<UIStateGroup>();
        quickSlotPopup = GetComponentInChildren<UIQuickSlotPopup>();
        damageIndicator = GetComponentInChildren<UIDamageIndicator>();
        endingPopup = GetComponentInChildren<UIEndingPopup>();
        
        tmpInformation = transform.Find("Tmp_Information").GetComponent<TextMeshProUGUI>();
        tmpDay = transform.Find("Tmp_Day").GetComponent<TextMeshProUGUI>();
        
        imgLogo = transform.Find("Img_Logo").GetComponent<Image>();
    }

    public void Initialization()
    {
        inventoryPopup.Initialization();
        
        equipmentPopup.Initialization();
        smelterPopup.Initialization();
        campfirePopup.Initialization();
        workbenchPopup.Initialization();
        anvilPopup.Initialization();
        
        dialoguePopup.Initialization();
        quickSlotPopup.Initialization();
        damageIndicator.Initialization();
        endingPopup.Initialization();
        
        tmpInformation.gameObject.SetActive(false);

        currentStation = StationType.Default;
        isStationOpened = false;

        imgLogo.gameObject.SetActive(false);
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
                // 클릭한 것이 광물이면 광물 칸에 장착
                // 칸이 전부 찼으면 무시
                // 둘 다 아니면 무시
                return smelterPopup.TryPlaceItem(item);
            case StationType.Campfire:
                // 클릭한 것이 음식이면 음식 칸에 차례대로
                // 아니면 무시
                // 칸이 전부 찼으면 무시
                return campfirePopup.TryPlaceItem(item);
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
        if (currentStation == type)
        {
            MyDebug.Log($"this {currentStation} station is opened already! ");
            return;
        }
        
        CharacterManager.Player.controller.ToggleCursor();
        
        if(currentStation != StationType.Default)
            CloseStation();
        
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
        quickSlotPopup.Close();
        isStationOpened = true;
    }
    
    public void CloseStation()
    {
        CharacterManager.Player.controller.ToggleCursor();
        
        switch (currentStation) 
        {
            case StationType.None:
                equipmentPopup.Close();
                break;
            case StationType.Smelter:
                smelterPopup.Close();
                break;
            case StationType.Campfire:
                campfirePopup.Close();
                break;
            case StationType.Workbench:
                workbenchPopup.Close();
                break;
            case StationType.Anvil:
                anvilPopup.Close();
                break;
            default:
                Debug.Log("Unknown Station Type");
                break;
        }
        
        inventoryPopup.Close();
        quickSlotPopup.Open();
        isStationOpened = false;
        currentStation = StationType.Default;
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

    public void FirstDialogue(string line)
    {
        dialoguePopup.Open();
        dialoguePopup.FirstDialogue(line);
    }
        
    public void PlayTypingEffect(string line) => dialoguePopup.PlayTypingEffect(line);
    public bool DialoguePopupActve() => dialoguePopup.gameObject.activeSelf;
    public bool IsTyping() => dialoguePopup.IsTyping;
    public void SkipTyping(string fullText) => dialoguePopup.SkipTyping(fullText);
    public void HideDialogue() => dialoguePopup.HideDialogue();
    
    
    public void PlayDamageEffect(float duration) => damageIndicator.PlayDamageEffect(duration);

    public IEnumerator GameOverFlow()
    {
        yield return StartCoroutine(damageIndicator.DieScreenAnim());

        StartCoroutine(endingPopup.EndingCreditAnim());
    }

    public IEnumerator GameStartLogo()
    {
        imgLogo.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 650);
        imgLogo.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 650);
        imgLogo.color = Color.white;
        imgLogo.gameObject.SetActive(true);
        yield return null;
        
        imgLogo.rectTransform.DOSizeDelta(new Vector2(800, 800), 0.1f);
        yield return new WaitForSeconds(0.05f);
        imgLogo.rectTransform.DOSizeDelta(new Vector2(700, 700), 0.15f);
        yield return new WaitForSeconds(0.5f);
        imgLogo.rectTransform.DOSizeDelta(new Vector2(1200, 1200), 0.3f);
        imgLogo.DOFade(0, 0.3f);
        yield return new WaitForSeconds(0.3f);
        imgLogo.gameObject.SetActive(false);
    }
    
    
    #region  TestCode
#if  UNITY_EDITOR
    public void TestOpenEquipmentPopup()
    {
        currentStation = StationType.None;
        if (equipmentPopup.CanvasAlpha() >= 0.5f)
        {
            CloseStation();
        }
        else
        {
            OpenStation(currentStation);
        }
    }

    public void TestOpenSmelterPopup()
    {
        currentStation = StationType.Smelter;
        if (smelterPopup.CanvasAlpha() >= 0.5f)
        {
            CloseStation();
        }
        else
        {
            OpenStation(currentStation);
        }
    }
    
    public void TestOpenCampfirePopup()
    {
        currentStation = StationType.Campfire;
        if (campfirePopup.CanvasAlpha() >= 0.5f)
        {
            CloseStation();
        }
        else
        {
            OpenStation(currentStation);
        }
    }
    
    public void TestOpenWorkbenchPopup()
    {
            currentStation = StationType.Workbench;
            if (workbenchPopup.CanvasAlpha() >= 0.5f)
            {
                CloseStation();
            }
            else
            {
                OpenStation(currentStation);
            }
    }
    
    public void TestOpenAnvilPopup()
    {
        currentStation = StationType.Anvil;
        if (anvilPopup.CanvasAlpha() >= 0.5f)
        {
            CloseStation();
        }
        else
        {
            OpenStation(currentStation);
        }
    }
    
#endif

    #endregion
}
