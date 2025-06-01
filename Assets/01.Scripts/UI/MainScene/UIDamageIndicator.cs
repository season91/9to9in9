using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDamageIndicator : MonoBehaviour, IGUI
{
    [SerializeField] private CanvasGroup canvasGroup; // 알파 조절을 위한 CanvasGroup
    [SerializeField] private Image damagedImage;
    [SerializeField] private TextMeshProUGUI tmpDie;
    
    public GameObject GUIObject => gameObject;

    // [SerializeField] private float damageRate = 1f;
    
    void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        damagedImage = GetComponent<Image>();
        tmpDie = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    public void Initialization()
    {
        damagedImage.color = new Color(171 / 255f, 0, 0, 150 / 255f);
        tmpDie.gameObject.SetActive(false);
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

    // 플레이어 피격
    public void PlayDamageEffect(float duration)
    {
        Open();
        canvasGroup.DOFade(0f, duration);
    }


    // 플레이어 사망
    public IEnumerator DieScreenAnim()
    {
        tmpDie.alpha = 0;
        damagedImage.color = Color.black;
        
        canvasGroup.DOFade(1f, 1f).SetUpdate(true);
        
        yield return new WaitForSecondsRealtime(0.5f);
        tmpDie.DOFade(1f, 0.5f).SetUpdate(true);
        
        yield return new WaitForSecondsRealtime(1f);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
