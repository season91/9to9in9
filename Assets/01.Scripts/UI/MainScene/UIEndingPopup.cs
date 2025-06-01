using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEndingPopup : MonoBehaviour, IGUI
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI tmpSlideTitle; // 0, 180, 2600
    [SerializeField] private TextMeshProUGUI tmpTeamName;
    
    public GameObject GUIObject => gameObject;

    void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        tmpSlideTitle = transform.Find("Tmp_SlideTitle")?.GetComponent<TextMeshProUGUI>();
        tmpTeamName = transform.Find("Tmp_TeamName")?.GetComponent<TextMeshProUGUI>();
    }
    
    public void Initialization()
    {
        tmpSlideTitle.rectTransform.anchoredPosition = new Vector2(0, -250);
        tmpSlideTitle.alpha = 0f;
        tmpTeamName.alpha = 0f;
        Close();
    }

    public void Open()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Close()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    
    public IEnumerator EndingCreditAnim()
    {
        Initialization();
        yield return null;
        
        canvasGroup.DOFade(1, 0.5f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(0.5f);
        
        tmpSlideTitle.rectTransform.DOAnchorPosY(180, 2f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(0.2f);
        
        tmpSlideTitle.DOFade(1, 1f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(2.5f);
        
        tmpSlideTitle.rectTransform.DOAnchorPosY(2600, 10f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(10f);
        
        tmpTeamName.DOFade(1, 3f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(3f);
        tmpTeamName.DOFade(0, 3f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(3f);

        // 다시 실행
        Time.timeScale = 1;

        DOTween.KillAll();
        _ = UIManager.Instance.OpenScene(SceneType.Start);
    }
}