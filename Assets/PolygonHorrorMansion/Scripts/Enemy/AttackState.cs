using UnityEngine;

public class AttackState : EnemyState
{
    public ChaseState chaseState;
    public PatrolState patrolState;

    [SerializeField] private float attackDmg = 1.0f;

    private bool isStunned = false;
    private float stunDuration = 3f;
    private float stunTimer = 0f;

    private bool hasAttacked = false;

    public EnemyManager enemyManager;

    private void OnEnable()
    {
        FirstPersonController.OnPlayerEscaped += HandlePlayerEscaped;
        // Reset the attack flag every time we enter this state
        hasAttacked = false;
    }

    private void OnDisable()
    {
        FirstPersonController.OnPlayerEscaped -= HandlePlayerEscaped;
    }

    private void HandlePlayerEscaped()
    {
        if (!isStunned)
        {
            isStunned = true;
            hasAttacked = false;
            stunTimer = 0f;
        }
    }

    public override EnemyState RunCurrentState(EnemyManager enemy)
    {
        // In Attack state, the enemy does not move
        enemy.enemyMovement.isMoving = false;
        enemy.navMeshAgent.isStopped = true;
        enemy.navMeshAgent.speed = 0;

        // Attack once if we have not attacked yet and have a valid target
        if (!hasAttacked && enemyManager.lastDamageableTarget != null && !isStunned)
        {
            enemyManager.lastDamageableTarget.ApplyDamage(attackDmg);
            hasAttacked = true;
            // After the attack, the player must escape to allow another attack cycle in the future
        }

        if (isStunned)
        {
            // Handle stun logic
            stunTimer += Time.deltaTime;
            if (stunTimer >= stunDuration)
            {
                // Stun is over
                isStunned = false;
                enemyManager.lastDamageableTarget = null; // Clear the last target after escaping

                // After stun, check if enemy can still see the player
                if (enemy.fov.canSeePlayer)
                {
                    return chaseState;
                }
                else
                {
                    return patrolState;
                }
            }

            // While stunned, just remain in AttackState doing nothing
            return this;
        }

        // If the enemy no longer sees the player, return to patrol
        if (!enemy.fov.canSeePlayer)
        {
            return patrolState;
        }

        // If the player moves out of immediate range, go back to chase
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.target.transform.position);
        if (distanceToPlayer > enemy.navMeshAgent.stoppingDistance + 1f)
        {
            return chaseState;
        }

        return this;
    }
}
