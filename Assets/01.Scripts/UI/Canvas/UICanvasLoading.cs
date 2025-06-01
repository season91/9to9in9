using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum LoadType
{
    Scene,
    GUI,
    Resource
}

public class UICanvasLoading : MonoBehaviour, IGUI
{
    [SerializeField] private Image imgProgress;
    [SerializeField] private TextMeshProUGUI tmpLoadingTitle;
    [SerializeField] private TextMeshProUGUI tmpLoadingStatus;
    [SerializeField] private float maxProgressSize;
    [SerializeField] private Image imgLoading;
    [SerializeField] private Sprite[] spriteLoadings; // 직접 참조

    private Coroutine coroutine;
    private IGUI iguiImplementation;

    public GameObject GUIObject => gameObject;

    private void Reset()
    {
        imgProgress = transform.Find("Img_ProgressBar").GetComponent<Image>();
        tmpLoadingTitle = transform.Find("Tmp_ProgressTitle").GetComponent<TextMeshProUGUI>();
        tmpLoadingStatus = transform.Find("Tmp_ProgressStatus").GetComponent<TextMeshProUGUI>();
        maxProgressSize = imgProgress.rectTransform.sizeDelta.x;
        imgLoading = transform.Find("Img_Loading").GetComponent<Image>();
    }

    public void Initialization()
    {
        SetProgressBar(0);
        tmpLoadingTitle.text = string.Empty;
        tmpLoadingStatus.text = string.Empty;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        coroutine = StartCoroutine(UpdateLoadingImage());
    }
    
    public void Close()
    {
        StopCoroutine(coroutine);
        gameObject.SetActive(false);
    }
    
    public void SetProgressTitle(LoadType type)
    {
        string text = string.Empty;
        switch (type)
        {
            case LoadType.GUI:
                text = "세계 꾸미는 중...";
                break;
            case LoadType.Resource:
                text = "세계 구축 중...";
                break;
            case LoadType.Scene:
                text = "세계 속으로 들어가는 중...";
                break;
            default:
                text = "뭐하는 지 모름...";
                break;
        }

        tmpLoadingTitle.text = text;
    }

    public void SetProgressStatus(string value)
    {
        tmpLoadingStatus.text = value;
    }
    
    public void SetProgressBar(float normalizedValue)
    {
        float width = Mathf.Clamp01(normalizedValue) * maxProgressSize;

        if (imgProgress != null)
        {
            imgProgress.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }
    }

    IEnumerator UpdateLoadingImage()
    {
        while (gameObject.activeSelf)
        {
            int ranIndex = Random.Range(0, spriteLoadings.Length);
            
            imgLoading.sprite = spriteLoadings[ranIndex];
            yield return new WaitForSeconds(2f);
        }
    }
}
