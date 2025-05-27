using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public enum AIState
{
    Idle,
    Wandering,
    Attacking
}

public class Zombie : Enemy, IAttackAble
{
    [Header("Info")]
    public float walkSpeed;
    public float runSpeed;
    
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
    private float attackPower = 15f;
    public float AttackPower => attackPower;
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;

    private float playerDistance;
    
    public float fieldOfView = 120f;
    
    private Animator animator;
    private SkinnedMeshRenderer[] meshRenderers;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        SetState(AIState.Wandering);
    }

    private void Update()
    {
        playerDistance = Vector3.Distance(transform.position, CharacterManager.Player.transform.position);
        animator.SetBool("IsMove", aiState != AIState.Idle);

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
            Invoke("WanderToNewLocation", UnityEngine.Random.Range(minWanderWaitTime, maxWanderWaitTime));    
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
            Vector3 randomDirection = UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(minWanderDistance, maxWanderDistance);
            NavMesh.SamplePosition(transform.position + randomDirection, out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
        }
        while (Vector3.Distance(transform.position, hit.position) < detectDistance && i < 30);

        return hit.position;
    }

    public override void TakeDamage()
    {
        
    }
    
    public void Attack()
    {
        
    }

    public void SetState(AIState state)
    {
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

}