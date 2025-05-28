/// <summary>
/// [IInteractable] E 키 상호작용
/// 아이템 줍기, 상자 열기 등
/// </summary>
public interface IInteractable : IInspectable
{
    // 상호작용 E 키 누름으로 인해 아이템 습득시
    public void OnInteract();
}