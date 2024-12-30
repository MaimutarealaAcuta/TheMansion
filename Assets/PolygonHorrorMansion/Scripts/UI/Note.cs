using UnityEngine;

public class Note : Interactable
{
    [Header("Note Settings")]
    [SerializeField, TextArea(3, 5)]
    private string noteText = "Default Note Text";

    public override void OnInteract()
    {
        NoteUIManager.Instance.ShowNote(noteText);
    }

    public override void OnFocus()
    {
        base.OnFocus();
    }

    public override void OnLoseFocus()
    {
        base.OnLoseFocus();
    }
}
