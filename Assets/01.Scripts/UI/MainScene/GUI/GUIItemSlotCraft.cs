using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GUIItemSlotCraft : GUIItemSlotBase
{
    [SerializeField] private Button btnSelect;
    [SerializeField] private int index;
    [SerializeField] private ItemData itemData;

    public ItemData ItemData() => itemData;
    
    void Reset()
    {
        imgIcon = transform.Find("Img_Icon_Slot")?.GetComponentInChildren<Image>();
        btnSelect = GetComponent<Button>();
    }

    public override void Initialization()
    {
        imgIcon.enabled = false;
    }

    public override void Show(Sprite icon, int pcs = 0, ItemData data = null)
    {
        SetImageToSilhouette(true);
        imgIcon.enabled = true;
        imgIcon.sprite = icon;
        itemData = data;
    }

    public override void Select()
    {
        
    }

    public void SetImageToSilhouette(bool isGetPossible)
    {
        // ItemSlot (Equip) 보면 실루엣 스프라이트 컬러 기준 있음
        imgIcon.color = isGetPossible? Color.white : new Color(0, 0, 0, 0.509804f);
    }

    public void SetClickEvent(UnityAction<int> callback, int slotIndex)
    {
        index = slotIndex;
        btnSelect.onClick.RemoveAllListeners();
        btnSelect.onClick.AddListener(() => callback(index));
    }
}
