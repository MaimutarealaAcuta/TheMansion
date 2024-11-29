using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public static event Action OnGameOver;
    public static event Action OnGameRestart;

    private IGameState currentGameState;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Set initial game state
        SetGameState(new PlayingState());
    }

    private void Update()
    {
        currentGameState.UpdateState();
    }

    public void SetGameState(IGameState newState)
    {
        if (currentGameState != null)
        {
            currentGameState.ExitState();
        }
        currentGameState = newState;
        currentGameState.EnterState(this);
    }

    public void PlayerDied()
    {
        SetGameState(new GameOverState());
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        SetGameState(new RestartingState());
        OnGameRestart?.Invoke();
    }
}
