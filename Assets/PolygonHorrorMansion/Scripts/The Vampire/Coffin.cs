using System.Collections;
using UnityEngine;

public class Coffin : Interactable
{
    [Header("Coffin Settings")]
    [SerializeField] private Transform lidTransform;
    [SerializeField] private Rigidbody lidRigidbody;
    [SerializeField] private float lidNudgeAmount = .5f; // How much to move lid on X axis
    [SerializeField] private float moveDuration = 0.5f;   // Duration of the smooth move

    private bool hasBeenInteracted = false;

    public override void OnInteract()
    {
        if (hasBeenInteracted)
            return;

        hasBeenInteracted = true;

        // Start the coroutine to smoothly move the lid
        StartCoroutine(MoveLidAndDrop());
    }

    public override void OnFocus()
    {
        if (!hasBeenInteracted)
        {
            UIManager.Instance.ShowMessage("Interact to move the coffin lid");
        }
    }

    public override void OnLoseFocus()
    {
        UIManager.Instance.HideMessage();
    }

    private IEnumerator MoveLidAndDrop()
    {
        Vector3 startPos = lidTransform.localPosition;
        Vector3 endPos = new Vector3(startPos.x + lidNudgeAmount, startPos.y, startPos.z);

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            // Using Lerp for linear interpolation; if you really want Slerp, you can use:
            // lidTransform.localPosition = Vector3.Slerp(startPos, endPos, t);
            lidTransform.localPosition = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }
    }
}
