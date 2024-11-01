using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractable : Interactable
{
    public override void OnFocus()
    {
        print("Loking at " + gameObject.name);
    }

    public override void OnInteract()
    {
        print("Interacted with " + gameObject.name);
    }

    public override void OnLoseFocus()
    {
        print("Stoped looking at " + gameObject.name);
    }
}
