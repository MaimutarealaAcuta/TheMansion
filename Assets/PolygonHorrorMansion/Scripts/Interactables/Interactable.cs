using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private Outline outlineComponent;

    private void Awake()
    {
        gameObject.layer = 7; // Interactable layer, check Unity inspector for all the layers

        // Force the game object to have an Outline at runtime
        if (!TryGetComponent(out outlineComponent))
        {
            outlineComponent = gameObject.AddComponent<Outline>();
            outlineComponent.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        }

        outlineComponent.OutlineColor = new Color(211/255f, 211/255f, 211/255f);

        outlineComponent.enabled = false;
    }

    public abstract void OnInteract();

    public virtual void OnFocus()
    {
        if (outlineComponent != null)
        {
            outlineComponent.enabled = true;
        }
    }

    public virtual void OnLoseFocus()
    {
        if (outlineComponent != null)
        {
            outlineComponent.enabled = false;
        }
    }
}
