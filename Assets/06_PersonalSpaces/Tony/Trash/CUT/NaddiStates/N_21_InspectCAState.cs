using UnityEngine;

public class N_21_InspectCAState : NaddiBaseState
{
    private bool _returnToPath = false;

    public N_21_InspectCAState(NaddiStateMachine stateMachine) : base(stateMachine, NaddiStates.InspectCA) {    }

    public override void Enter()
    {
        _target = stateMachine.ConspicousArea;
        stateMachine._registeredSomething = false;
        _speed = (stateMachine.AlertSpeed + stateMachine.AwareSpeed ) * 0.5f;
        stateMachine.currentPath.StopPathWalking();
        stateMachine.timeIdled = 0.0f;
        stateMachine.PlaySound("event:/SFX/NaddiIdle");
    }

    public override void Tick()
    {
        InspectCA();
        if(stateMachine.GetCurrentStateByAwareness()._state == NaddiStates.Hunt || stateMachine.GetCurrentStateByAwareness()._state == NaddiStates.Suspicious)
        {
            stateMachine.SwitchStateByAwareness(this);
        }
        /*
        if (stateMachine._registeredSomething) { InspectWhatGotRegistered(); }
        if(!_returnToPath ) { FollowTarget(_target.transform.position); }
        else if(_returnToPath) { ReturnToPath(_speed); }
         */
        
    }

    public override void Exit()
    {
        stateMachine.currentPath.ContinueOnPath();
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "NaddiConspicousArea")
        {
            _speed = 0.0f;
            _returnToPath = true;
        }
    }
}