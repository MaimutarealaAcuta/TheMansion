public class PlayingState : IGameState
{
    private GameStateManager gameManager;

    public void EnterState(GameStateManager gameManager)
    {
        this.gameManager = gameManager;
        // Any setup for playing state
    }

    public void UpdateState()
    {
        // Handle game logic while playing
    }

    public void ExitState()
    {
        // Any cleanup when exiting playing state
    }
}
