using UnityEngine;

public enum EquipmentType
{
    Helmet,
    Chest,
    Leggings,
    Boots,
    Sword,
    Shield
}

public class UIEquipmentPopup : MonoBehaviour, IGUI
{
    // 필요한 것
    // equipment 슬롯
    // 보유 스탯, 스탯 이름
    // 제작 시스템 (제작대 만들 재료 있으면 활성화)
    // 캐릭터 이미지 
    // 인벤토리 클릭 시 장착된 장비 있으면 해제 후 장착
    // 현재 장착 중인 아이템 리스트
    
    [SerializeField] private GUIItemSlotEquipment equipmentHelmet;
    [SerializeField] private GUIItemSlotEquipment equipmentChest;
    [SerializeField] private GUIItemSlotEquipment equipmentLeggings;
    [SerializeField] private GUIItemSlotEquipment equipmentBoots;
    [SerializeField] private GUIItemSlotEquipment equipmentSword;
    [SerializeField] private GUIItemSlotEquipment equipmentShield;
    
    public GameObject GUIObject => gameObject;
    
    void Reset()
    {
        equipmentHelmet = transform.Find("GUI_ItemSlot (Helmet)").GetComponent<GUIItemSlotEquipment>();
        equipmentChest = transform.Find("GUI_ItemSlot (Chest)").GetComponent<GUIItemSlotEquipment>();
        equipmentLeggings = transform.Find("GUI_ItemSlot (Leggings)").GetComponent<GUIItemSlotEquipment>();
        equipmentBoots = transform.Find("GUI_ItemSlot (Boots)").GetComponent<GUIItemSlotEquipment>();
        equipmentSword = transform.Find("GUI_ItemSlot (Sword)").GetComponent<GUIItemSlotEquipment>();
        equipmentShield = transform.Find("GUI_ItemSlot (Shield)").GetComponent<GUIItemSlotEquipment>();
    }
    
    public void Initialization()
    {
        equipmentHelmet.SetClickEvent(OnEquipSlotSelected, EquipmentType.Helmet);
        equipmentChest.SetClickEvent(OnEquipSlotSelected, EquipmentType.Chest);
        equipmentLeggings.SetClickEvent(OnEquipSlotSelected, EquipmentType.Leggings);
        equipmentBoots.SetClickEvent(OnEquipSlotSelected, EquipmentType.Boots);
        equipmentSword.SetClickEvent(OnEquipSlotSelected, EquipmentType.Sword);
        equipmentShield.SetClickEvent(OnEquipSlotSelected, EquipmentType.Shield);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void OnEquipSlotSelected(EquipmentType type)
    {
        // CharacterManager.Player.inventoryController.Unequip(type);
    }
}
