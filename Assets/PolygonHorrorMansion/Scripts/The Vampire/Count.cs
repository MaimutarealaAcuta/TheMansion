using UnityEngine;

public class Count : Interactable
{
    // Conditions: Player needs hasWoodenSpike = true and monstersBurned >= 8 to kill the Count.
    // Otherwise, the Count kills the player by dealing damage.

    public override void OnFocus()
    {
        // Check inventory state and show appropriate message.
        if (!PlayerInventory.hasWoodenSpike)
        {
            // No spike
            UIManager.Instance.ShowMessage("You need something sharp to stab the Count");
        }
        else
        {
            //// Player has a spike
            //if (PlayerInventory.monstersBurned < 8)
            //{
            //    // Not enough monsters burned -> Count is invincible
            //    UIManager.Instance.ShowMessage("The Count seems invincible. Burn more monsters!");
            //}
            //else
            //{
                // Conditions met: Player can kill the Count
                UIManager.Instance.ShowMessage("Press Interact to stab the Count");
            //}
        }
    }

    public override void OnLoseFocus()
    {
        UIManager.Instance.HideMessage();
    }

    public override void OnInteract()
    {
        // Check conditions again upon interact
        if (!PlayerInventory.hasWoodenSpike)
        {
            // No spike: Count kills you
            KillPlayer();
        }
        else
        {
            // Player has spike
            if (PlayerInventory.monstersBurned < 8)
            {
                // Not enough monsters burned: Count kills you
                KillPlayer();
            }
            else
            {
                // Conditions are met: Kill the Count
                UIManager.Instance.ShowMessage("You have slain the Count!");
                Destroy(gameObject);
            }
        }
    }

    private void KillPlayer()
    {
        FirstPersonController player = FindObjectOfType<FirstPersonController>();
        if (player != null)
        {
            UIManager.Instance.HideMessage();
            player.ApplyDamage(10); // This will kill the player
        }
    }
}
