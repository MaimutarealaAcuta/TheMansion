using System;
using System.Collections.Generic;
using UnityEngine;

public partial class StateMachine
{
    public readonly StatesEnum States;
    public readonly ActionsEnum Actions;

    private AbstractState currentState;
    private readonly FirstPersonController playerController;

    public StateMachine(FirstPersonController playerController)
    {
        this.playerController = playerController;

        Actions = new();
        States = new(this);

        currentState = (AbstractState)States.Neutral;
    }

    private void SwapState(IState newState)
    {
        if (newState is AbstractState state)
        {
            currentState.ExitState();
            currentState = state;
            state.EnterState();
        }
    }

    public void HandleAction(IAction action)
    {
        if (currentState.ActionHandlers.TryGetValue(action, out var handler))
        {
            handler.Invoke();
        }
    }

    public class ActionsEnum
    {
        public readonly IAction GetStunned = new EmptyAction();
        public readonly IAction Input = new EmptyAction();

        private class EmptyAction : IAction { };
    }

    public class StatesEnum
    {
        private readonly StateMachine stateMachine;

        public readonly IState Neutral;
        public readonly IState Stunned;

        internal StatesEnum(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;

            Neutral = new NeutralState(stateMachine);
            Stunned = new StunnedState(stateMachine);
        }
    }

    private abstract class AbstractState : IState
    {
        public Dictionary<IAction, Action> ActionHandlers { get; } = new();
        public abstract void EnterState();
        public abstract void ExitState();
    }
}

public interface IState { }

public interface IAction { }
