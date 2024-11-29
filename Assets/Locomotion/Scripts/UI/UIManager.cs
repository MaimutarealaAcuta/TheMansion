using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverPanel;

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
