using System.Collections.Generic;
using System;

public partial class StateMachine
{
    private class StunnedState : AbstractState
    {
        private readonly StateMachine stateMachine;

        public StunnedState(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public override void EnterState()
        {
            throw new NotImplementedException();
        }

        public override void ExitState()
        {
            throw new NotImplementedException();
        }
    }

}