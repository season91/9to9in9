using System;
using UnityEngine;
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
        
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            string path = $"Item/Data/{type}";
            var allItems = Resources.LoadAll<ItemData>(path);
            foreach (var item in allItems)
            {
                if (item.name == name)
                {
                    itemData = item;
                    found = true;
                    return;
                }
            }
        }
        
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
}
