using System;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [TextArea(2, 5)]
    public string[] dialogueLines; // NPC 대사들

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.instance.StartDialogue(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.instance.EndDialogue();
        }
    }
}