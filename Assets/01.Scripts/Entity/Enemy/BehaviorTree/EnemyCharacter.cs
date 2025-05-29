using UnityEngine;
using UnityEngine.AI;

public class EnemyCharacter : MonoBehaviour
{
    SelectorNode rootNode; // 루트 노드는 셀렉터노드
 
    SequenceNode attackSequence; // 공격 시퀀스 노드
    SelectorNode attackSelector; // 공격 셀렉터 노드
    SequenceNode realattackSequence; // 공격 시퀀스 안의 시퀀스 
    
    SelectorNode detectiveSelector; // 탐지 셀렉터 노드
    SequenceNode detectiveSequence; // 탐지 셀렉터 안의 시퀀스

    ActionNode idleActionNode; // 예외처리인 가만히 있기 액션
    
    [Header("Info")] 
    [SerializeField] private Stat health;
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

    [SerializeField] private float playerDistance;
    
    public float fieldOfView = 120f;
    
    private Animator animator;
    private SkinnedMeshRenderer[] meshRenderers;

    private bool isCalculate = true;

    void Start()
    {
        // 공격 관련 노드 생성
        attackSequence = new SequenceNode();
        attackSelector = new SelectorNode();
        realattackSequence = new SequenceNode();

        // 공격
        // 시퀀스 -> 셀렉터 노드 -> 시퀀스
        realattackSequence.Add(new ActionNode(CheckAbleToAttack));
        realattackSequence.Add(new ActionNode(Attack));
        
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
        rootNode.Add(detectiveSequence); // 탐지 시퀀스
        rootNode.Add(idleActionNode); // 가만히 있기 액션
    }
    
    INode.State Attack() // 공격 액션에 할당될 함수
    {
        Debug.Log("공격중");
        return INode.State.RUN;
    }

    public INode.State CheckAbleToAttack()
    {
        return INode.State.SUCCESS;
        // 공격 사거리 내에, 시야각 내에 있는지 체크
    }

    public INode.State ChasingPlayer()
    {
        return INode.State.RUN;
    }

    public INode.State CheckPlayerInDetectDistnace()
    {
        return INode.State.SUCCESS;
    }

    public INode.State IsAlivedTargettingPlace()
    {
        return INode.State.SUCCESS;
    }

    public INode.State RestAndSelectNewPlaceToMove()
    {
        return INode.State.SUCCESS;
    }

    public INode.State MoveToSelectedPlace()
    {
        return INode.State.SUCCESS;
    }

    public INode.State IdleAction()
    {
        return INode.State.SUCCESS;
    }
    
    // 업데이트 내에서는 루트 노드(행동 트리 전체)의 상태를 평가한다.
    void Update()
    {
        rootNode.Evaluate();
    }

}