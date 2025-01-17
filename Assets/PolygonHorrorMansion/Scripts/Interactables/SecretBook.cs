using UnityEngine;

public class SecretBook : Interactable
{
    [Header("Secret Book Settings")]
    [SerializeField] private Door secretDoor;

    [SerializeField] private string focusMessage = "Push.";

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
        // Instead of opening this object, we call the door's interaction logic
        if (secretDoor != null)
        {
            secretDoor.OnInteract();
        }
        else
        {
            Debug.LogWarning("SecretBook: No door is assigned to be toggled!");
        }
    }
}

