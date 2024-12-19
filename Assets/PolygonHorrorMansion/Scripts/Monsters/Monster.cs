using UnityEngine;
using System;

public class Monster : Interactable, IItem
{
    public static event Action OnMonsterCry;
    public static event Action OnMonsterScream;
    public static event Action<Vector3> OnMonsterCarriedUpdate;

    public FirstPersonController player;

    private bool isCarried = false;
    private bool isBurned = false;
    private Collider col;
    private Rigidbody rb;

    [Header("Monster Settings")]
    [SerializeField] private string monsterName = "Small Monster";

    // Audio references for crying/screaming
    [SerializeField] private AudioClip crySound;
    [SerializeField] private AudioClip screamSound;
    [SerializeField] private AudioSource audioSource;

    private MonsterPatrol patrol;
    private bool updatePositionRunning = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = rb.GetComponent<Collider>();
        patrol = GetComponent<MonsterPatrol>();
    }

    public override void OnFocus()
    {
        if (UIManager.Instance.IsDisplayingMessage) return;

        UIManager.Instance.ShowMessage("Interact to pick up monster");
    }

    public override void OnLoseFocus()
    {
        UIManager.Instance.HideMessage();
    }

    public override void OnInteract()
    {
        if (!isCarried && !isBurned)
        {
            if (PlayerInventory.CurrentHeldObject)
            {
                UIManager.Instance.ShowMessage("You cannot carry more than one 'Minion'");
                return;
            }

            // Pick up the monster
            isCarried = true;
            PlayerInventory.CurrentHeldObject = this.gameObject;

            // Monster cries upon being picked up
            OnMonsterCry?.Invoke();

            // Play cry sound if available
            if (audioSource && crySound)
                audioSource.PlayOneShot(crySound);

            // Stop movement via patrol system
            if (patrol) patrol.PauseMovement();

            // Disable collider so it doesn’t interact with environment while carried
            if (col) col.enabled = false;

            // Disable rigid body, so it does not fall from gravity
            if (rb)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            // Visually move the monster in front of the player’s camera
            AttachToPlayerHands();

            // Start sending player's position updates
            StartUpdatingPosition();
        }
    }

    private void AttachToPlayerHands()
    {
        Transform holdPoint = player.holdPoint;
        transform.parent = holdPoint;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnDrop()
    {
        if (isCarried && !isBurned)
        {
            isCarried = false;
            transform.parent = null;
            PlayerInventory.CurrentHeldObject = null;

            // Re-enable the collider
            if (col) col.enabled = true;

            if (rb)
            {
                rb.isKinematic = false; // Allow physics to move it
                rb.useGravity = true;   // Ensure gravity is on
            }

            // Place the monster slightly in front of the player at ground level
            Vector3 dropPosition = player.transform.position + player.transform.forward * 1f;
            dropPosition.y = player.transform.position.y; // Same height as player feet
            transform.position = dropPosition;

            // Stop updating position once dropped
            StopUpdatingPosition();

            // Run to next waypoint once dropped
            if (patrol) patrol.RunToNextWaypoint();
        }
    }

    public void Burn()
    {
        if (isCarried && !isBurned)
        {
            isBurned = true;
            isCarried = false;

            // Detach from player
            transform.parent = null;
            PlayerInventory.CurrentHeldObject = null;

            // Trigger scream event
            OnMonsterScream?.Invoke();

            // Play scream sound if available
            if (audioSource && screamSound)
                audioSource.PlayOneShot(screamSound);

            // Stop updating position once dropped
            StopUpdatingPosition();

            // Destroy the monster after screaming (or after a delay)
            Destroy(gameObject, .2f);
        }
    }

    public void OnTake()
    {
        throw new NotImplementedException();
    }

    private void StartUpdatingPosition()
    {
        if (!updatePositionRunning && player != null)
        {
            updatePositionRunning = true;
            StartCoroutine(UpdatePlayerPositionContinuously());
        }
    }

    private void StopUpdatingPosition()
    {
        updatePositionRunning = false;
        // The coroutine will exit once the boolean is false
    }

    private System.Collections.IEnumerator UpdatePlayerPositionContinuously()
    {
        while (updatePositionRunning && player != null)
        {
            // Invoke event with player’s current position
            OnMonsterCarriedUpdate?.Invoke(player.transform.position);
            yield return new WaitForSeconds(0.5f); // Update every half second
        }
    }
}
