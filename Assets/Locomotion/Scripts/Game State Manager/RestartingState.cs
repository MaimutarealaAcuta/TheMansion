using UnityEngine.SceneManagement;

public class RestartingState : IGameState
{
    private GameStateManager gameManager;

    public void EnterState(GameStateManager gameManager)
    {
        this.gameManager = gameManager;
        // Reload the scene or reset game variables
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateState()
    {
        // No update needed during restarting
    }

    public void ExitState()
    {
        // Any cleanup after restarting
    }
}
