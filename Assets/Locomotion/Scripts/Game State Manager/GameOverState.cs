using System.Collections;
using UnityEngine;

public class GameOverState : IGameState
{
    private readonly float gameRestartDelay = 3f;
    private GameStateManager gameManager;

    public void EnterState(GameStateManager gameManager)
    {
        this.gameManager = gameManager;

        Debug.Log("Game Over!");

        gameManager.StartCoroutine(RestartGameAfterDelay(gameRestartDelay));
    }

    public void UpdateState()
    {
        // Handle inputs on the Game Over screen
    }

    public void ExitState()
    {
        // Any cleanup when exiting Game Over state
    }

    private IEnumerator RestartGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameManager.RestartGame();
    }
}
