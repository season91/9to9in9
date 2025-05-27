using UnityEngine;
/// <summary>
/// [요청 처리] 캐릭터 리소스 관리
/// </summary>
public class PlayerStatHandler : MonoBehaviour
{
    [SerializeField] private Stat health;
    [SerializeField] private Stat stamina;
    
    private void Start()
    {
        health.Init(100f, 100f, 0f);
        stamina.Init(100f, 100f, 1f);
    }
    
    private void Update()
    {
        RegenerateStamina();
    }
        
    private void RegenerateStamina()
    {
        if (stamina.CurValue < stamina.MaxValue)
            stamina.Change(stamina.PassiveValue * Time.deltaTime);
    }  

    // Die인 상태만 전달해주면 됨
    public void TakeDamage(float amount)
    {
        health.Change(-amount);
        if (health.IsEmpty)
            Debug.Log("플레이어 사망!");
    }
}
