<<<<<<< HEAD
using System.Collections;
using TMPro;
=======
>>>>>>> 41b4171424be09a5b91d797761b44aa171482b6e
using UnityEngine;

public class UIManager : MonoBehaviour
{
<<<<<<< HEAD
    public static UIManager Instance { get; private set; }

    [Header("Message Display Settings")]
    [SerializeField] private TextMeshProUGUI infoMessage;
    [SerializeField] private float fadeInDuration = 0.5f;   // Time to fade in
    [SerializeField] private float displayDuration = 2f;    // Time text stays visible
    [SerializeField] private float fadeOutDuration = 0.5f;  // Time to fade out

    [Header("Game Over Settings")]
    public GameObject gameOverPanel;

    private Coroutine messageCoroutine;

    public bool IsDisplayingMessage
    {
        get { return messageCoroutine != null; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

=======
    public GameObject gameOverPanel;

>>>>>>> 41b4171424be09a5b91d797761b44aa171482b6e
    private void OnEnable()
    {
        GameStateManager.OnGameOver += ShowGameOverPanel;
        GameStateManager.OnGameRestart += HideGameOverPanel;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameOver -= ShowGameOverPanel;
        GameStateManager.OnGameRestart -= HideGameOverPanel;
    }

<<<<<<< HEAD
    /// <summary>
    /// Show a message on screen that fades in, stays for displayDuration, and then fades out.
    /// If this method is called again while a message is displaying, it overwrites the previous message.
    /// </summary>
    public void ShowMessage(string message)
    {
        // If another message is currently being displayed, stop it
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }

        messageCoroutine = StartCoroutine(ShowMessageRoutine(message));
    }

    /// <summary>
    /// Hide the currently displayed message immediately,
    /// stopping any fade-in/out coroutines and resetting the text.
    /// </summary>
    public void HideMessage()
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }

        // Immediately hide the text
        if (infoMessage != null)
        {
            SetTextAlpha(0f);
            infoMessage.gameObject.SetActive(false);
        }
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        // Ensure the text object is active
        infoMessage.gameObject.SetActive(true);
        infoMessage.text = message;

        // Start fully transparent
        SetTextAlpha(0f);

        // Fade in
        yield return StartCoroutine(FadeTextAlpha(0f, 1f, fadeInDuration));

        // Wait for the display duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        yield return StartCoroutine(FadeTextAlpha(1f, 0f, fadeOutDuration));

        // Hide the text object after fade out
        infoMessage.gameObject.SetActive(false);

        messageCoroutine = null;
    }

    private IEnumerator FadeTextAlpha(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, t);
            SetTextAlpha(currentAlpha);
            yield return null;
        }
    }

    private void SetTextAlpha(float alpha)
    {
        Color c = infoMessage.color;
        c.a = alpha;
        infoMessage.color = c;
    }

=======
>>>>>>> 41b4171424be09a5b91d797761b44aa171482b6e
    private void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void HideGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
}
