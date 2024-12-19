using UnityEngine;

public class Furnace : Interactable
{
    [Header("Fireplace Settings")]
    [SerializeField] public Door furnanceDoor;
    [SerializeField] private bool isLoaded = false;
    [SerializeField] private GameObject woodStack;
    [SerializeField] private GameObject fireLight;

    [Header("Wood Requirements")]
    [SerializeField] private int requiredWood = 6;

    public override void OnFocus()
    {
        if (UIManager.Instance.IsDisplayingMessage) return;

        bool doorIsOpen = furnanceDoor.IsOpen;

        if (!doorIsOpen)
        {
            UIManager.Instance.ShowMessage("The furnace door is closed. Open it first!");
        }
        else
        {
            // If the door is open:
            if (!isLoaded)
            {
                UIManager.Instance.ShowMessage("Interact to load wood");
            }
            else
            {
                UIManager.Instance.ShowMessage("Interact to burn monster");
            }
        }
    }

    public override void OnLoseFocus()
    {
        UIManager.Instance.HideMessage();
    }

    public override void OnInteract()
    {
        bool doorIsOpen = furnanceDoor.IsOpen;

        // If the furnace door is closed, don't allow loading wood or burning monsters
        if (!doorIsOpen)
        {
            UIManager.Instance.ShowMessage("The furnace door is closed!");
            return;
        }

        if (!isLoaded)
        {
            // Attempt to load wood if the door is open
            if (PlayerInventory.HasWood(requiredWood))
            {
                PlayerInventory.UseWood(requiredWood);
                LoadFireplace();
            }
            else
            {
                UIManager.Instance.ShowMessage("Not enough wood to load!");
            }
        }
        else
        {
            // Fireplace is loaded, check if the player is holding a monster
            GameObject heldObject = PlayerInventory.CurrentHeldObject;
            if (heldObject != null)
            {
                Monster monster = heldObject.GetComponent<Monster>();
                if (monster != null)
                {
                    BurnMonster(monster);
                }
                else
                {
                    UIManager.Instance.ShowMessage("You need a monster to burn!");
                }
            }
            else
            {
                UIManager.Instance.ShowMessage("You need a monster to burn!");
            }
        }
    }

    private void LoadFireplace()
    {
        isLoaded = true;
        UpdateVisuals();

        // TO DO: Lay a sound or particle effect for ignition
    }

    private void BurnMonster(Monster monster)
    {
        monster.Burn();
        isLoaded = false;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (woodStack != null)
        {
            woodStack.SetActive(isLoaded);
            fireLight.SetActive(isLoaded);
        }
    }
}
