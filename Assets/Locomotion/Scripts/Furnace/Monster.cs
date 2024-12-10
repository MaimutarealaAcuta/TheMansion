using UnityEngine;
using System;

public class Monster : Interactable
{
    public static event Action OnMonsterCry;
    public static event Action OnMonsterScream;

    private bool isCarried = false;
    private bool isBurned = false;

    [Header("Monster Settings")]
    [SerializeField] private string monsterName = "Small Monster";

    // Audio references for crying/screaming
    [SerializeField] private AudioClip crySound;
    [SerializeField] private AudioClip screamSound;
    [SerializeField] private AudioSource audioSource;

    public override void OnFocus()
    {
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
            // Pick up the monster
            isCarried = true;
            PlayerInventory.CurrentHeldObject = this.gameObject;

            // Monster cries upon being picked up
            OnMonsterCry?.Invoke();

            // Play cry sound if available
            if (audioSource && crySound)
                audioSource.PlayOneShot(crySound);

            // Disable collider so it doesn’t interact with environment while carried
            Collider col = GetComponent<Collider>();
            if (col) col.enabled = false;

            // Optional: visually move the monster in front of the player’s camera/hands
            // Implement a method to position it in player’s view
            AttachToPlayerHands();
        }
    }

    private void AttachToPlayerHands()
    {
        // A transform on the player called "HoldPoint"
        // Transform holdPoint = PlayerInventory.HoldPoint;
        // transform.parent = holdPoint;
        // transform.localPosition = Vector3.zero;
        // transform.localRotation = Quaternion.identity;
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

            // Destroy the monster after screaming (or after a delay)
            Destroy(gameObject, 1f);
        }
    }
}
