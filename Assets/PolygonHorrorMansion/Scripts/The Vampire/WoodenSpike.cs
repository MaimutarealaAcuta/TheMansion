using UnityEngine;

public class WoodenSpike : Interactable
{
    public override void OnFocus()
    {
        UIManager.Instance.ShowMessage("Press Interact to pick up wooden spike");
    }

    public override void OnLoseFocus()
    {
        UIManager.Instance.HideMessage();
    }

    public override void OnInteract()
    {
        PlayerInventory.hasWoodenSpike = true;
        UIManager.Instance.ShowMessage("You picked up a wooden spike!");
        Destroy(gameObject);
    }
}
