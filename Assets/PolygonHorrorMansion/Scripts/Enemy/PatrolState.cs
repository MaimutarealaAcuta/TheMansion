using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : EnemyState
{
    public ChaseState chaseState;
    public IddleState idleState;

    [SerializeField] List<Transform> waypoints;
    private int waypointIndex = 0;
    private int waypointIndexOld;

    public override EnemyState RunCurrentState(EnemyManager enemy)
    {
        enemy.navMeshAgent.speed = 2;

        if (enemy.fov.canSeePlayer)
        {
            return chaseState;
        }


        if (waypointIndex == 0)
        {
            enemy.navMeshAgent.SetDestination(waypoints[waypointIndex].position);
            waypointIndex++;
        }

        enemy.enemyMovement.isMoving = true;

        // When the agent reaches the current waypoint, choose a new one and then go idle
        if (enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance)
        {
            NavMeshPath path = new NavMeshPath();
            enemy.navMeshAgent.CalculatePath(waypoints[waypointIndex].position, path);
            enemy.navMeshAgent.SetPath(path);

            waypointIndexOld = waypointIndex;
            waypointIndex = Random.Range(0, waypoints.Count);

            if (waypointIndex == waypointIndexOld && waypointIndex < waypoints.Count - 1)
            {
                waypointIndex++;
            }

            enemy.enemyMovement.isMoving = false;
            return idleState;
        }

        return this;
    }
}
