using System;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
     public static DialogueManager instance;
     private string[] dialogueLines;
     private int currentLine = 0;
     private NPCDialogue currentNPCDialogue; // 현재 말 걸고 있는 NPC
 
     private void Awake()
     {
         if (instance == null)
         {
             instance = this;
         }
     }
     
     private void Update()
     {
         if (currentNPCDialogue != null && Input.GetKeyDown(KeyCode.Return))
         {
             if (!UIManager.Instance.DialoguePopupActive()) return;
             if (UIManager.Instance.IsTyping())
             {
                 UIManager.Instance.SkipTyping(dialogueLines[currentLine]);
             }
             else
             {
                 NextLine();
             }
         }
     }
 
     private void NextLine()
     {
         currentLine++;
         if (currentLine < dialogueLines.Length)
         {
             UIManager.Instance.PlayTypingEffect(dialogueLines[currentLine]);
         }
         else
         {
             EndDialogue();
         }
     }
 
     public void EndDialogue()
     {
         UIManager.Instance.HideDialogue();
         currentNPCDialogue = null;
     }
 
     public void StartDialogue(NPCDialogue npcDialogue)
     {
         currentNPCDialogue = npcDialogue;
         dialogueLines = npcDialogue.dialogueLines;
         currentLine = 0;
         
         UIManager.Instance.FirstDialogue(dialogueLines[currentLine]);
     }
     
}
