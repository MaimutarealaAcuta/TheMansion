using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IddleState : EnemyState
{

    public ChaseState chaseState;
    public bool canSeeThePlayer;

    public PatrolState patrolState;

    private EnemyState state;

    private float idleTime;             // Time to stay idle
    private float idleTimer = 0;        // Timer to track idle duration
    private bool isWaiting = false;

    private void Start()
    {
        print(gameObject.name);

    }

    public override EnemyState RunCurrentState(EnemyManager enemy)
    {
        //print("Idle");

        if (enemy.fov.canSeePlayer)
        {

            return chaseState;
        }
        else
        {

            
            if (!isWaiting)
            {
                idleTime = Random.Range(2f, 10f);
                print("idle time: " + idleTime);
                idleTimer = 0; // Reset the timer
                isWaiting = true;
                //enemy.navMeshAgent.destination = enemy.transform.position;
                enemy.enemyMovement.isMoving = false;
               
            }

            // Increment the timer
            idleTimer += Time.deltaTime;
            //print(idleTime);
            //print(idleTimer);
            // Check if the idle time has elapsed
            if (idleTimer >= idleTime)
            {
                print("please do patrol from idle");
                isWaiting = false;
                enemy.enemyMovement.isMoving = true;
                return patrolState; // Switch to patrol state
            }


            //enemy.enemyMovement.speed = 0;
            enemy.navMeshAgent.speed = 0;
            // Remain in the idle state until time is up
            return this;

            
        }
    }
    
  
}
