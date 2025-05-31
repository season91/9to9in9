using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GUIItemSlotEquipment : GUIItemSlotBase
{
    // 슬롯 클릭시 해제
    // 장착된 게 없다면 실루엣 온
    [SerializeField] private Image imgSilhouette;
    [SerializeField] private Button btnSelect;
    [SerializeField] private EquipSlot type;
    
    private void Reset()
    {
        imgIcon = transform.Find("Img_Icon_Slot")?.GetComponent<Image>();
        imgSilhouette = transform.Find("Img_Icon_Silhouette")?.GetComponent<Image>();
        btnSelect = GetComponent<Button>();
    }

    public override void Initialization()
    {
        imgSilhouette.gameObject.SetActive(true);
        imgIcon.gameObject.SetActive(false);
        btnSelect.onClick.RemoveAllListeners();
        btnSelect.onClick.AddListener(Select);
    }

    public override void Show(Sprite icon, int pcs = 0, ItemData data = null)
    {
        imgSilhouette.gameObject.SetActive(false);
        imgIcon.gameObject.SetActive(true);
        imgIcon.sprite = icon;
    }

    public override void Select()
    {
        if (CharacterManager.Player.inventoryController.UnEquipItem(type))
        {
            Initialization();
        }
        else
        {
            Debug.Log("item can't unequip");
        }
    }
    
    // public void SetClickEvent(UnityAction<EquipSlot> callback)
    // {
    //     btnSelect.onClick.RemoveAllListeners();
    //     btnSelect.onClick.AddListener(() => callback(type));
    //     btnSelect.onClick.AddListener(Select);
    //     // CharacterManager.Player.inventoryController.Unequip(type);
    // }

    public void SetType(EquipSlot equipSlot)
    {
        type = equipSlot;
    }
}
