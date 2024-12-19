using UnityEngine;
using UnityEngine.UI;

public class Wood : Interactable
{
    [SerializeField] private int woodLogsCount = 5;

    public override void OnInteract()
    {
        if (PlayerInventory.CanGatherWood())
        {
            PlayerInventory.GatherWood(woodLogsCount);
            Destroy(gameObject);
        }
        else
        {
            UIManager.Instance.ShowMessage("Cannot hold more wood!");
        }
        
    }

    public override void OnFocus()
    {
        if (UIManager.Instance.IsDisplayingMessage) return;

        UIManager.Instance.ShowMessage("Gather Wood");
    }

    public override void OnLoseFocus()
    {
        UIManager.Instance.HideMessage();
    }
}
