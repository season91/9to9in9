using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class GUIItemSlotBase : MonoBehaviour
{
    [SerializeField] protected bool isEmpty = true;
    [SerializeField] protected Image imgIcon;
    // public ItemData itemData;
    
    //imgIcon = transform.Find("Img_Icon_Slot").GetComponent<Image>();
    
    public abstract void Initialization();

    public abstract void Show(Sprite icon, int pcs = 0);
    public abstract void Select();
    
    public bool IsEmpty() => isEmpty;
}
