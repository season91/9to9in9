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

        int pieces = 0;
        if (tmpPcs.text == string.Empty)
        {
            pieces += pcs;
        }
        else
        {
            pieces = Convert.ToInt32(tmpPcs.text);
            pieces += pcs;
        }
        tmpPcs.text = pieces.ToString().Trim();

        itemData = item;
    }

    public override void Select()
    {
        if (tmpPcs.text == string.Empty)
        {
            Debug.Log("This Slot is Empty");
        }
        else
        {
            int pieces = Convert.ToInt32(tmpPcs.text);
            
            switch (pieces)
            {
                case <= 0:
                    Debug.Log("This Slot is Empty");
                    return;
                case 1:
                    Initialization();
                    return;
                default:
                    pieces--;
                    tmpPcs.text = pieces.ToString().Trim();
                    break;
            }

            CharacterManager.Player.inventoryController.AddItem(itemData);
        }
    }

    public void SetTitle(string title)
    {
        tmpTitle.text = title;
    }
}
