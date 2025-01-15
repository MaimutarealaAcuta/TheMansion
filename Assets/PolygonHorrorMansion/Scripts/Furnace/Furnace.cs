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

    [Header("Audio References")]
    [SerializeField] AudioSource furnaceSource;
    [SerializeField] AudioClip burningSound;
    [SerializeField] private string fireSound = "fire";
    [SerializeField] private string loadWoodSound = "load_wood";
    [SerializeField] private string cannotSound = "cannot_collect_wood";

    private void Start()
    {
        if (furnaceSource != null)
        {
            furnaceSource.clip = burningSound;
            furnaceSource.loop = true;
            furnaceSource.playOnAwake = false;
            furnaceSource.spatialBlend = 1f;
        }
    }

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
            SoundManager.Instance.PlaySFX(cannotSound);
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
                SoundManager.Instance.PlaySFX(cannotSound);
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
                    SoundManager.Instance.PlaySFX(cannotSound);
                    UIManager.Instance.ShowMessage("You need a monster to burn!");
                }
            }
            else
            {
                SoundManager.Instance.PlaySFX(cannotSound);
                UIManager.Instance.ShowMessage("You need a monster to burn!");
            }
        }
    }

    private void LoadFireplace()
    {
        isLoaded = true;

        SoundManager.Instance.PlaySFX(loadWoodSound);
        UpdateVisuals();

        Invoke(nameof(FireTheLogs), .3f); // Waits for .5 seconds
    }

    private void FireTheLogs()
    {
        SoundManager.Instance.PlaySFX(fireSound);
        PlayBurningAudio();
    }

    private void BurnMonster(Monster monster)
    {
        monster.Burn();
        isLoaded = false;

        Invoke(nameof(UpdateVisuals), 1f);
        StopBurningAudio();
    }

    private void UpdateVisuals()
    {
        if (woodStack != null)
        {
            woodStack.SetActive(isLoaded);
            fireLight.SetActive(isLoaded);
        }
    }

    void PlayBurningAudio()
    {
        if (furnaceSource != null)
        {
            furnaceSource.Play();
        }
    }

    void StopBurningAudio()
    {
        if (furnaceSource != null)
        {
            furnaceSource.Stop();
        }
    }
}
