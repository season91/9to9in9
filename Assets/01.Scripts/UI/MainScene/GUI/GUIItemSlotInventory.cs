using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GUIItemSlotInventory : GUIItemSlotBase, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI tmpPcs;
    [SerializeField] private Button btnSelect;
    [SerializeField] private int index;

    void Reset()
    {
        imgIcon = transform.Find("Img_Icon_Slot").GetComponent<Image>();
        tmpPcs = transform.Find("Tmp_Pieces")?.GetComponent<TextMeshProUGUI>();
        btnSelect = GetComponent<Button>();
    }
    
    public override void Initialization()
    {
        imgIcon.gameObject.SetActive(false);
        tmpPcs.text = string.Empty;
    }

    public override void Show(Sprite icon, int pcs = 0, ItemData itemData = null)
    {
        imgIcon.gameObject.SetActive(true);
        imgIcon.sprite = icon;
        SetPcs(pcs);
    }

    public void SetPcs(int pcs = 0)
    {
        tmpPcs.text = pcs > 1 ? pcs.ToString() : string.Empty;
    }

    public override void Select()
    {
    }

    public void SetClickEvent(UnityAction<int> callback, int slotIndex)
    {
        index = slotIndex;
        btnSelect.onClick.RemoveAllListeners();
        btnSelect.onClick.AddListener(() => callback(index));
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            CharacterManager.Player.inventoryController.MoveItemToQuickSlot(index);
        }
    }
}
