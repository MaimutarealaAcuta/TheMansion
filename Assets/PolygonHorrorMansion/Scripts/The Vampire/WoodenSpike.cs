using UnityEngine;

public class WoodenSpike : Interactable
{
    public override void OnFocus()
    {
        base.OnFocus();
        UIManager.Instance.ShowMessage("Take wooden spike.");
    }

    public override void OnLoseFocus()
    {
        base.OnLoseFocus();
        UIManager.Instance.HideMessage();
    }

    public override void OnInteract()
    {
        PlayerInventory.hasWoodenSpike = true;
        UIManager.Instance.ShowMessage("You Know What to Do!");
        Destroy(gameObject);
    }
}
