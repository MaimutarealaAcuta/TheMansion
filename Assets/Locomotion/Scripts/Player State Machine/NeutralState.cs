
public class NeutralState : IPlayerState
{
    private FirstPersonController player;

    public void EnterState(FirstPersonController player)
    {
        this.player = player;
        player.CanMove = true;
    }

    public void UpdateState()
    {
        // Handle inputs and actions specific to Neutral state
        player.HandleMovementInput();
        player.HandleMouseLook();

        if (player.canJump)
            player.HandleJump();

        if (player.canCrouch)
            player.HandleCrouch();

        if (player.canUseHeadbob)
            player.HandleHeadbob();

        if (player.useFootsteps)
            player.HandleFootsteps();

        if (player.canInteract)
        {
            player.HandleInteractionCheck();
            player.HandleInteractionInput();
        }

        if (player.useStamina)
        {
            player.HandleStamina();
        }

        player.ApplyFinalMovements();
    }

    public void ExitState()
    {
        // Any cleanup when exiting Neutral state
    }
}
