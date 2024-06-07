using UnityEngine;

public class N_02_AlertState : NaddiBaseState
{
    public N_02_AlertState(NaddiStateMachine stateMachine) : base(stateMachine, NaddiStates.Alert)  {       }

    public override void Enter()
    {
        _speed = stateMachine.AlertSpeed;
        _target = stateMachine.currentPath.GetPathFollower(_speed);
    }

    public override void Tick()
    {
        stateMachine.SwitchStateByAwareness(this);
        if (stateMachine._switchToNewPath) { SwitchToNewLocation(); }
        if (stateMachine._registeredSomething) { InspectWhatGotRegistered(); }
        ContinueOnPath();
        FollowTarget(_target.transform.position);
    }

    public override void Exit()   {    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "NaddiPointOfInterest")
        {
            stateMachine.SwitchState(new N_20_InspectPOIState(stateMachine));
        }
    }
}