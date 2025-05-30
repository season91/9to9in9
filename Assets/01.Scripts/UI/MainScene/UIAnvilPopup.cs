using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnvilPopup : MonoBehaviour, IGUI
{
    [SerializeField] private CanvasGroup canvasGroup;
    
    public GameObject GUIObject => gameObject;

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
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    
    public void Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    
#if UNITY_EDITOR
    public void TestOpen()
    {
        if (canvasGroup.alpha >= 0.5)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
#endif
}
