using UnityEngine;
using UnityEngine.AI;

public class ChaseState : EnemyState
{
    public IddleState idleState;
    public PatrolState patrolState;
    public AttackState attackState;

    public EnemyManager enemyManager;
    private bool playerCollided = false;

    private void OnTriggerEnter(Collider other)
    {
        FirstPersonController player = other.GetComponent<FirstPersonController>();
        if (player == null) return;

        if (enemyManager.currentState == this) // Only respond if currently in chase state
        {
            IDamageable damageable = player.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Store the damageable target in the EnemyManager so AttackState can access it
                enemyManager.lastDamageableTarget = damageable;
                playerCollided = true; // Mark that we hit the player
            }
        }
    }

    public override EnemyState RunCurrentState(EnemyManager enemy)
    {
        // If we collided with the player, switch to attack state
        if (playerCollided)
        {
            playerCollided = false;
            return attackState;
        }

        // If we have a last known position from the monster being carried
        if (enemy.isTrackingPlayer && enemy.GetLastKnownPlayerPosition().HasValue)
        {
            Vector3 destination = enemy.GetLastKnownPlayerPosition().Value;

            if (!enemy.navMeshAgent.enabled)
                enemy.navMeshAgent.enabled = true;

            enemy.enemyMovement.isMoving = true;
            enemy.navMeshAgent.SetDestination(destination);
            enemy.navMeshAgent.speed = 4.1f;

            // If can't see player and have reached destination, maybe revert to patrol
            if (!enemy.fov.canSeePlayer)
            {
                float distance = Vector3.Distance(enemy.transform.position, destination);
                if (distance < enemy.navMeshAgent.stoppingDistance + 0.5f)
                {
                    return patrolState;
                }
            }

            // Remain in chase
            return this;
        }
        else
        {
            // If no tracking info from the monster and we can’t see the player, go back to patrol
            if (!enemy.fov.canSeePlayer)
            {
                return patrolState;
            }

            // If we do see the player normally, chase them
            enemy.enemyMovement.isMoving = true;
            if (!enemy.navMeshAgent.enabled)
                enemy.navMeshAgent.enabled = true;

            enemy.navMeshAgent.SetDestination(enemy.target.transform.position);
            enemy.navMeshAgent.speed = 4.1f;

            // Remain in chase
            return this;
        }
    }
}
