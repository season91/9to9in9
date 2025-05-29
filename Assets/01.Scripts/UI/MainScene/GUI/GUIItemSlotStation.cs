using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GUIItemSlotStation : GUIItemSlotBase
{
    private bool isEmpty = true;
    [SerializeField] private TextMeshProUGUI tmpTitle;
    [SerializeField] private TextMeshProUGUI tmpPcs;
    [SerializeField] private Button btnSelect;
    
    public bool IsEmpty() => isEmpty;

    void Reset()
    {
        imgIcon = transform.Find("Img_Icon_Slot")?.GetComponentInChildren<Image>();
        tmpTitle = transform.Find("Tmp_Title")?.GetComponentInChildren<TextMeshProUGUI>();
        tmpPcs = transform.Find("Tmp_Pieces")?.GetComponentInChildren<TextMeshProUGUI>();
        btnSelect = GetComponent<Button>();
    }

    public override void Initialization()
    {
        imgIcon.gameObject.SetActive(false);
        tmpTitle.text = string.Empty;
        tmpPcs.text = string.Empty;
        isEmpty = imgIcon.sprite == null;
        
        btnSelect.onClick.RemoveAllListeners();
        btnSelect.onClick.AddListener(Select);
    }

    public override void Show(Sprite icon, int pcs = 0)
    {
        imgIcon.gameObject.SetActive(true);
        imgIcon.sprite = icon;
        int pieces = Convert.ToInt32(tmpPcs.text);
        pieces += pcs;
        tmpPcs.text = pieces.ToString().Trim();
    }

    public override void Select()
    {
        int pieces = Convert.ToInt32(tmpPcs.text);

        if (pieces == 1)
        {
            Initialization();
            return;
        }
        
        if (pieces <= 0 || tmpPcs.text == null)
        {
            Debug.Log("This Slot is Empty");
            return;
        }
        
        pieces--;
        tmpPcs.text = pieces.ToString().Trim();
    }

    public void SetTitle(string title)
    {
        tmpTitle.text = title;
    }
}
