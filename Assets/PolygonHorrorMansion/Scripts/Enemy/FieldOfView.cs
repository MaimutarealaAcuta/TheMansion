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

    [SerializeField]
    private float playerHeightOffset = 1.5f; // Adjust if needed

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

            radius = playerControler.IsCrouching() ? crouchRadius : normalRadius;
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

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                // Raycast to player's center
                if (!Physics.Raycast(transform.position + new Vector3(0, rayHeight, 0), directionToTarget, distanceToTarget, obstructionMask | interactableMask))
                {
                    canSeePlayer = true;
                }
                else
                {
                    // Raycast to player's head
                    Vector3 headPosition = playerRef.transform.position + new Vector3(0, playerHeightOffset, 0);
                    Vector3 directionToHead = (headPosition - transform.position).normalized;

                    canSeePlayer = !Physics.Raycast(transform.position + new Vector3(0, rayHeight, 0), directionToHead, distanceToTarget, obstructionMask | interactableMask);
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
