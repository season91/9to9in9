using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIDialoguePopup : MonoBehaviour, IGUI
{
    [SerializeField] private CanvasGroup dialogueCanvasGroup; // 알파 조절을 위한 CanvasGroup
    [SerializeField] private TextMeshProUGUI dialogueText;
    
    public GameObject GUIObject => gameObject;
    public bool IsTyping { get; private set; }
    
    // 두투윈 관련 변수
    private int currentLine = 0;
    private bool isPlayerInRange = false;
    private bool isTyping = false;
    private Tween typingTween;
    
    void Reset()
    {
        dialogueCanvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void Initialization()
    {
        Close();
    }

    public void Open()
    {
        dialogueCanvasGroup.alpha = 1f;
        dialogueCanvasGroup.interactable = true;
        dialogueCanvasGroup.blocksRaycasts = true;
    }

    public void Close()
    {
        dialogueCanvasGroup.alpha = 0f;
        dialogueCanvasGroup.interactable = false;
        dialogueCanvasGroup.blocksRaycasts = false;
    }
    
    public void FirstDialogue(string line)
    {
        dialogueText.text = "";
        currentLine = 0;
        dialogueCanvasGroup.DOFade(1f, 0.5f).OnComplete(() =>
        {
            dialogueCanvasGroup.interactable = true;
            dialogueCanvasGroup.blocksRaycasts = true;
            PlayTypingEffect(line);
        });
    }
    
    // 대화 중
    public void PlayTypingEffect(string line)
    {
        isTyping = true;
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;

        typingTween = DOTween.To(
            () => dialogueText.maxVisibleCharacters,
            x => dialogueText.maxVisibleCharacters = x,
            line.Length,
            0.05f * line.Length
        ).OnComplete(() =>
        {
            isTyping = false;
        });
    }
    
    public void SkipTyping(string fullText)
    {
        if (typingTween != null && typingTween.IsActive())
        {
            typingTween.Kill();
            dialogueText.maxVisibleCharacters = fullText.Length;
            IsTyping = false;
        }
    }

    public void HideDialogue()
    {
        dialogueCanvasGroup.DOFade(0f, 0.5f).OnComplete(() =>
        {
            dialogueCanvasGroup.interactable = false;
            dialogueCanvasGroup.blocksRaycasts = false;
            dialogueText.text = "";
        });
    }
}
