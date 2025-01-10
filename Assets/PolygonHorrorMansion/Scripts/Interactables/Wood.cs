using UnityEngine;
using UnityEngine.UI;

public class Wood : Interactable
{
    [SerializeField] private int woodLogsCount = 5;
    [SerializeField] private string collectWoodSound = "collect_wood";
    [SerializeField] private string cannotCollectWoodSound = "cannot_collect_wood";

    public override void OnInteract()
    {
        if (PlayerInventory.CanGatherWood())
        {
            PlayerInventory.GatherWood(woodLogsCount);
            SoundManager.Instance.PlaySFX(collectWoodSound);
            Destroy(gameObject);
        }
        else
        {
            SoundManager.Instance.PlaySFX(cannotCollectWoodSound);
            UIManager.Instance.ShowMessage("Cannot hold more wood!");
        }
        
    }

    public override void OnFocus()
    {
        base.OnFocus();

        if (UIManager.Instance.IsDisplayingMessage) return;

        UIManager.Instance.ShowMessage("Gather Wood");
    }

    public override void OnLoseFocus()
    {
        base.OnLoseFocus();

        UIManager.Instance.HideMessage();
    }
}
