using UnityEngine;

public class N_10_PauseState : NaddiBaseState
{
    private float _timePaused = 0.0f;
    public N_10_PauseState(NaddiStateMachine stateMachine) : base(stateMachine, NaddiStates.Pause)   {       }

    public override void Enter()
    {
        _speed = 0.0f;
        stateMachine.currentPath.StopPathWalking();
        stateMachine.timeIdled = 0.0f;
        stateMachine.PlaySound("event:/SFX/NaddiIdle");
        //Debug.Log("ENTER PAUSE STATE");
    }

    public override void Tick()
    {
        //ContinuePause();
        PauseAtPauseLocation();
    }

    public override void Exit() {
        //Debug.Log("LEAVE PAUSE STATE");
    }

    private void ContinuePause()
    {
        _timePaused += Time.deltaTime;
        NaddiBaseState checkState = stateMachine.GetCurrentStateByAwareness();
        if ( checkState.GetType().Name == "N_00_RegularState" &&  _timePaused > stateMachine.PauseRegular) { 
            stateMachine.SwitchState(checkState);
        }
        else if( checkState.GetType().Name == "N_01_SuspiciousState" && _timePaused > stateMachine.PauseSuspicious) { 
            stateMachine.SwitchState(checkState);
        }
        else if ( checkState.GetType().Name != "N_00_RegularState" &&  checkState.GetType().Name != "N_01_SuspiciousState")
        {
            stateMachine.SwitchState(checkState);
        }
    }
}