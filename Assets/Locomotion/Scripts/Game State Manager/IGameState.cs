
public interface IGameState
{
    void EnterState(GameStateManager gameManager);
    void UpdateState();
    void ExitState();
}
