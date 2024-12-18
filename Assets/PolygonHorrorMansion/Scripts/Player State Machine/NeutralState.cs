
using UnityEngine;

public class NeutralState : IPlayerState
{
    private FirstPersonController player;
    private KeyCode dropKey = KeyCode.G;

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
            HandleOtherTypesOfInteraction();
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

    private void HandleOtherTypesOfInteraction()
    {
        if (Input.GetKeyDown(dropKey) && PlayerInventory.CurrentHeldObject != null)
        {
            PlayerInventory.CurrentHeldObject.TryGetComponent<IItem>(out IItem heldItem);
            heldItem?.OnDrop();
        }

    }
}
