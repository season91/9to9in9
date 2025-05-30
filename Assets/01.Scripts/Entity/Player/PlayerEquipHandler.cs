using System;
using UnityEngine;

public class PlayerEquipHandler : MonoBehaviour
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

    
    public void UpdateStat(EquipableItemData item, float value)
    {
        switch (item.equipType)
        {
            case EquipType.Armor:
                CharacterManager.Player.statHandler.ModifyBonus(StatType.DefensePower, value);
                UIManager.Instance.UpdateStatUI(StatType.DefensePower);
                break;
            
            case EquipType.Weapon:
                CharacterManager.Player.statHandler.ModifyBonus(StatType.AttackPower, value);
                UIManager.Instance.UpdateStatUI(StatType.AttackPower);
                break;
            
            case EquipType.GatheringTool:
                break;
            
            default:
                Debug.Log($"{item.equipType}: 타입이 잘못됐어요!!!");
                break;
        }
    }

    public void UseEquippedWeapon()
    {
        if (curEquip == null) return;
        
        if (curEquip.TryGetComponent(out WeaponHandler weaponHandler))
        {
            weaponHandler.OnAttackInput();
        }
    }
}
