using UnityEngine;

/// <summary>
/// [IInspectable] 설명만 표시
/// 나무, 간판 등
/// </summary>
public interface IInspectable
{
    // 상호작용 대상에 가까이 갔을 때 UI에 표시할 설명
    public string GetPromptText();
}