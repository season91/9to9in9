using UnityEngine;
/// <summary>
/// 플레이어나 오브젝트가 상호작용할 수 있는 기능을 제공하는 컴포넌트.
/// ItemObject.cs 하나로 사용하기 위해 생성
/// 예: 문 열기, 장치 작동, 아이템 줍기 등.
/// </summary>
public class InteractableBehaviour : MonoBehaviour, IInteractable,  IInitializable<ItemData>
{
    private ItemData data;

    public void Initialize(ItemData itemData)
    {
        data = itemData;
    }
    
    public string GetPromptText()
    {
        string promptText =  $"\n{data.displayName}\n{data.description}";
        return promptText;
    }

    public void OnInteract()
    {
        // Add Inventory
        Destroy(gameObject);
    }
}
