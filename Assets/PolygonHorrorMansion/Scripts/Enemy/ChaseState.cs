using System;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : EnemyState
{
    public IddleState idleState;
    public PatrolState patrolState;
    public AttackState attackState;

    public EnemyManager enemyManager;
    private bool playerCollided = false;

    public AudioSource chaseAudioSource;
    public AudioClip chaseMusicClip;
    private bool isAudioPlaying = false;

    private float currentSpeed; // Tracks current speed
    public float maxSpeed = 4.1f; // Maximum speed
    public float accelerationRate = 1f; // Rate of speed increase per second
    public float startingSpeed = 2.5f; // Initial speed

    private void Start()
    {
        currentSpeed = startingSpeed;

        if (chaseAudioSource != null)
        {
            chaseAudioSource.clip = chaseMusicClip;
            chaseAudioSource.loop = true;
            chaseAudioSource.playOnAwake = false;
            chaseAudioSource.spatialBlend = 1f;
        }
    }

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
        if (!isAudioPlaying)
        {
            PlayChasingAudio();
        }

        // If we collided with the player, switch to attack state
        if (playerCollided)
        {
            playerCollided = false;
            StopChasingAudio();
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
                    StopChasingAudio();
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
                StopChasingAudio();
                return patrolState;
            }

            // If we do see the player normally, chase them
            enemy.enemyMovement.isMoving = true;
            if (!enemy.navMeshAgent.enabled)
                enemy.navMeshAgent.enabled = true;

            enemy.navMeshAgent.SetDestination(enemy.target.transform.position);
            enemy.navMeshAgent.speed = 4.1f;

            currentSpeed = Mathf.Min(currentSpeed + accelerationRate * Time.deltaTime, maxSpeed);
            enemy.navMeshAgent.speed = currentSpeed;

            // Remain in chase
            return this;
        }
    }

    void PlayChasingAudio()
    {
        isAudioPlaying = true;

        if (chaseAudioSource != null && !chaseAudioSource.isPlaying)
        {
            chaseAudioSource.Play();
        }
    }

    void StopChasingAudio()
    {
        isAudioPlaying = false;

        if (chaseAudioSource != null && !chaseAudioSource.isPlaying)
        {
            chaseAudioSource.Stop();
        }
    }
}
