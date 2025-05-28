using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GUIItemSlotInventory : GUIItemSlotBase
{
    [SerializeField] private GameObject selectedCover;
    [SerializeField] private TextMeshProUGUI tmpPcs;
    [SerializeField] private Button btnSelect;
    [SerializeField] private int index;

    void Reset()
    {
        imgIcon = transform.Find("Img_Icon_Slot").GetComponent<Image>();
        selectedCover = transform.Find("Img_SlotBG_Selected")?.gameObject;
        tmpPcs = transform.Find("Tmp_Pieces")?.GetComponent<TextMeshProUGUI>();
        btnSelect = GetComponent<Button>();
    }
    
    public override void Initialization()
    {
        selectedCover.SetActive(false);
        imgIcon.gameObject.SetActive(false);
        tmpPcs.text = string.Empty;
    }

    public override void Show(Sprite icon, int pcs = 0)
    {
        imgIcon.gameObject.SetActive(true);
        imgIcon.sprite = icon;
        SetPcs(pcs);
    }

    public void SetPcs(int pcs = 0)
    {
        tmpPcs.text = pcs > 0 ? pcs.ToString() : string.Empty;
    }

    public override void Select()
    {
        selectedCover.SetActive(true);
    }

    public void SetClickEvent(UnityAction<int> callback, int slotIndex)
    {
        index = slotIndex;
        btnSelect.onClick.RemoveAllListeners();
        btnSelect.onClick.AddListener(() => callback(index));
    }
}
