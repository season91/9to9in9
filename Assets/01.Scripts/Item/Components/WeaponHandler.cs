using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    [SerializeField] private float attackRate; // 공격 주기: (1 / 플레이어 공격속도)
    [SerializeField] private bool isAttacking; // 공격 가능한지
    [SerializeField] private float attackDistance; // 공격 가능 거리
    
    private EquipableItemData itemData;
    
    private Animator animator;
    private Camera camera;

    private void Awake()
    {
        if (!TryGetComponent(out ItemObject itemObject)) return;
        
        itemData = itemObject.itemData as EquipableItemData;
        if (itemData == null)
        {
            Debug.LogWarning("itemData가 EquipableItemData가 아닌가봐요");
        }
        
        camera = Camera.main;
        if (animator == null) Debug.LogError("camera is null");
        
        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("animator is null");
    }

    private void Start()
    {
        attackRate = 1 / CharacterManager.Player.statHandler.Get(StatType.AttackSpeed);
    }
    
    public void OnAttackInput()
    {
        if (isAttacking) return;
        
        isAttacking = true;
        Debug.Log($"{name} 사용!");
        animator.SetTrigger(Attack);
        OnHit();
        
        Invoke(nameof(OnCanAttack), attackRate);
    }

    private void OnCanAttack()
    {
        isAttacking = false;
    }

    private void OnHit()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, attackDistance)) return;

        bool isGather = (itemData.equipType == EquipType.GatheringTool);
        if (isGather && hit.collider.TryGetComponent(out ResourceHandler resourceHandler))
        {
            resourceHandler.Gather(hit.point, hit.normal);
        }
    }
}