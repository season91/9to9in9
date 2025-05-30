using UnityEngine;

public class PlayerEquip : MonoBehaviour
{
    [SerializeField] private GameObject curEquip;
    [SerializeField] private Transform equipParent;

    private void Reset()
    {
        equipParent = GameObject.Find("Equip Camera").transform;
    }
    
    
    public void Equip(ItemData item)
    {
        UnEquip();
        curEquip = Instantiate(item.prefab, equipParent);
        
        Debug.Log($"[ItemObject-{name}] 장착됨 !");
    }

    
    public void UnEquip()
    {
        if (!curEquip) return;
        
        Destroy(curEquip.gameObject);
        curEquip = null;
        Debug.Log($"[ItemObject-{name}] 해제됨 !");
    }
    
    /// <summary>
    /// 오른손에 든 거 사용하기
    /// 타입에 따라 switch (공격 / 채집)
    /// </summary>
    /// <param name="item">EquipableItemData</param>
    private void UseEquipableItem(EquipableItemData item)
    {
        switch (item.equipType)
        {
            case EquipType.Weapon:
                UseWeapon();
                break;
            
            case EquipType.GatheringTool:
                UseGatheringTool();
                break;
        }
    }

    
    private void UseWeapon()
    {
        
    }

    
    private void UseGatheringTool()
    {
        
    }
}
