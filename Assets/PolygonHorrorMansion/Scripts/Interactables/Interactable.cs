using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private void Awake()
    {
        gameObject.layer = 7; // Interactable layer, check Unity inspector for all the layers
    }

    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnLoseFocus();
}
