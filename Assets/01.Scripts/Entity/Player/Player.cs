
using UnityEngine;

/// <summary>
/// IDamageble, IMoveable, IJumpable, IAttackable
/// </summary>
public class Player : Entity, IDamagable
{
    public PlayerController playerController { get; private set; }
    public PlayerInventoryController inventoryController { get; private set; }
    public StatHandler statHandler;
    
    [SerializeField] private StatProfile statProfile;
    
    private void Awake()
    {
        //getcomponent에서 null 반환 시 addcomponent할 것
        CharacterManager.Register(this);
        playerController = GetComponent<PlayerController>();
        inventoryController = GetComponent<PlayerInventoryController>();
        statHandler = GetComponent<StatHandler>();
        
        // null체크
        if (playerController == null) Debug.LogError("PlayerController not found");
        if (inventoryController == null) Debug.LogError("InventoryController not found");
        if (statHandler == null) Debug.LogError("Player StatHandler not found");
        if (statProfile == null) Debug.LogError("StatProfile not found");
    }

    private void Start()
    {
        statHandler.Initialize(statProfile.ToDictionary());
        SpawnManager temp = SpawnManager.Instance;
    }

    public void TakeDamage(float damage)
    {
        // TODO: 피해입는 로직 작성
    }

    public void Die()
    {
        Debug.Log("플레이어 사망!");
    }
}
