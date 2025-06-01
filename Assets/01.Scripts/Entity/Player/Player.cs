
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
    
    // 플레이어 수치 참조용
    private float hungerPassive;
    private float staminaPassive;
    private float healthPassive;
    
    private bool isPlayerDead;
    
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
        
        hungerPassive = CharacterManager.Player.statHandler.GetPassive(StatType.Hunger);
        staminaPassive = CharacterManager.Player.statHandler.GetPassive(StatType.Stamina);
        healthPassive = CharacterManager.Player.statHandler.GetPassive(StatType.Health);
    }

    private void Start()
    {
        statHandler.Initialize(statProfile.ToDictionary());
        SpawnManager temp = SpawnManager.Instance;
        
        hungerPassive = CharacterManager.Player.statHandler.GetPassive(StatType.Hunger);
        staminaPassive = CharacterManager.Player.statHandler.GetPassive(StatType.Stamina);
        isPlayerDead = false;
    }

    private void Update()
    {
        if (isPlayerDead) return;
        timer += Time.deltaTime;

        if (timer < 1f) return;
        if (statHandler.Get(StatType.Health) <= 0f)
        {
            isPlayerDead = true;
            Die();
        }
        timer = 0f;
        HandlePassiveStats();
    }
    
    private void HandlePassiveStats()
    {
        // 배고픔 감소
        statHandler.Modify(StatType.Hunger, hungerPassive);
        UIManager.Instance.UpdateStatUI(StatType.Hunger);

        if (statHandler.IsEmpty(StatType.Hunger))
        {
            // 배고픔이 0이면 체력 감소
            statHandler.Modify(StatType.Health, healthPassive);
            UIManager.Instance.UpdateStatUI(StatType.Health);
            return; // 스태미나 리젠 없음
        }

        // 스태미나 회복
        if (!controller.isRunning)
        {
            statHandler.Modify(StatType.Stamina, staminaPassive);
            UIManager.Instance.UpdateStatUI(StatType.Stamina);
        }
    }
    
    public void TakeDamage(float damage)
    {
        statHandler.Modify(StatType.Health, -damage);
        SoundManager.Instance.PlayRandomSfx(SfxType.Hit);
        UIManager.Instance.PlayDamageEffect(0.5f);
        UIManager.Instance.UpdateStatUI(StatType.Health);
    }
    
    public void Die()
    {
        GameManager.Instance.GameOver();
        //Debug.Log("플레이어 사망!");
    }
}