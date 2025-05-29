
using UnityEngine;

/// <summary>
/// IDamageble, IMoveable, IJumpable, IAttackable
/// </summary>
public class Player : Entity
{
    public PlayerController playerController { get; private set; }
    public PlayerInventoryController inventoryController { get; private set; }
    
    [SerializeField] private StatProfile statProfile;
    
    private void Awake()
    {//getcomponent에서 null 반환 시 addcomponent할 것
        CharacterManager.Register(this);
        playerController = GetComponent<PlayerController>();
        if (playerController == null) Debug.LogError("PlayerController not found");
        
        inventoryController = GetComponent<PlayerInventoryController>();
        if (inventoryController == null) Debug.LogError("InventoryController not found");
    }

    private void Start()
    {
        statHandler.Initialize(statProfile.ToDictionary());
        SpawnManager temp = SpawnManager.Instance;
    }

    public override void Die()
    {
        Debug.Log("플레이어 사망!");
    }
}
