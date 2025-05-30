
using UnityEngine;

/// <summary>
/// IDamageble, IMoveable, IJumpable, IAttackable
/// </summary>
public class Player : Entity, IDamagable
{
    public PlayerController controller { get; private set; }
    public PlayerInventoryController inventoryController { get; private set; }
    public PlayerInteractionHandler interactionHandler { get; private set; }
    public StatHandler statHandler { get; private set; }
    public PlayerEquipHandler equipHandler { get; private set; }
    
    [SerializeField] private StatProfile statProfile;
    
    private void Awake()
    {
        //getcomponent에서 null 반환 시 addcomponent할 것
        CharacterManager.Register(this);
        controller = GetComponent<PlayerController>();
        inventoryController = GetComponent<PlayerInventoryController>();
        statHandler = GetComponent<StatHandler>();
        interactionHandler = GetComponent<PlayerInteractionHandler>();
        equipHandler = GetComponent<PlayerEquipHandler>();
        
        // null체크
        if (controller == null) Debug.LogError("Player Controller not found");
        if (inventoryController == null) Debug.LogError("Player InventoryController not found");
        if (statHandler == null) Debug.LogError("Player StatHandler not found");
        if (interactionHandler == null) Debug.LogError("Player InteractionHandler not found");
        if (equipHandler == null) Debug.LogError("Player EquipHandler not found");
        
        if (statProfile == null) Debug.LogError("Player StatProfile not found");
    }

    private void Start()
    {
        statHandler.Initialize(statProfile.ToDictionary());
        SpawnManager temp = SpawnManager.Instance;
    }

    public void TakeDamage(float damage)
    {
        statHandler.Modify(StatType.Health, -damage);
        UIManager.Instance.UpdateStatUI(StatType.Health);
    }

    public void Die()
    {
        Debug.Log("플레이어 사망!");
    }
}
