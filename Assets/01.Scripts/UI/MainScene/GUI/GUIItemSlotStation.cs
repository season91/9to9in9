using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GUIItemSlotStation : GUIItemSlotBase
{
    [SerializeField] private TextMeshProUGUI tmpTitle;
    [SerializeField] private TextMeshProUGUI tmpPcs;
    [SerializeField] private Button btnSelect;
    [SerializeField] private ItemData itemData;

    public ItemData ItemData => itemData;
    
    public bool IsEmpty => !itemData;
    
    public bool IsPlacePossible(Sprite icon)
    {
        if(!imgIcon.enabled)
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
        imgIcon.enabled = false;
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
    public override void Show(Sprite icon, int pcs, ItemData data)
    {
        imgIcon.enabled = true;
        imgIcon.sprite = icon;
        SetPcs(pcs);

        itemData = data;
    }

    public override void Select()
    {
        if (!imgIcon.enabled)
        {
            MyDebug.Log("This Slot is Empty");
            return;
        }
        
        CharacterManager.Player.inventoryController.AddItem(itemData);
        SetPcs(-1);
    }

    public void SetTitle(string title)
    {
        tmpTitle.text = title;
    }

    public void SetPcs(int pcs)
    {
        // text가 1일때는 텍스트를 비움
        
        if (!int.TryParse(tmpPcs.text, out int pieces))
        {
            pieces = 1;
        }
        else
        {
            pieces += pcs;
        }
        
        if (pieces <= 0)
        {
            Initialization();
            return;
        }
        
        tmpPcs.text = pieces == 1 ? String.Empty : pieces.ToString().Trim();
    }
    
    public void DiscountPcsOfName(string itemName, int amount)
    {
        if(IsEmpty)
            return;

        if (itemData.itemName == itemName)
        {
            for (int i = 0; i < amount; i++)
            {
                SetPcs(-1);
            }
        }
    }
    
    public int GetPcs()
    {
        if(!imgIcon.enabled)
            return 0;
            
        if (int.TryParse(tmpPcs.text, out int pcs))
        {
            return pcs;
        }
        return 1;
    }
    
    public void SetClickEvent(UnityAction<ItemData, UnityAction> callback)
    {
        btnSelect.onClick.RemoveAllListeners();
        btnSelect.onClick.AddListener(() => callback(ItemData, Select));
    }
}
