using UnityEngine;

public class FoodCan : Interactable
{
    [Header("Food Can Settings")]
    [SerializeField] private float healAmount = 20f;
    [SerializeField] private string eatingSound = "eat";

    public override void OnFocus()
    {
        base.OnFocus();
        UIManager.Instance.ShowMessage("Eat.");
    }

    public override void OnLoseFocus()
    {
        base.OnLoseFocus();
        UIManager.Instance.HideMessage();
    }

    public override void OnInteract()
    {
        // 1) Heal the player
        FirstPersonController player = FindObjectOfType<FirstPersonController>();
        if (player != null)
        {
            player.Heal(healAmount);
        }

        // 2) Play eating sound
        SoundManager.Instance.PlaySFX(eatingSound);

        // 3) Destroy this object so it can't be used repeatedly
        Destroy(gameObject);
    }
}
