using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance; // 싱글톤

    public GameObject dialogueUI;
    public TextMeshProUGUI dialogueText;

    private string[] currentLines;
    private int currentLine;
    private bool isTyping;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        dialogueUI.SetActive(false);
    }

    private void Update()
    {
        if (dialogueUI.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentLines[currentLine];
                isTyping = false;
            }
            else
            {
                currentLine++;
                if (currentLine < currentLines.Length)
                {
                    typingCoroutine = StartCoroutine(TypeLine(currentLines[currentLine]));
                }
                else
                {
                    EndDialogue();
                }
            }
        }
    }

    public void StartDialogue(string[] lines)
    {
        dialogueUI.SetActive(true);
        currentLines = lines;
        currentLine = 0;
        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentLine]));
    }

    private void EndDialogue()
    {
        dialogueUI.SetActive(false);
        dialogueText.text = "";
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.03f);
        }
        isTyping = false;
    }
}