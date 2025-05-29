using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 아이템 프리팹에 붙는 공통 스크립트
/// 해당 오브젝트가 어떤 아이템 데이터를 기반으로 생성되었는지 추적
/// 필요한 기능 컴포넌트(Inspectable, Interactable, Damagable 등)를 자동으로 부착
/// </summary>
public class ItemObject : MonoBehaviour
{
    public ItemData itemData;

    private void Reset()
    {
        bool found = false;
        
        string itemKey = name; // 혹은 $"{itemType}/{name}" 형태로 키 구성도 가능

        var handle = Addressables.LoadAssetAsync<ItemData>(itemKey);

        try
        {
            if (itemData != null)
            {
                Debug.Log($"[ItemObject] {itemKey} 로드 성공");
            }
            else
            {
                Debug.LogWarning($"[ItemObject] {itemKey}에 해당하는 ItemData가 없습니다.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ItemObject] {itemKey} 로드 실패: {e.Message}");
        }
        
        // foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        // {
        //     string path = $"Item/Data/{type}";
        //     var allItems = Resources.LoadAll<ItemData>(path);
        //     foreach (var item in allItems)
        //     {
        //         if (item.name == name)
        //         {
        //             itemData = item;
        //             found = true;
        //             return;
        //         }
        //     }
        // }
        
        // 예외 처리
        if (!found)
        {
            Debug.LogWarning($"[ItemObject] '{name}'에 해당하는 ItemData를 찾을 수 없습니다. SO 확인 필요!", this);
            itemData = null;
        }
    }

    private void Start()
    {
        if (itemData == null)
        {
            Debug.LogError($"ItemObject-{name} Inspector Reset 누르세요!!!!!");
            return;
        }

        if (itemData.functions == null || itemData.functions.Length == 0)
        {
            Debug.LogError($"ItemObject-{name} csv functions 파싱 제대로 안됐어요!!!!!");
            return;
        }

        foreach (var function in itemData.functions)
        {
            switch (function)
            {
                case ItemFunction.Inspectable:
                    if (!TryGetComponent<IInspectable>(out _))
                        gameObject.AddComponent<InspectableBehaviour>().Initialize(itemData);
                    break;

                case ItemFunction.Interactable:
                    if (!TryGetComponent<IInteractable>(out _))
                        gameObject.AddComponent<InteractableBehaviour>().Initialize(itemData);
                    break;
                
                case ItemFunction.Damageable:
                    if (!TryGetComponent<IDamagable>(out _))
                        gameObject.AddComponent<DamageableBehaviour>().Initialize(itemData as BuildItemData);
                    break;

                default:
                    Debug.LogWarning($"[ItemObject] Unknown function: {function}");
                    break;
            }
        }
    }

    /// <summary>
    /// 퀵슬롯에서 현재 선택된 칸의 아이템 데이터를 받아와서 사용 처리
    /// 퀵슬롯에서는 소비 / 건축 아이템만 사용
    /// </summary>
    /// <param name="itemData">ItemData</param>
    private void UseItem(ItemData itemData)
    {
        switch (itemData.type)
        {
            case ItemType.Build:
                BuildManager.Instance.EnterBuildMode(itemData as BuildItemData);
                break;
            
            case ItemType.Consumable:
                UseConsumableItem(itemData as ConsumableItemData);
                break;
        }
    }
    
    /// <summary>
    /// 음식 먹기 
    /// </summary>
    /// <param name="item">ConsumableItemData</param>
    private void UseConsumableItem(ConsumableItemData item)
    {
        for (int i = 0; i < item.consumableTypes.Length; i++)
        {
            float value = item.amounts[i];
            switch (item.consumableTypes[i])
            {
                case ConsumableType.Health:
                    CharacterManager.Player.statHandler.Modify(StatType.Health, value);
                    break;
                
                case ConsumableType.Hunger:
                    CharacterManager.Player.statHandler.Modify(StatType.Hunger, value);
                    break;
                
                case ConsumableType.Stamina:
                    CharacterManager.Player.statHandler.Modify(StatType.Stamina, value);
                    break;
            }
        }
    }

    #region 오른손 무기 처리 로직 (이동 예정)

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
                // TODO: 휘두르기
                break;
            
            case EquipType.GatheringTool:
                // TODO: 채집하기
                break;
        }
    }
    
    // curEquip, equipParent 선언해야됨
    private void Equip(ItemData item)
    {
        UnEquip();
        // curEquip = Instantiate(item.prefab, equipParent);
        
        Debug.Log($"[ItemObject-{name}] 장착됨 !");
    }

    private void UnEquip()
    {
        //Destroy(curEquip.gameObject);
        //cureEquip = null
        Debug.Log($"[ItemObject-{name}] 해제됨 !");
    }

    #endregion
    
}
