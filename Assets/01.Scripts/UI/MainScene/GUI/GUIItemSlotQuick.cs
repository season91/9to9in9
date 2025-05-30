using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GUIItemSlotQuick : GUIItemSlotBase, IPointerClickHandler
{
    // [SerializeField] private Image imgSelected;
    [SerializeField] private TextMeshProUGUI tmpPcs;
    // [SerializeField] private ItemData curItemData;
    private int itemIndex;

    public void SetItemIndex(int index) => itemIndex = index;
    
    private void Reset()
    {
        imgIcon = transform.Find("Img_Icon_Slot")?.GetComponent<Image>();
        // imgSelected = transform.Find("Img_SlotBG_Selected")?.GetComponent<Image>();
        tmpPcs = transform.Find("Tmp_Pieces")?.GetComponent<TextMeshProUGUI>();
    }

    public override void Initialization()
    {
        // imgSelected.enabled = false;
        tmpPcs.text = string.Empty;
        imgIcon.gameObject.SetActive(false);
    }

    public override void Show(Sprite icon, int pcs = 0, ItemData itemData = null)
    {
        imgIcon.gameObject.SetActive(true);
        imgIcon.sprite = icon;
        SetPcs(pcs);
        
        // curItemData = itemData;
    }

    public override void Select()
    {
        // imgSelected.enabled = true;
        // 이 아이템 슬롯의 아이템이 소모형이면 소모
        // if (curItemData.type != ItemType.Consumable)
        //     return;
        
        // UseItemInQuickSlot()
        // curItemData.
    }
    

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            CharacterManager.Player.inventoryController.MoveItemFromQuickSlot(itemIndex);
        }
    }
    
    void SetPcs(int pcs = 0)
    {
        tmpPcs.text = pcs > 1 ? pcs.ToString() : string.Empty;
    }
}
