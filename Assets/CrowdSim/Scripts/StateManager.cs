using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{

    public EnemyState currentState;

    // Update is called once per frame
    void Update()
    {
        //RunStateMachine(EnemyManager enemy);
    }

    //public void RunStateMachine(EnemyManager enemy)
    //{
    //    EnemyState nextState = currentState?.RunCurrentState(enemy);

    //    if(nextState != null)
    //    {
    //        SwitchToTheNextState(nextState);
    //    }
    //}

    //private void SwitchToTheNextState(EnemyState nextState)
    //{
    //    currentState = nextState;
    //}
}
