using UnityEngine;
using TMPro;

public class NoteUIManager : MonoBehaviour
{
    public static NoteUIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject notePanel;       // Panel that displays the note
    [SerializeField] private TextMeshProUGUI noteTextUI; // Text component for the note's text
    [SerializeField] private GameObject crosshair;       // Reference to crosshair UI

    private bool isNoteOpen = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Ensure the note panel is hidden at start
        if (notePanel != null)
            notePanel.SetActive(false);
    }

    private void Update()
    {
        if (isNoteOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseNote();
        }
    }

    public void ShowNote(string text)
    {
        if (notePanel == null || noteTextUI == null)
            return;

        noteTextUI.text = text;
        notePanel.SetActive(true);
        isNoteOpen = true;

        // Hide crosshair if you want
        if (crosshair != null)
            crosshair.SetActive(false);

        // Disable player movement (and possibly mouse look)
        FirstPersonController player = FindObjectOfType<FirstPersonController>();
        if (player != null)
            player.CanMove = false;
    }

    public void CloseNote()
    {
        if (notePanel == null)
            return;

        notePanel.SetActive(false);
        isNoteOpen = false;

        // Re-enable crosshair
        if (crosshair != null)
            crosshair.SetActive(true);

        // Re-enable player movement
        FirstPersonController player = FindObjectOfType<FirstPersonController>();
        if (player != null)
            player.CanMove = true;
    }
}
