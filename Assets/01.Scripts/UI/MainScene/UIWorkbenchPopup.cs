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
    
    public GameObject GUIObject => gameObject;
    
    public void Initialization()
    {
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
