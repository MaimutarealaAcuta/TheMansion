public class PlayingState : IGameState
{
    private GameStateManager gameManager;
    private readonly string bgMusicName = "bg_music";

    public void EnterState(GameStateManager gameManager)
    {
        this.gameManager = gameManager;

        SoundManager.Instance.PlayBGM(bgMusicName);
    }

    public void UpdateState()
    {
        // Handle game logic while playing
    }

    public void ExitState()
    {
        SoundManager.Instance.StopBGM();
    }
}
