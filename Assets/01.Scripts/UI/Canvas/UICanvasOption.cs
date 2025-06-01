using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// 이 파일은 완전히 하드코딩!!!
// 리팩토링 필요!!!
public class UICanvasOption : MonoBehaviour, IGUI
{
    [SerializeField] private RectTransform rectTransf;
    [SerializeField] CanvasGroup canvasGroup;
    
    [Header("Buttons")] 
    [SerializeField] private GUIButtonCraftType[] btnTypes;

    private Dictionary<string, GUIButtonCraftType> btnByType;

    [Header("Audio")] 
    [SerializeField] private GameObject groupAudio;
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Slider sliderBgm;
    [SerializeField] private Slider sliderSfx;
    
    [Header("HowTo")] 
    [SerializeField] private GameObject groupHowTo;
    [SerializeField] private Image imgHowTo;
    [SerializeField] private Image imgHowToPop;
    [SerializeField] private Sprite[] spritesHowTo; // inspector에서 직접 연결해야 됨 넣을 sprite

    public GameObject GUIObject { get; }

    private void Reset()
    {
        rectTransf = transform.Find("Group_Option")?.GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        btnTypes = transform.GetComponentsInChildren<GUIButtonCraftType>();
        
        Transform audioRootTr = transform.Find("Group_Option/Group_Audio").GetComponent<Transform>();
        groupAudio = audioRootTr.gameObject;
        sliderMaster = audioRootTr.Find("Layout_Slider/Slider_Master")?.GetComponent<Slider>();
        sliderBgm = audioRootTr.Find("Layout_Slider/Slider_BGM")?.GetComponent<Slider>();
        sliderSfx = audioRootTr.Find("Layout_Slider/Slider_SFX")?.GetComponent<Slider>();
        
        Transform howToRootTr = transform.Find("Group_Option/Group_HowTo").GetComponent<Transform>();
        groupHowTo = howToRootTr.gameObject;
        imgHowTo = howToRootTr.Find("Img_Mask/Img_HowTo (1920*1080)")?.GetComponent<Image>();
        imgHowToPop = howToRootTr.Find("Img_HowToPop")?.GetComponent<Image>();
    }

    public void Initialization()
    {
        // 기능 구현이 우선이니 우선 하드 코딩
        // 리팩토링 필요
        btnByType = new Dictionary<string, GUIButtonCraftType>();
        
        btnByType["Audio"] = btnTypes[0];
        btnByType["Audio"].Setting("Audio");
        btnByType["Audio"].SetClickEvent(SettingGUIByType);

        btnByType["HowTo"] = btnTypes[1];
        btnByType["HowTo"].Setting("HowTo");
        btnByType["HowTo"].SetClickEvent(SettingGUIByType);

        groupAudio.SetActive(false);
        groupHowTo.SetActive(false);

        rectTransf.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Open()
    {
        // Esc 누를 때, 호출 하기.
        if (canvasGroup.alpha >= 0.5f)
        {
            Close();
        }
        else
        {
            SettingGUIByType("Audio");
            
            canvasGroup.DOFade(1, 0.2f);
            rectTransf.DOScale(1, 0.5f).SetEase(Ease.OutBack);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void Close()
    {
        canvasGroup.DOFade(0, 0.2f);
        rectTransf.DOScale(0, 0.1f);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void SettingGUIByType(string type)
    {
        switch (type)
        {
            case "Audio":
                groupHowTo.SetActive(false);
                groupAudio.SetActive(true);
                UpdateAudioGUI();
                break;
            case "HowTo":
                groupAudio.SetActive(false);
                groupHowTo.SetActive(true);
                index = 0;
                UpdateHowToGUI();
                imgHowToPop.enabled = false;
                break;
            default:
                break;
        }
    }

    void UpdateAudioGUI()
    {
        sliderMaster.value = SoundManager.Instance.masterVolume;
        sliderBgm.value =  SoundManager.Instance.bgmVolume;
        sliderSfx.value = SoundManager.Instance.sfxVolume;
    }

    // 나중에 인스펙터 바인딩이 아니라 코드 바인딩으로 변경
    public void SetMasterValue()
    {
        SoundManager.Instance.masterVolume = sliderMaster.value;
    }
    public void SetBgmValue()
    {
        SoundManager.Instance.bgmVolume = sliderBgm.value;
    }
    public void SetSfxValue()
    {
        SoundManager.Instance.sfxVolume = sliderSfx.value;
    }

    private int index;
    
    private void UpdateHowToGUI()
    {
        imgHowTo.sprite = spritesHowTo[index];
    }
    
    // 나중에 인스펙터 바인딩이 아니라 코드 바인딩으로 변경
    public void NextButton()
    {
        if(index >= spritesHowTo.Length - 1) 
            index = 0;
        else
            index++;
        
        UpdateHowToGUI();
    }

    public void PrevButton()
    {
        if(index <= 0) 
            index = spritesHowTo.Length - 1;
        else
            index--;
        
        UpdateHowToGUI();
    }

    public void PopHowToImage()
    {
        if (!imgHowToPop.enabled)
        {
            imgHowToPop.enabled = true;
            imgHowToPop.sprite = spritesHowTo[index];
        }
        else
        {
            imgHowToPop.enabled = false;
        }
            
    }
}
