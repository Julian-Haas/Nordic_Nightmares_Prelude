using UnityEngine;

public class N_00_RegularState : NaddiBaseState
{  
    public N_00_RegularState(NaddiStateMachine stateMachine) : base(stateMachine, NaddiStates.Regular)    {    }

    public override void Enter()
    {
        _speed = stateMachine.RegularSpeed;
        _target = stateMachine.currentPath.GetPathFollower(_speed);
    }

    public override void Tick()
    {
        if(stateMachine._switchToNewPath) { SwitchToNewLocation(); }
        ContinueOnPath();
        FollowTarget(_target.transform.position);
        stateMachine.SwitchStateByAwareness(this);
    }

    public override void Exit()
    {

    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "NaddiPause")
        {
            stateMachine.SwitchState(new N_10_PauseState(stateMachine));
            Debug.Log("PAUSE");
        }
    }
}