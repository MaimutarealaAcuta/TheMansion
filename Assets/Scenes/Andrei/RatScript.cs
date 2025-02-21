using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class RatScript : MonoBehaviour
{

    [SerializeField]
    private Transform FoodPosition;

    [SerializeField]
    private Transform RetreatPosition;

    [SerializeField]
    private float normalSpeed = .5f;

    [SerializeField]
    private float runSpeed = 3f;

    private float idleTimer = 0;
    private float idleMinDuration = 2f;
    private float idleMaxDuration = 5f;

    private float startledTimer = 0;
    private float startledDuration = 5f;

    private NavMeshAgent agent;
    private Animator animator;

    private Transform currentWaypoint;

    enum RatState
    {
        Idle,
        RunningToSafety,
        Hiding,
        RunningToFood
    }

    private RatState currentState = RatState.Idle;
    private bool isStartled = false;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = RatState.Idle;
        isStartled = false;
        agent.enabled = true;

        agent.SetDestination(FoodPosition.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.pathPending) return;

        switch (currentState)
        {
            case RatState.Idle:
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    if (idleTimer <= 0)
                    {
                        Vector3 randomDirection = Random.insideUnitSphere;
                        randomDirection += FoodPosition.position;
                        NavMeshHit hit;
                        NavMesh.SamplePosition(randomDirection, out hit, 5f, NavMesh.AllAreas);
                        agent.SetDestination(hit.position);
                        idleTimer = Random.Range(idleMinDuration, idleMaxDuration);
                    }
                    else
                    {
                        idleTimer -= Time.deltaTime;
                    }
                }                
                break;
            case RatState.RunningToSafety:
                //Debug.Log(agent.destination);
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    currentState = RatState.Hiding;
                    agent.enabled = false;
                    startledTimer = startledDuration;
                }
                break;
            case RatState.Hiding:
                if(!isStartled)
                {
                    currentState = RatState.RunningToFood;
                    agent.enabled = true;
                    agent.speed = runSpeed;
                    agent.SetDestination(FoodPosition.position);                    
                }
                else
                {
                    startledTimer -= Time.deltaTime;
                    if (startledTimer <= 0)
                    {
                        isStartled = false;
                    }
                }
                break;
            case RatState.RunningToFood:
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    currentState = RatState.Idle;
                    agent.speed = normalSpeed;
                }
                break;
        }

    }

    public void Startle()
    {
        isStartled = true;
        currentState = RatState.RunningToSafety; 
        agent.speed = runSpeed;
        if(!agent.SetDestination(RetreatPosition.position))
            Debug.Log("Failed to set retreat position destination on Startle!");
        agent.enabled = true;
    }
}
