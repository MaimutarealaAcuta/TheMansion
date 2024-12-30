using UnityEngine;

public class IddleState : EnemyState
{
    public ChaseState chaseState;
    public PatrolState patrolState;

    private float idleTime;
    private float idleTimer = 0;
    private bool isWaiting = false;

    public override EnemyState RunCurrentState(EnemyManager enemy)
    {
        if (enemy.fov.canSeePlayer)
        {
            return chaseState;
        }
        else
        {
            if (!isWaiting)
            {
                idleTime = Random.Range(2f, 10f);
                idleTimer = 0;
                isWaiting = true;
                enemy.enemyMovement.isMoving = false;
            }

            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTime)
            {
                isWaiting = false;
                enemy.enemyMovement.isMoving = true;
                return patrolState;
            }

            enemy.navMeshAgent.speed = 0;
            return this;
        }
    }
}
