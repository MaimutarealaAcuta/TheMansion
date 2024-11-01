using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : EnemyState
{
    public ChaseState chaseState;
    public bool canSeeThePlayer;

    public IddleState idleState;

    [SerializeField] List<Transform> waypoints;
    private int waypointIndex=0;
    private int waypointIndexOld;
    private float patrolTimeLimit = 5f;   // Time limit for patrol before switching to idle
    private float patrolTimer = 0f;       // Timer to track patrol duration

    public override EnemyState RunCurrentState(EnemyManager enemy)
    {
        //print("Patrol");
        //enemy.enemyMovement.speed = 2;
        enemy.navMeshAgent.speed = 2;

        if (enemy.fov.canSeePlayer)
        {
            print("chase");
            return chaseState;
        }


        ////Switch to idle state once the patrol time limit is reached
        //patrolTimer += Time.deltaTime;
        //if (patrolTimer >= patrolTimeLimit)
        //{
        //    patrolTimer = 0;

        //    return idleState;
        //}

        //print(enemy.navMeshAgent.remainingDistance);
        print("remaining distance" + enemy.navMeshAgent.remainingDistance);
        if (waypointIndex == 0)
        {
            enemy.navMeshAgent.SetDestination(waypoints[waypointIndex].transform.position);
            waypointIndex++;

            
        }
        enemy.enemyMovement.isMoving = true;

        print("remaining distance" + enemy.navMeshAgent.remainingDistance);

        if (enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance)
        {
            //enemy.navMeshAgent.SetDestination(waypoints[waypointIndex].transform.position);
            print(waypoints.Count+" "+ waypointIndex);
            NavMeshPath path = new NavMeshPath();
            enemy.navMeshAgent.CalculatePath(waypoints[waypointIndex].transform.position, path);
            enemy.navMeshAgent.SetPath(path);
            print(waypoints.Count);

            waypointIndexOld = waypointIndex;
            waypointIndex = Random.Range(0, waypoints.Count);

            if (waypointIndex == waypointIndexOld && waypointIndex< waypoints.Count - 1)
            {
                 waypointIndex++;
            }

                //if (waypointIndex < waypoints.Count-1)
                //{
                //    waypointIndex++;
                //}
                //else
                //    waypointIndex = 0;

                print("will return idle");
            enemy.enemyMovement.isMoving = false;
            return idleState;
        }




        

        return this;
        
    }

}
