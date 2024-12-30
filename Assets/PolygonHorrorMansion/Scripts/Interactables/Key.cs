using UnityEngine;

public class Key : Interactable
{
    [SerializeField] private string keyName = "Main Door";
    [SerializeField] private string keyPickupSound = "key_pickup";

    public override void OnInteract()
    {
        PlayerInventory.AddKey(keyName);

        SoundManager.Instance.PlaySFX(keyPickupSound);

        gameObject.SetActive(false);
    }

    public override void OnFocus()
    {
        base.OnFocus();

        UIManager.Instance.ShowMessage($"{keyName} Key");
    }

    public override void OnLoseFocus()
    {
        base.OnLoseFocus();

        UIManager.Instance.HideMessage();
    }
}
