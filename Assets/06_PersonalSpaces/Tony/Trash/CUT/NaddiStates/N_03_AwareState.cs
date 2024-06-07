using UnityEngine;

public class N_03_AwareState : NaddiBaseState
{
    public N_03_AwareState(NaddiStateMachine stateMachine) : base(stateMachine, NaddiStates.Aware)   {     }

    public override void Enter()
    {
        _speed = stateMachine.AwareSpeed;
        _target = stateMachine.currentPath.GetPathFollower(_speed);
        if(Vector3.Magnitude(_target.transform.position - stateMachine.transform.position) > 5.0f)
        {
            stateMachine.currentPath.StopPathWalking();
        }
    }

    public override void Tick()
    {
        stateMachine.SwitchStateByAwareness(this);
        if (stateMachine._switchToNewPath) { SwitchToNewLocation(); }
        if (stateMachine._registeredSomething) { InspectWhatGotRegistered(); }

        if(Vector3.Magnitude(_target.transform.position - stateMachine.transform.position)> 5.0f)
        {
            ReturnToPath(_speed);
        }
        else
        {
            ContinueOnPath();
            FollowTarget(_target.transform.position);
        }
    }

    public override void Exit()
    {

    }
}