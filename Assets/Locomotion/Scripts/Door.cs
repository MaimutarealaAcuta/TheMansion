using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Door : Interactable
{
    [Header("Door Configuration")]
    [SerializeField] private bool isDoubleDoor = false;
    [SerializeField] private Transform otherDoor;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private float colliderEnableThreshold = 5f;

    private Door otherDoorRef;
    private bool isOpen = false;
    public bool isOpen = false;
    private bool isMoving = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Quaternion otherDoorClosedRotation;
    private Quaternion otherDoorOpenRotation;

    private NavMeshObstacle navMeshObst;

    public bool IsOpen
    {
        get { return isOpen; }
    }

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

        if (isDoubleDoor && otherDoor != null)
        {
            otherDoorRef = otherDoor.GetComponent<Door>();
            otherDoorClosedRotation = otherDoor.rotation;
            // Other door opens in opposite direction
            otherDoorOpenRotation = otherDoorClosedRotation * Quaternion.Euler(0, -openAngle, 0);
        }


        if (transform.parent.TryGetComponent<NavMeshObstacle>(out navMeshObst))
        {
            navMeshObst.enabled = true;
        }
    }

    public override void OnInteract()
    {
        

        if (!isMoving)
        {
            isOpen = !isOpen;
            otherDoorRef.isOpen = isOpen;
            StartCoroutine(RotateDoors());
        }


    }

    private IEnumerator RotateDoors()
    {
        isMoving = true;
        otherDoorRef.isMoving = true;

        // Get colliders
        Collider mainCollider = GetComponent<Collider>();
        Collider otherCollider = null;

        if (isDoubleDoor && otherDoor != null)
        {
            otherCollider = otherDoor.GetComponent<Collider>();
        }

        // Disable colliders
        mainCollider.enabled = false;
        if (otherCollider != null) otherCollider.enabled = false;

        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        Quaternion otherTargetRotation = isOpen ? otherDoorOpenRotation : otherDoorClosedRotation;

        float remainingAngle;

        bool navMeshChanged = false;

        do
        {
            // Calculate remaining angle (use the maximum of both doors if it's a double door)
            float mainAngle = Quaternion.Angle(transform.rotation, targetRotation);
            float otherAngle = isDoubleDoor && otherDoor != null ?
                              Quaternion.Angle(otherDoor.rotation, otherTargetRotation) : 0f;

            remainingAngle = Mathf.Max(mainAngle, otherAngle);

            // Rotate main door
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                                                Time.deltaTime * openSpeed);

            // Rotate other door if it's a double door
            if (isDoubleDoor && otherDoor != null)
            {
                otherDoor.rotation = Quaternion.Slerp(otherDoor.rotation, otherTargetRotation,
                                                     Time.deltaTime * openSpeed);
            }

            // Enable colliders when near target
            if (remainingAngle <= colliderEnableThreshold)
            {
                if (!mainCollider.enabled) mainCollider.enabled = true;
                if (otherCollider != null && !otherCollider.enabled) otherCollider.enabled = true;
            }


            //Let enemy through
            if (!navMeshChanged && remainingAngle < 40f && navMeshObst != null)
            {
                navMeshObst.enabled = !navMeshObst.enabled;
                navMeshChanged = true;
                print("navmeshchanged");
            }

            yield return null;
        }
        while (remainingAngle > 0.1f);

        // Final snap to target rotations
        transform.rotation = targetRotation;
        if (isDoubleDoor && otherDoor != null)
        {
            otherDoor.rotation = otherTargetRotation;
        }

        isMoving = false;
        otherDoorRef.isMoving = false;
    }

    public override void OnFocus()
    {

    }

    public override void OnLoseFocus()
    {

    }
}