using UnityEngine;

public class DeadState : IPlayerState
{
    private FirstPersonController player;

    public void EnterState(FirstPersonController player)
    {
        this.player = player;
        player.CanMove = false;
        // Trigger death animation, sounds, etc.
        Debug.Log("Player has died.");
        // Notify GameStateManager
        GameStateManager.Instance.PlayerDied();
    }

    public void UpdateState()
    {
        // Handle death screen inputs (e.g., restart game)
    }

    public void ExitState()
    {
        // Any cleanup when exiting Dead state
    }
}
