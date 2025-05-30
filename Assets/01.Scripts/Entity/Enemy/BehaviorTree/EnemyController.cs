using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Enemy, IAttackAble
{
    [Header("Nodes")]
    SelectorNode rootNode; // 루트 노드는 셀렉터노드
 
    SequenceNode attackSequence; // 공격 시퀀스 노드
    SelectorNode attackSelector; // 공격 셀렉터 노드
    SequenceNode realattackSequence; // 공격 시퀀스 안의 시퀀스 
    
    SelectorNode detectiveSelector; // 탐지 셀렉터 노드
    SequenceNode detectiveSequence; // 탐지 셀렉터 안의 시퀀스

    ActionNode idleActionNode; // 예외처리인 가만히 있기 액션
    
    [Header("Stat")]
    [SerializeField] private StatProfile statProfile;
    private StatHandler statHandler;
    private float walkSpeed;
    
    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance;
        
    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    [Header("Combat")]
    [SerializeField] private float speedOffset;
    private float lastAttackTime;
    public float attackDistance;
    
    [Header("AI")]
    private float playerDistance;
    private bool isWaitingToWander = false;
    
    public float fieldOfView = 120f;
    
    [Header("Particle")]
    [SerializeField] private ParticleSystem hitBlood;
    
    private Animator animator;
    private SkinnedMeshRenderer[] meshRenderers;
    

    public ItemData[] itemdatas;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    
        if (agent == null) Debug.LogError("NavMeshAgent not found");
        if (animator == null) Debug.LogError("Animator not found");
        if (meshRenderers == null) Debug.LogError("SkinnedMeshRenderer not found");
    }

    void Start()
    {
        statHandler = GetComponent<StatHandler>();
        if (statHandler == null) Debug.LogError("Player StatHandler not found");
        
        statHandler.Initialize(statProfile.ToDictionary());
        if (statProfile == null) Debug.LogError("StatProfile not found");

        walkSpeed = statHandler.Get(StatType.MoveSpeed);
        
        // 공격 관련 노드 생성
        attackSequence = new SequenceNode();
        attackSelector = new SelectorNode();
        realattackSequence = new SequenceNode();

        // 공격
        // 시퀀스 -> 셀렉터 노드 -> 시퀀스
        realattackSequence.Add(new ActionNode(CheckAbleToAttack));
        realattackSequence.Add(new ActionNode(Attacking));
        
        // 시퀀스 -> 셀렉터 노드
        attackSelector.Add(realattackSequence);
        attackSelector.Add(new ActionNode(ChasingPlayer));
        
        // 시퀀스
        attackSequence.Add(new ActionNode(CheckPlayerInDetectDistnace));
        attackSequence.Add(attackSelector);
        
        // 탐지 관련 노드 생성
        realattackSequence = new SequenceNode();
        detectiveSelector = new SelectorNode();
        detectiveSequence = new SequenceNode();

        // 탐지
        // 셀렉터 -> 시퀀스
        detectiveSequence.Add(new ActionNode(IsAlivedTargettingPlace));
        detectiveSequence.Add(new ActionNode(RestAndSelectNewPlaceToMove));
        
        // 셀렉터
        detectiveSelector.Add(detectiveSequence);
        detectiveSelector.Add(new ActionNode(MoveToSelectedPlace));
        
        // 예외처리 Idle 노드 생성
        idleActionNode = new ActionNode(IdleAction);
        
        // 루트 노드 생성
        rootNode = new SelectorNode();
        rootNode.Add(attackSequence); // 루트 노드의 자식으로 공격 시퀀스
        rootNode.Add(detectiveSelector); // 탐지 시퀀스
        rootNode.Add(idleActionNode); // 가만히 있기 액션
    }
    
    // 업데이트 내에서는 루트 노드(행동 트리 전체)의 상태를 평가한다.
    void Update()
    {
        playerDistance = Vector3.Distance(transform.position, CharacterManager.Player.transform.position);
        animator.SetBool("IsMove", agent.velocity.magnitude > 0);
        
        rootNode.Evaluate();
    }
    
    INode.State Attacking() // 공격 액션에 할당될 함수
    {
        Attack();
        return INode.State.RUN;
    }

    public INode.State CheckAbleToAttack()
    {
        if (playerDistance < attackDistance && IsPlayerInFieldOfView())
        {
            return INode.State.SUCCESS;
        }
        return INode.State.FAILED;
    }

    public INode.State ChasingPlayer()
    {
        Move();
        return INode.State.RUN;
    }

    public INode.State CheckPlayerInDetectDistnace()
    {
        if (playerDistance < detectDistance && IsPlayerOnWalkable())
        {
            agent.isStopped = false;
            agent.speed = walkSpeed + speedOffset;
            return INode.State.SUCCESS;
        }
        return INode.State.FAILED;
    }

    public INode.State IsAlivedTargettingPlace()
    {
        agent.speed = walkSpeed;
        agent.isStopped = false;
        
        if (agent.remainingDistance < 0.1f)
        {
            return INode.State.SUCCESS;
        }
        return INode.State.FAILED;
    }

    public INode.State RestAndSelectNewPlaceToMove()
    {
        agent.speed = walkSpeed;
        agent.isStopped = true;
        
        if (isWaitingToWander == false)
        {
            Debug.Log(isWaitingToWander);
            isWaitingToWander = true;
             //           Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
            Invoke("WanderToNewLocation", 10f);
        }
        return INode.State.RUN;
    }

    public INode.State MoveToSelectedPlace()
    {
        return INode.State.RUN;
    }

    public INode.State IdleAction()
    {
        agent.speed = walkSpeed;
        agent.isStopped = true;
        return INode.State.SUCCESS;
    }
    
    bool IsPlayerInFieldOfView()
    {
        Vector3 direction = CharacterManager.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, direction);
        return angle < fieldOfView * 0.5f;
    }

    public void Attack()
    {
        float attackPower = statHandler.Get(StatType.AttackPower);
        float attackspeed = statHandler.Get(StatType.AttackSpeed);
        
        agent.isStopped = true;
        if (Time.time - lastAttackTime > attackspeed)
        {
            lastAttackTime = Time.time;
            animator.speed = 1;
            animator.SetTrigger("Attack");
            
            CharacterManager.Player.TakeDamage(attackPower);
        }
    }
    
    bool IsPlayerOnWalkable()
    {
        NavMeshHit hit;
        float checkDistance = 0.1f;

        // Walkable 마스크 구하기 (Walkable = 기본 0 또는 1번 영역)
        int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");

        if (NavMesh.SamplePosition(CharacterManager.Player.transform.position, out hit, checkDistance, walkableMask))
        {
            return true;
        }

        return false;
    }
    
    void WanderToNewLocation()
    {
        isWaitingToWander = false;
        agent.speed = walkSpeed;
        agent.isStopped = false;
        
        Debug.Log(agent.pathPending);

        Vector3 targetPosition = GetWanderLocation();
        
        agent.SetDestination(CharacterManager.Player.transform.position);
        Debug.Log(agent.hasPath);
        Debug.Log(agent.remainingDistance);
        Debug.Log("WanderToNewLocation");
    }
    
    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;
        int i = 0;

        do
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30) break;
        } while (Vector3.Distance(transform.position, hit.position) < detectDistance);

        Debug.Log(hit.position);
        return hit.position;
    }

    public override void TakeDamage(float damage)
    {
        statHandler.Modify(StatType.Health, -damage);
        SoundManager.Instance.PlayParticle(hitBlood);
        
        if (statHandler.Get(StatType.Health) <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        foreach (ItemData itemData in itemdatas)
        {
            SpawnManager.Instance.ItemSpawnInPosition(itemData.name, transform.position);
        }
        Destroy(gameObject);
    }

    public override void Move()
    {
        agent.isStopped = false;
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(CharacterManager.Player.transform.position, path))
        {
            agent.SetDestination(CharacterManager.Player.transform.position);
        }
    }
}