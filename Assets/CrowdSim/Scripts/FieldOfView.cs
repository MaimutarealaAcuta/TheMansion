using System.Collections;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;

    private float normalRadius;
    private float crouchRadius;

    [Range(0, 360)]
    public float angle;
    public float rayHeight;

    public GameObject playerRef;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public LayerMask interactableMask;

    public bool canSeePlayer = false;
    FirstPersonController playerControler;

    private float playerHeightOffset = 1.5f; // Adjust this to the height of the player’s head

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        playerControler = playerRef.GetComponent<FirstPersonController>();

        normalRadius = radius;
        crouchRadius = radius / 3;

        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;

            if (playerControler.IsCrouching())
                radius = crouchRadius;
            else
                radius = normalRadius;

            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position + new Vector3(0, rayHeight, 0), radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // Check if the player is within the angle of view
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                // Perform the first raycast to the player's center position
                if (!Physics.Raycast(transform.position + new Vector3(0, rayHeight, 0), directionToTarget, distanceToTarget, obstructionMask | interactableMask))
                {
                    canSeePlayer = true;
                }
                else
                {
                    // Perform a second raycast to the player’s head position to see over shorter obstacles
                    Vector3 headPosition = playerRef.transform.position + new Vector3(0, playerHeightOffset, 0);
                    Vector3 directionToHead = (headPosition - transform.position).normalized;

                    if (!Physics.Raycast(transform.position + new Vector3(0, rayHeight, 0), directionToHead, distanceToTarget, obstructionMask | interactableMask))
                    {
                        canSeePlayer = true;
                    }
                    else
                    {
                        canSeePlayer = false;
                    }
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
    }
}
