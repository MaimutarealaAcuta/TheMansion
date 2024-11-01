using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : EnemyState
{
    public bool isInAttackRange;
    public IddleState idleState;
    public PatrolState patrolState;

    public override EnemyState RunCurrentState(EnemyManager enemy)
    {
        print("chase");

        if (isInAttackRange)
        {
            print("will Attack");
            return this; // should return an AttackState object
        }
        else if(enemy.isPerformingAction)
        {
            return this;
        }
        else if (!enemy.fov.canSeePlayer)
        {
            print("cant see");
            return patrolState;
        }

        if (!enemy.navMeshAgent.enabled)
            enemy.navMeshAgent.enabled = true;


        enemy.enemyMovement.isMoving = true;

        

        print("is moving nav: " + enemy.navMeshAgent.isStopped);
        //OPTION 1
        //enemy.navMeshAgent.SetDestination(enemy.target.transform.position); //is asynchronous, can be costly if path is very long

        //OPTION 2
        NavMeshPath path = new NavMeshPath();
   
        enemy.navMeshAgent.CalculatePath(enemy.target.transform.position, path);
        enemy.navMeshAgent.SetPath(path);
        
        //enemy.navMeshAgent.Move(transform.forward*Time.deltaTime*5);

        //enemy.enemyMovement.RotateTowardsAgent(enemy.navMeshAgent.transform.rotation);


        //enemy.enemyMovement.speed = 5;
        enemy.navMeshAgent.speed = 4.1f;

        return this;
    }

   
}

   

