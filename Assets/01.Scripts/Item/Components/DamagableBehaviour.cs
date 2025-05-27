using UnityEngine;
/// <summary>
/// 공격이나 충돌 등으로부터 피해를 받을 수 있는 기능을 제공하는 컴포넌트
/// ItemObject.cs 하나로 사용하기 위해 생성
/// 예: 적 캐릭터, 파괴 가능한 오브젝트, 데미지를 받는 구조물 등.
/// </summary>
public class DamagableBehaviour : MonoBehaviour, IDamagable, IInitializable<ItemData>
{
    private ItemData data;

    public void Initialize(ItemData itemData)
    {
        data = itemData;
    }
    
    public void TakeDamage()
    {
        
    }
}
