using UnityEngine;
/// <summary>
/// 제네릭 타입 T를 기반으로 초기화
/// 외부에서 데이터를 초기화해야 하는 경우에 사용
/// </summary>
public interface IInitializable<T>
{
    void Initialize(T data);
}
