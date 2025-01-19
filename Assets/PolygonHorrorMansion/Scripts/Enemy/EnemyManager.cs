using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public bool isPerformingAction;
    public NavMeshAgent navMeshAgent;
    public GameObject target;
    public EnemyState currentState;

    [Header("States")]
    [SerializeField] IddleState idle;
    [SerializeField] ChaseState chase;
    [SerializeField] PatrolState patrol;
    [SerializeField] AttackState attack;

    public EnemyMovement enemyMovement;
    public FieldOfView fov;
    [SerializeField] Animator animator;

    public IDamageable lastDamageableTarget;

    // Tracking player position via monster events
    private Vector3? lastKnownPlayerPosition;
    public bool isTrackingPlayer = false; // True if currently receiving or have received position updates

    private void Awake()
    {
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        currentState = patrol;
        enemyMovement = GetComponent<EnemyMovement>();
        fov = GetComponent<FieldOfView>();

        chase.attackState = attack;
    }

    private void OnEnable()
    {
        Monster.OnMonsterCry += OnMonsterPickedUp;
        Monster.OnMonsterCarriedUpdate += OnMonsterPositionUpdate;
        Monster.OnMonsterDropped += OnMonsterLost;
        Monster.OnMonsterBurned += OnMonsterLost;
    }

    private void OnDisable()
    {
        Monster.OnMonsterCry -= OnMonsterPickedUp;
        Monster.OnMonsterCarriedUpdate -= OnMonsterPositionUpdate;
        Monster.OnMonsterDropped -= OnMonsterLost;
        Monster.OnMonsterBurned -= OnMonsterLost;
    }

    private void OnMonsterPickedUp()
    {
        // Monster picked up, start tracking player position
        isTrackingPlayer = true;

        // If currently not in chase state, switch
        if (currentState != chase)
        {
            SwitchToTheNextState(chase);
        }
    }

    private void OnMonsterPositionUpdate(Vector3 playerPos)
    {
        // Update last known player position while monster is carried
        lastKnownPlayerPosition = playerPos;
    }

    public Vector3? GetLastKnownPlayerPosition()
    {
        return lastKnownPlayerPosition;
    }

    void Update()
    {
        RunStateMachine();
    }

    public void RunStateMachine()
    {
        Vector3 agentPosition = navMeshAgent.transform.position;
        Quaternion agentRotation = navMeshAgent.transform.rotation;

        EnemyState nextState = currentState?.RunCurrentState(this);

        if (nextState != null && nextState != currentState)
        {
            SwitchToTheNextState(nextState);
        }

        navMeshAgent.isStopped = !enemyMovement.isMoving;

        if (navMeshAgent.enabled)
        {
            float remainingDistance = Vector3.Distance(navMeshAgent.destination, transform.position);
            if (remainingDistance > navMeshAgent.stoppingDistance)
            {
                transform.position = agentPosition;
                transform.rotation = agentRotation;
                navMeshAgent.transform.localPosition = Vector3.zero;
                navMeshAgent.transform.localRotation = Quaternion.identity;
            }
        }
        else
        {
            enemyMovement.isMoving = false;
        }
    }

    private void SwitchToTheNextState(EnemyState nextState)
    {
        currentState = nextState;
        string trigger = AnimationTriggerName(nextState);
        animator.SetTrigger(trigger);
    }

    private string AnimationTriggerName(EnemyState nextState)
    {
        switch (nextState)
        {
            case IddleState _:
                return "Idle";
            case PatrolState _:
                return "Patrol";
            case ChaseState _:
                return "Chase";
            default:
                return "Idle";
        }
    }

    private void OnMonsterLost()
    {
        isTrackingPlayer = false;
        lastKnownPlayerPosition = null;
    }
}
