using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class MonsterPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float normalSpeed = 1f;
    [SerializeField] private float runSpeed = 3f;
    [SerializeField] private float animationSpeed = 1.2f;
    [SerializeField] private bool randomWaypoints = true;

    [SerializeField] private bool wasJustDropped = false;
    [SerializeField] private bool isPaused = true; // Start in paused state

    private NavMeshAgent agent;
    private Animator animator;

    private Transform currentWaypoint;
    

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        //agent.speed = normalSpeed;
        //ChooseNextWaypoint();

        // Do not choose a waypoint yet, since we are paused.
        // Movement will start once StartMovementFromPause() is called.
        agent.enabled = false;
    }

    void Update()
    {
        UpdateAnimation();

        if (!isPaused && agent.enabled)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (wasJustDropped)
                {
                    // After reaching the next waypoint post-drop, return to normal speed
                    agent.speed = normalSpeed;
                    wasJustDropped = false;
                }
                ChooseNextWaypoint();
            }
        }
    }

    /// <summary>
    /// Activate the monster from a paused state.
    /// It will run at runSpeed to the first waypoint, then revert to normal patrol.
    /// </summary>
    public void StartMovementFromPause()
    {
        if (isPaused && agent)
        {
            isPaused = false;
            agent.enabled = true;
            agent.speed = runSpeed;
            wasJustDropped = true;
            ChooseNextWaypoint();
        }
    }

    public void PauseMovement()
    {
        isPaused = true;
        agent.enabled = false;
    }

    public void ResumeNormalMovement()
    {
        isPaused = false;
        agent.enabled = true;
        agent.speed = normalSpeed;
        ChooseNextWaypoint();
    }

    public void RunToNextWaypoint()
    {
        isPaused = false;
        agent.enabled = true;
        agent.speed = runSpeed;
        wasJustDropped = true;
        ChooseNextWaypoint();
    }

    private void ChooseNextWaypoint()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        Transform nextWaypoint;
        if (randomWaypoints)
        {
            nextWaypoint = waypoints[Random.Range(0, waypoints.Count)];
        }
        else
        {
            int currentIndex = currentWaypoint == null ? -1 : waypoints.IndexOf(currentWaypoint);
            int nextIndex = (currentIndex + 1) % waypoints.Count;
            nextWaypoint = waypoints[nextIndex];
        }

        currentWaypoint = nextWaypoint;
        if (agent.enabled)
        {
            agent.SetDestination(currentWaypoint.position);
        }
    }

    private void UpdateAnimation()
    {
        if (!animator) return;

        bool moving = false;
        float speedValue = 0f;

        if (!isPaused && agent.enabled)
        {
            moving = true;
            speedValue = agent.velocity.magnitude;
        }

        animator.SetBool("Moving", moving);
        animator.speed = animationSpeed * speedValue;
    }
}
