using UnityEngine;
/// <summary>
/// 해당 오브젝트를 Inspect(조사, 설명 보기 등)할 수 있도록 만드는 컴포넌트.
/// ItemObject.cs 하나로 사용하기 위해 생성
/// 예: 인벤토리에서 아이템 설명 보기, 월드 오브젝트에 마우스 오버 시 정보 표시 등.
/// </summary>
public class InspectableBehaviour : MonoBehaviour, IInspectable,  IInitializable<ItemData>
{
    private ItemData data;

    public void Initialize(ItemData itemData)
    {
        data = itemData;
    }
    
    public string GetPromptText()
    {
        string promptText =  $"\n<size=50>{data.displayName}</size>\n<size=36>{data.description}</size>";
        return promptText;
    }
}
