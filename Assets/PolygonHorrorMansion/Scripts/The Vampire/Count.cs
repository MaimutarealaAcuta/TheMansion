using Unity.VisualScripting;
using UnityEngine;

public class Count : Interactable
{
    // Conditions: Player needs hasWoodenSpike = true and monstersBurned >= 8 to kill the Count.
    // Otherwise, the Count kills the player by dealing damage.

    [SerializeField] private Door mainGates;
    [SerializeField] private string deathSound = "count_death";
    [SerializeField] private string batsSound = "bats";
    [SerializeField] private GameObject flock;
    [SerializeField] private FlockManager flockManager;

    public override void OnFocus()
    {
        // Check inventory state and show appropriate message.
        if (!PlayerInventory.hasWoodenSpike)
        {
            // No spike
            UIManager.Instance.ShowMessage("Disturb.");
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
                UIManager.Instance.ShowMessage("Stab.");
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
                gameObject.SetActive(false);

                SoundManager.Instance.PlaySFX(deathSound);

                Invoke("OpenTheGates", 2.5f); // delay
            }
        }
    }

    private void KillPlayer()
    {
        UIManager.Instance.HideMessage();

        gameObject.SetActive(false);
        flock.SetActive(true);

        SoundManager.Instance.PlaySFX(batsSound);
        flockManager.ActivateGoAround();

        Invoke("GameOver", 5f);
    }

    private void GameOver()
    {
        FirstPersonController player = FindObjectOfType<FirstPersonController>();

        if (player != null)
        {
            player.ApplyDamage(10); // This will kill the player
        }
    }

    private void OpenTheGates()
    {
        GoalManager.Instance.CompleteGoal("KillTheCount");

        mainGates.requireKey = false;
        mainGates.OnInteract();
    }
}
