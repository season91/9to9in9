using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum AIState
{
    Idle,
    Wandering,
    Attacking
}

public class Zombie : Enemy, IAttackAble
{
    [Header("Stat")]
    [SerializeField] private StatProfile statProfile;
    private StatHandler statHandler = new StatHandler();
    
    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance;
    private AIState aiState;
    
    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    [Header("Combat")]
    [SerializeField] private float speedOffset;
    private float lastAttackTime;
    public float attackDistance;

    
    [SerializeField] private float playerDistance;
    
    public float fieldOfView = 120f;
    
    private Animator animator;
    private SkinnedMeshRenderer[] meshRenderers;

    private bool isCalculate = true;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        
        if (agent == null) Debug.LogError("NavMeshAgent not found");
        if (animator == null) Debug.LogError("Animator not found");
        if (meshRenderers == null) Debug.LogError("SkinnedMeshRenderer not found");
        
    }

    private void Start()
    {
        SetState(AIState.Wandering);
        
        statHandler.Initialize(statProfile.ToDictionary());
        if (statProfile == null) Debug.LogError("StatProfile not found");
    }

    private void Update()
    {
        playerDistance = Vector3.Distance(transform.position, CharacterManager.Player.transform.position);
        animator.SetBool("IsMove", agent.velocity.magnitude > 0);

        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(CharacterManager.Player.transform.position,path))
        {
            isCalculate = true;
        }
        else
        {
            isCalculate = false;
        }
        
        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                Move();
                break;
            case AIState.Attacking:
                Attack();
                break;
        }
    }

    public override void Move()
    {
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));    
        }

        if (playerDistance < detectDistance && IsPlayerOnWalkable())
        {
            SetState(AIState.Attacking);
        }
    }

    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) return;
        
        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
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

        return hit.position;
    }


    public override void TakeDamage(float damage)
    {
        statHandler.Modify(StatType.Health, -damage);
        if (statHandler.IsEmpty(StatType.Health))
            Die();
    }

    public override void Die()
    {
        //TODO: 죽었을 때 로직
    }

    public void Attack()
    {
        float attackPower = statHandler.Get(StatType.AttackPower);
        float attackspeed = statHandler.Get(StatType.AttackSpeed);
        
        if (playerDistance < attackDistance && IsPlayerInFieldOfView())
        {
            agent.isStopped = true;
            if (Time.time - lastAttackTime > attackspeed)
            {
                lastAttackTime = Time.time;
                animator.speed = 1;
                animator.SetTrigger("Attack"); ;
                statHandler.Modify(StatType.Health, -attackPower);
            }
        }
        else
        {
            if (playerDistance < detectDistance)
            {
                agent.isStopped = false;
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(CharacterManager.Player.transform.position, path))
                {;
                    agent.SetDestination(CharacterManager.Player.transform.position);
                }
                else
                {
                    agent.SetDestination(transform.position);
                    agent.isStopped = true;
                    SetState(AIState.Wandering);
                    
                }
            }
            else
            {
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                SetState(AIState.Wandering);
            }
        }
    }

    bool IsPlayerInFieldOfView()
    {
        Vector3 direction = CharacterManager.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, direction);
        return angle < fieldOfView * 0.5f;
    }
    
    public void SetState(AIState state)
    {
        float walkSpeed = statHandler.Get(StatType.MoveSpeed);
        float runSpeed = statHandler.Get(StatType.MoveSpeed) + speedOffset;
        
        aiState = state;

        switch (aiState)
        {
            case AIState.Idle:
                agent.speed = walkSpeed;
                agent.isStopped = true;
                break;
            case AIState.Wandering:
                agent.speed = walkSpeed;
                agent.isStopped = false;
                break;
            case AIState.Attacking:
                agent.speed = runSpeed;
                agent.isStopped = false;
                break;
        }
        
        animator.speed = agent.speed / walkSpeed;
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
}