using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class N_20_InspectPOIState : NaddiBaseState
{
    private float _timeInspectedPOI = 0.0f;
    public N_20_InspectPOIState(NaddiStateMachine stateMachine) : base(stateMachine, NaddiStates.InspectPOI) {   }

    public override void Enter() 
    {
        _speed = 0.0f;
        stateMachine.currentPath.StopPathWalking();
        stateMachine.timeIdled = 0.0f;
        stateMachine.PlaySound("event:/SFX/NaddiIdle");
    }

    public override void Tick()
    {
        ContinueInspectionOfPOI();
    }

    public override void Exit()
    {
        stateMachine.currentPath.ContinueOnPath();
    }

    private void ContinueInspectionOfPOI()
    {
        _timeInspectedPOI += Time.deltaTime;
        NaddiBaseState checkState = stateMachine.GetCurrentStateByAwareness();
        if(checkState._state == NaddiStates.Suspicious && _timeInspectedPOI > stateMachine.InspectPOISuspicious)
        {
            stateMachine.SwitchState(checkState);
        }
        else if (checkState._state != NaddiStates.Suspicious && _timeInspectedPOI > stateMachine.InspectPOIAlert)
        {
            stateMachine.SwitchState(checkState);
        }
    }
}