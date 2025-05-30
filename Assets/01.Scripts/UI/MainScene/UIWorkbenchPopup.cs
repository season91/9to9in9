using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorkbenchPopup : MonoBehaviour, IGUI
{
    // CraftType 
    // 조합
    // bool 제작할 수 있는가?
    // 조합법 
    // 모든 레시피 아이콘과 해당 아이콘이 제작할 수 있는가?
    // 딕셔너리  key: sprite, value: bool
    // sprite 
    // int형으로 index를
    // 모루 (type, 어떤 타입(장비,도구), index)
    
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
    public float CanvasAlpha() => canvasGroup.alpha;
#endif
}
