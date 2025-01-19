using System.Collections;
using UnityEngine;
using UnityEngine.AI;

enum Axis
{
    X, Y, Z
}

public class Door : Interactable
{
    [Header("Door Configuration")]
    [SerializeField] private bool isDoubleDoor = false;
    [SerializeField] private Transform otherDoor;
    [SerializeField] private Axis openAxis = Axis.Y;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private float colliderEnableThreshold = 5f;
    [SerializeField] private bool shouldDoorBeOutlined = false;

    [Header("Lock Settings")]
    [SerializeField] public bool requireKey = false;
    [SerializeField] private string requiredKeyName = "Main Door";

    [Header("Sound Settings")]
    [SerializeField] private string closedDoorSound = "closed_door";
    [SerializeField] private string openDoorSound = "open_door";
    [SerializeField] private string closeDoorSound = "close_door";

    private Door otherDoorRef;
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
        openRotation = closedRotation * getAxisOpenAngle(false);

        if (isDoubleDoor && otherDoor != null)
        {
            otherDoorRef = otherDoor.GetComponent<Door>();
            otherDoorClosedRotation = otherDoor.rotation;
            // Other door opens in opposite direction
            otherDoorOpenRotation = otherDoorClosedRotation * getAxisOpenAngle(true);
        }


        if (transform.parent.TryGetComponent<NavMeshObstacle>(out navMeshObst))
        {
            navMeshObst.enabled = true;
        }
    }

    public override void OnInteract()
    {
        // If the door requires a key, check if player has it
        if (requireKey)
        {
            if (!PlayerInventory.HasKey(requiredKeyName))
            {
                SoundManager.Instance.PlaySFX(closedDoorSound);

                if (requiredKeyName == "Locked")
                {
                    UIManager.Instance.ShowMessage($"Locked.");
                } else
                {
                    UIManager.Instance.ShowMessage($"{requiredKeyName} key required to open this door.");
                }

                return;
            }
            // After opening a door for the 1st time with a key, you dont need the key anymore
            requireKey = false;
            if (isDoubleDoor && otherDoor != null)
                otherDoorRef.requireKey = false;
        }

        // If we reach here, either no key is required or the player has the correct key

        if (!isMoving)
        {
            isOpen = !isOpen;
            if (isDoubleDoor && otherDoor != null)
                otherDoorRef.isOpen = isOpen;
            StartCoroutine(RotateDoors());
        }
    }

    private IEnumerator RotateDoors()
    {
        isMoving = true;
        if (isDoubleDoor && otherDoor != null)
            otherDoorRef.isMoving = true;

        SoundManager.Instance.PlaySFX(isOpen ? openDoorSound : closeDoorSound);

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

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;

        Quaternion otherStartRotation = Quaternion.identity;
        Quaternion otherTargetRotation = Quaternion.identity;

        if (isDoubleDoor && otherDoor != null)
        {
            otherStartRotation = otherDoor.rotation;
            otherTargetRotation = isOpen ? otherDoorOpenRotation : otherDoorClosedRotation;
        }

        float duration = 2f / openSpeed; // Total animation duration
        float elapsed = 0f;

        bool navMeshChanged = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Smooth progression using an easing function
            float smoothStep = Mathf.SmoothStep(0f, 1f, t);

            // Interpolate main door rotation
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, smoothStep);

            // Interpolate other door rotation if it's a double door
            if (isDoubleDoor && otherDoor != null)
            {
                otherDoor.rotation = Quaternion.Slerp(otherStartRotation, otherTargetRotation, smoothStep);
            }

            // Enable colliders near the end of the animation
            if (t >= 1f - (colliderEnableThreshold / 90f))
            {
                if (!mainCollider.enabled) mainCollider.enabled = true;
                if (otherCollider != null && !otherCollider.enabled) otherCollider.enabled = true;
            }

            // Toggle NavMeshObstacle if needed
            if (!navMeshChanged && t >= 0.6f && navMeshObst != null)
            {
                navMeshObst.enabled = !navMeshObst.enabled;
                navMeshChanged = true;
            }

            yield return null;
        }

        // Snap to final target rotations
        transform.rotation = targetRotation;
        if (isDoubleDoor && otherDoor != null)
        {
            otherDoor.rotation = otherTargetRotation;
        }

        isMoving = false;
        if (isDoubleDoor && otherDoor != null)
            otherDoorRef.isMoving = false;
    }

    public override void OnFocus()
    {
        if (shouldDoorBeOutlined) base.OnFocus();
    }

    public override void OnLoseFocus()
    {
        if (shouldDoorBeOutlined) base.OnLoseFocus();
    }

    private Quaternion getAxisOpenAngle(bool otherDoor)
    {
        Quaternion axisOpenAngle = Quaternion.identity;
        float angle = otherDoor ? -openAngle : openAngle;

        switch (openAxis)
        {
            case Axis.X:
                axisOpenAngle = Quaternion.Euler(angle, 0, 0);
                break;
            case Axis.Y:
                axisOpenAngle = Quaternion.Euler(0, angle, 0);
                break;
            case Axis.Z:
                axisOpenAngle = Quaternion.Euler(0, 0, angle);
                break;
            default:
                break;
        }

        return axisOpenAngle;
    }
}