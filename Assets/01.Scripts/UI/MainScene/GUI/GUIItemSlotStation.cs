using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GUIItemSlotStation : GUIItemSlotBase
{
    [SerializeField] private TextMeshProUGUI tmpTitle;
    [SerializeField] private TextMeshProUGUI tmpPcs;
    [SerializeField] private Button btnSelect;
    [SerializeField] private ItemData itemData;

    public bool IsEmpty => !itemData;
    
    public bool IsPlacePossible(Sprite icon)
    {
        if(!imgIcon.sprite)
            return true;
        
        return imgIcon.sprite == icon;
    }

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
        imgIcon.sprite = null;
        tmpPcs.text = string.Empty;
        itemData = null;
        
        btnSelect.onClick.RemoveAllListeners();
        btnSelect.onClick.AddListener(Select);
    }

    /// <summary>
    /// pcs 값 넣어주세요! 제거하는 거면 -1, 추가하는 거면 1
    /// </summary>
    /// <param name="icon">아이템 아이콘</param>
    /// <param name="pcs">아이템 개수 -1 or 1</param>
    public override void Show(Sprite icon, int pcs, ItemData item)
    {
        imgIcon.gameObject.SetActive(true);
        imgIcon.sprite = icon;

        SetPcs(pcs);

        itemData = item;
    }

    public override void Select()
    {
        if (!int.TryParse(tmpPcs.text, out int pieces))
        {
            MyDebug.Log("This Slot is Empty");
            return;
        }

        switch (pieces)
        {
            case <= 0:
                MyDebug.Log("This Slot is Empty");
                break;
            case 1:
                CharacterManager.Player.inventoryController.AddItem(itemData);
                Initialization();
                break;
            default:
                CharacterManager.Player.inventoryController.AddItem(itemData);
                SetPcs(-1);
                break;
        }
    }

    public void SetTitle(string title)
    {
        tmpTitle.text = title;
    }

    public void SetImageToSilhouette(bool isGetPossible)
    {
        // ItemSlot (Equip) 보면 실루엣 스프라이트 컬러 기준 있음
        imgIcon.color = isGetPossible? Color.white : new Color(0, 0, 0, 0.509804f);
    }
    
    public void SetPcs(int pcs)
    {
        if (!int.TryParse(tmpPcs.text, out int pieces))
        {
            pieces = 0;
        }

        pieces += pcs;

        if (pieces <= 0)
        {
            Initialization();
            return;
        }

        tmpPcs.text = pieces.ToString().Trim();
    }
}
