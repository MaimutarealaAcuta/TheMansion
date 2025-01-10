using UnityEngine;

public class DebugControls : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            // Increase monster count
            PlayerInventory.monstersBurned++;
            Debug.Log("Increased monstersBurned to: " + PlayerInventory.monstersBurned);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            // Apply one damage to the player
            FirstPersonController player = FindObjectOfType<FirstPersonController>();
            if (player != null)
            {
                player.ApplyDamage(3);
            }
            else
            {
                Debug.LogWarning("No player found in the scene to apply damage to.");
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerInventory.AddKey("Outside Basement Door");
        }

    }
}
