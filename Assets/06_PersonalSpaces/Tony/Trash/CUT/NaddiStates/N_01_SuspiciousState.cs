using UnityEngine;

public class N_01_SuspiciousState : NaddiBaseState
{
    public N_01_SuspiciousState(NaddiStateMachine stateMachine) : base(stateMachine, NaddiStates.Suspicious)   {    }

    public override void Enter()
    {
        _speed = stateMachine.SuspiciousSpeed;
        _target = stateMachine.currentPath.GetPathFollower(_speed);
    }

    public override void Tick()
    {
        stateMachine.SwitchStateByAwareness(this);
        if(stateMachine._switchToNewPath) { SwitchToNewLocation(); }
        ContinueOnPath();
        FollowTarget(_target.transform.position);
    }

    public override void Exit()   {    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "NaddiPause")
        {
            stateMachine.SwitchState(new N_10_PauseState(stateMachine));
        }
        else if (collider.gameObject.tag == "NaddiPointOfInterest")
        {
            stateMachine.SwitchState(new N_20_InspectPOIState(stateMachine));
        }
    }
}