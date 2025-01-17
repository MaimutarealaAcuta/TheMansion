using UnityEngine;

public class Piano : Interactable
{
    [Header("Piano Settings")]
    [SerializeField] private string pianoSound = "piano";  // Reference to an AudioSource on this object

    [SerializeField, TextArea(1, 2)]
    private string focusMessage = "Press.";

    public override void OnFocus()
    {
        base.OnFocus();
        UIManager.Instance.ShowMessage(focusMessage);
    }

    public override void OnLoseFocus()
    {
        base.OnLoseFocus();
        UIManager.Instance.HideMessage();
    }

    public override void OnInteract()
    {
        SoundManager.Instance.PlaySFX(pianoSound);
    }
}
