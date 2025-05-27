using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPopup : MonoBehaviour, IGUI
{
    private IGUI iguiImplementation;
    public GameObject GUIObject => gameObject;

    public void Initialization()
    {
        iguiImplementation.Initialization();
    }

    public void Open()
    {
        iguiImplementation.Open();
    }

    public void Close()
    {
        iguiImplementation.Close();
    }
}
