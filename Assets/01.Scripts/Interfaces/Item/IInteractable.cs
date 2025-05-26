/// <summary>
/// [IInteractable] E 키 상호작용
/// 아이템 줍기, 상자 열기 등
/// </summary>
public interface IInteractable : IInspectable
{
    public void OnInteract();
}