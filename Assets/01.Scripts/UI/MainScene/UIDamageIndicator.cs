using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDamageIndicator : MonoBehaviour, IGUI
{
    [SerializeField] private CanvasGroup canvasGroup; // 알파 조절을 위한 CanvasGroup
    [SerializeField] private Image damagedImage;
    
    public GameObject GUIObject => gameObject;

    private Coroutine coroutine;
    [SerializeField] private float damageRate = 1f;
    
    void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void Initialization()
    {
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

    public void DamageIndicate()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(DamageEffect());
    }

    IEnumerator DamageEffect()
    {
        canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutQuad);
        yield return null;
    }
}
