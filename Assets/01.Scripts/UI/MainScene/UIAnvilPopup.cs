using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnvilPopup : MonoBehaviour, IGUI
{
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
