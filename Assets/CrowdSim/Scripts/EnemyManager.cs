using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public bool isPerformingAction;
    public NavMeshAgent navMeshAgent;
    public GameObject target;

    [SerializeField] EnemyState currentState;

    [Header("States")]
    [SerializeField] IddleState idle;
    [SerializeField] ChaseState chase;
    [SerializeField] PatrolState patrol;

    public EnemyMovement enemyMovement;
    public FieldOfView fov;

    [SerializeField] Animator animator;

    private void Awake()
    {
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();

        currentState = patrol;
        EnemyManager enemyManager = GetComponent<EnemyManager>();
        enemyManager.currentState = currentState;

        enemyMovement = GetComponent<EnemyMovement>();

        fov = GetComponent<FieldOfView>();
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
        //print("current state: "+currentState);

        if (nextState != null)
        {
            SwitchToTheNextState(nextState);
        }

        //enemyMovement.isMoving = false;
        //navMeshAgent.transform.localPosition = Vector3.zero;
        //navMeshAgent.transform.localRotation = Quaternion.identity;

        navMeshAgent.isStopped = !enemyMovement.isMoving;
        //print(navMeshAgent.isStopped);
        if (navMeshAgent.enabled)
        {
            Vector3 agentDestination = navMeshAgent.destination;
            float remainingDistance = Vector3.Distance(agentDestination, transform.position);
            //print(remainingDistance + " " + navMeshAgent.stoppingDistance);
            if (remainingDistance > navMeshAgent.stoppingDistance)   //make sure to adjust stopping distance on Nav Mesh Agent component
            {

                transform.position = agentPosition;
                transform.rotation = agentRotation;
                navMeshAgent.transform.localPosition = Vector3.zero;
                navMeshAgent.transform.localRotation = Quaternion.identity;

                //print("rotate");
                //print(remainingDistance);
                //enemyMovement.isMoving = true;

                //enemyMovement.RotateTowardsAgent(navMeshAgent.transform.rotation);
                //transform.position = navMeshAgent.transform.position;
            }
            //else
            //{
            //    print("is moving false 1");
            //    enemyMovement.isMoving = false;
            //}
        }
        else
        {
            //print("is moving false 2");

            enemyMovement.isMoving = false;
        }
    }

    private void SwitchToTheNextState(EnemyState nextState)
    {
        //print("next state: "+nextState);
        currentState = nextState;
        //animator.SetFloat("Speed", enemyMovement.speed);
        animator.SetFloat("Speed", navMeshAgent.speed);
    }
}
