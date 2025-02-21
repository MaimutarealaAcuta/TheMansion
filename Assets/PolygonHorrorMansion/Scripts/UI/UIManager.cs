using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Info")]
    [SerializeField] private TextMeshProUGUI healthText = default;
    [SerializeField] private TextMeshProUGUI staminaText = default;
    [SerializeField] private TextMeshProUGUI woodText = default;


    [SerializeField] private Image healthBar = default;
    [SerializeField] private Image staminaBar = default;
    [SerializeField] private RectMask2D staminaMask = default;
    private float maxRightMask;
    private float initialRightMask;

    [SerializeField] Volume postProcessingVolume = default;

    [Header("Message Display Settings")]
    [SerializeField] private TextMeshProUGUI infoMessage;
    [SerializeField] private float fadeInDuration = 0.4f;   // Time to fade in
    [SerializeField] private float displayDuration = 1.7f;    // Time text stays visible
    [SerializeField] private float fadeOutDuration = 0.4f;  // Time to fade out

    [Header("Pause Game Settings")]
    [SerializeField] private GameObject pausePanel;

    [Header("Stats sprites")]
    [SerializeField] private Sprite[] healthSprites;


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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameStateManager.OnGameOver += ShowGameOverPanel;
        GameStateManager.OnGameRestart += HideGameOverPanel;
        FirstPersonController.OnDamage += UpdateHealth;
        FirstPersonController.OnHeal += UpdateHealth;
        FirstPersonController.OnStaminaChange += UpdateStamina;
        PlayerInventory.OnWoodCountChange += UpdateWoodCount;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameOver -= ShowGameOverPanel;
        GameStateManager.OnGameRestart -= HideGameOverPanel;
        FirstPersonController.OnDamage -= UpdateHealth;
        FirstPersonController.OnHeal -= UpdateHealth;
        FirstPersonController.OnStaminaChange -= UpdateStamina;
        PlayerInventory.OnWoodCountChange -= UpdateWoodCount;
    }

    private void Start()
    {
        UpdateHealth(3);
        UpdateStamina(100);
        UpdateWoodCount(0);

        //pausePanel.SetActive(false);

        // Get the initial and max right mask values
        maxRightMask = staminaMask.rectTransform.rect.width - staminaMask.padding.x - staminaMask.padding.z;
        initialRightMask = staminaMask.rectTransform.offsetMax.x;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausePanel();
        }
    }

    private void UpdateHealth(float currentHealth)
    {
        //healthText.text = "Health: " + currentHealth.ToString();
        healthBar.sprite = healthSprites[(int)currentHealth];

        // change color adjustment saturation based on health
        ColorAdjustments colorAdjustments;
        if(postProcessingVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
            colorAdjustments.saturation.Override(1 - currentHealth / 3);
    }

    private void UpdateStamina(float currentStamina)
    {
        //staminaText.text = "Stamina: " + currentStamina.ToString("00");
        float newRightMask = maxRightMask * (1 - currentStamina / 100);
        staminaMask.padding = new Vector4(0, 0, newRightMask, 0);

        // add chromatic aberation effect when stamina is low
        ChromaticAberration chromaticAberration;
        if(postProcessingVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration))
            chromaticAberration.intensity.Override(1 - currentStamina / 100);

    }

    private void UpdateWoodCount(int woodCount)
    {
        woodText.text = "Wood: " + woodCount.ToString();
    }

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

    // pause logic
    public void TogglePausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
            // Pause the game when the pause panel is active
            Time.timeScale = pausePanel.activeSelf ? 0f : 1f;
            // enable mouse cursor
            Cursor.visible = pausePanel.activeSelf;
            Cursor.lockState = pausePanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    // return to main menu
    public void Quit()
    {
        // Unpause the game before quitting
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);   
    }
}
