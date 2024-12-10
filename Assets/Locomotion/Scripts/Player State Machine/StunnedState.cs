using System.Collections;
using UnityEngine;

public class StunnedState : IPlayerState
{
    private FirstPersonController player;
    private Coroutine stunCoroutine;
    private Coroutine flickerCoroutine;

    public void EnterState(FirstPersonController player)
    {
        this.player = player;
        player.CanMove = false;

        // Enable the stun message text and start flickering
        if (player.stunMessageText != null)
        {
            player.stunMessageText.gameObject.SetActive(true);
            player.stunMessageText.enabled = true;
            SetTextAlpha(1f); // Ensure full visibility at the start
            flickerCoroutine = player.StartCoroutine(FlickerText());
        }

        // Start the stun handling coroutine
        stunCoroutine = player.StartCoroutine(HandleStun());
    }

    public void UpdateState()
    {
        // Handle inputs for escaping the stun
        // Managed within the coroutine
    }

    public void ExitState()
    {
        // Cleanup when exiting Stunned state
        if (stunCoroutine != null)
        {
            player.StopCoroutine(stunCoroutine);
            stunCoroutine = null;
        }

        // Stop the flicker coroutine and disable the text
        if (player.stunMessageText != null)
        {
            if (flickerCoroutine != null)
            {
                player.StopCoroutine(flickerCoroutine);
                flickerCoroutine = null;
            }
            player.stunMessageText.enabled = false;
            player.stunMessageText.gameObject.SetActive(false);
        }
    }

    private IEnumerator HandleStun()
    {
        if (player.currentHealth <= 0)
        {
            player.SetPlayerState(new DeadState());
            yield break;
        }

        int escapePressCount = 0;
        const int requiredPresses = 10;
        float stunDuration = 5f;
        float timer = 0f;
        bool escaped = false;

        // To Do: Display UI prompt for escaping, e.g., "Press E rapidly to escape"

        while (timer < stunDuration)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                escapePressCount++;
                if (escapePressCount >= requiredPresses)
                {
                    escaped = true;
                    break;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (escaped)
        {
            player.SetPlayerState(new NeutralState());
        }
        else
        {
            // Player failed to escape, apply additional penalties
            player.ApplyDamage(1);
        }
    }

    private IEnumerator FlickerText()
    {
        float flickerDuration = 0.5f; // Total duration of one flicker cycle
        float halfDuration = flickerDuration / 2f;
        while (true)
        {
            // Fade out
            float timer = 0f;
            while (timer < halfDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, timer / halfDuration);
                SetTextAlpha(alpha);
                yield return null;
            }

            // Fade in
            timer = 0f;
            while (timer < halfDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, timer / halfDuration);
                SetTextAlpha(alpha);
                yield return null;
            }
        }
    }

    private void SetTextAlpha(float alpha)
    {
        Color color = player.stunMessageText.color;
        color.a = alpha;
        player.stunMessageText.color = color;
    }
}
