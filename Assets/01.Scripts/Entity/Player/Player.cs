
using System;
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
    
    private float timer;
    private float hungerPassive;
    private float staminaPassive;
    
    private void Awake()
    {
        //getcomponent에서 null 반환 시 addcomponent할 것
        CharacterManager.Register(this);
        controller = GetComponent<PlayerController>();
        inventoryController = GetComponent<PlayerInventoryController>();
        statHandler = GetComponent<StatHandler>();
        interactionHandler = GetComponent<PlayerInteractionHandler>();
        equipHandler = GetComponent<PlayerEquipHandler>();

        hungerPassive = CharacterManager.Player.statHandler.GetPassive(StatType.Hunger);
        staminaPassive = CharacterManager.Player.statHandler.GetPassive(StatType.Stamina);
        
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

    private void Update()
    {
        // 배고픔 수치 감소
        timer += Time.deltaTime;

        if (!(timer >= 1f)) return;
        
        
        timer = 0f;
        statHandler.Modify(StatType.Hunger, hungerPassive);
        
        bool empty = statHandler.IsEmpty(StatType.Hunger);
        
        // 스태미나 수치 증가
        if (empty) return; // 배고픔 0이면 스태미나 리젠 X
        statHandler.Modify(StatType.Stamina, staminaPassive);
        
        // 1초마다 호출되니까 괜찮을까요... 아니면 수정할게요...
        UIManager.Instance.UpdateStatUI(StatType.Hunger);
        UIManager.Instance.UpdateStatUI(StatType.Stamina);
    }
    
    public void TakeDamage(float damage)
    {
        statHandler.Modify(StatType.Health, -damage);
        SoundManager.Instance.PlayRandomSfx(SfxType.Hit);
        UIManager.Instance.UpdateStatUI(StatType.Health);
    }
    
    public void Die()
    {
        Debug.Log("플레이어 사망!");
    }
}