using System;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    
    private CanvasGroup dialogueCanvasGroup; // 알파 조절을 위한 CanvasGroup
    public TextMeshProUGUI dialogueText;
    public string[] dialogueLines;

    private int currentLine = 0;
    private bool isPlayerInRange = false;
    private bool isTyping = false;
    private Tween typingTween;

    private NPCDialogue currentNPCDialogue; // 현재 말 걸고 있는 NPC

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    private void Start()
    {
        dialogueCanvasGroup.alpha = 0f;
        dialogueCanvasGroup.interactable = false;
        dialogueCanvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        if (currentNPCDialogue != null && Input.GetKeyDown(KeyCode.Return))
        {
            if (dialogueCanvasGroup.alpha == 0f)
            {
                ShowDialogue();
            }
            else
            {
                if (isTyping)
                {
                    typingTween.Kill();
                    dialogueText.maxVisibleCharacters = dialogueLines[currentLine].Length;
                    isTyping = false;
                }
                else
                {
                    NextLine();
                }
            }
        }
    }


    private void ShowDialogue()
    {
        currentLine = 0;
        dialogueCanvasGroup.DOFade(1f, 0.5f).OnComplete(() =>
        {
            dialogueCanvasGroup.interactable = true;
            dialogueCanvasGroup.blocksRaycasts = true;
            PlayTypingEffect(dialogueLines[currentLine]);
        });
    }

    private void NextLine()
    {
        currentLine++;
        if (currentLine < dialogueLines.Length)
        {
            PlayTypingEffect(dialogueLines[currentLine]);
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        dialogueCanvasGroup.DOFade(0f, 0.5f).OnComplete(() =>
        {
            dialogueCanvasGroup.interactable = false;
            dialogueCanvasGroup.blocksRaycasts = false;
            dialogueText.text = "";
            currentNPCDialogue = null;
        });
    }

    private void PlayTypingEffect(string line)
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

    public void StartDialogue(NPCDialogue npcDialogue)
    {
        currentNPCDialogue = npcDialogue;
        dialogueLines = npcDialogue.dialogueLines;
        currentLine = 0;
    }
}
