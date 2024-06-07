using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 TO DO:
-stop hunting when player is in safeZone
-stop hunting when player is hidden && NOT MAD
-continue hunting when player is hidden && MAD

--> SafeZone > Mad > Hidden

--> walk up to certain distance and then stop and look around for a bit, until awareness dropped enough

Check if target is visible
If not, get last created reachable tracking point
--> if close enough, stop moving
--> else, move closer
--> move towards target
--> when close to target stop moving

 */

public class N_04_HuntState : NaddiBaseState
{
    private s_PlayerCollider _player;
    public N_04_HuntState(NaddiStateMachine stateMachine) : base(stateMachine, NaddiStates.Hunt)    {    }

    public override void Enter()
    {
        _speed = stateMachine.HuntSpeed;
        _target = GameObject.Find("PlayerAnimated");
        _player = _target.GetComponent<s_PlayerCollider>();
        stateMachine.currentPath.StopPathWalking();
        stateMachine.PlaySound("event:/SFX/NaddiAlert");
    }

    public override void Tick()
    {
        stateMachine.SwitchStateByAwareness(this);
        CheckPlayerStatus();
        TrackTarget();
        if ((_targetVisibility && CheckDistanceToTarget(_target.transform.position)) || (!_targetVisibility && CheckDistanceToTarget(_tempTarget)))
        {
            _speed = 0.0f;
            stateMachine.RotationSpeed = 0.0f;
        }
        else 
        {
            _speed = stateMachine.HuntSpeed;
            stateMachine.RotationSpeed = 5.0f;
        }
    }

    public override void Exit()
    {
        stateMachine._registeredSomething = false;
    }

    //stop walking towards Player if close enough AND:
    //Player is in safeZone OR  Player is hidden & NOT mad
    //otherwise move towards Player
    public void CheckPlayerStatus()
    {
        float targetDistance = Vector3.Distance(_target.transform.position, stateMachine.transform.position);


        if (targetDistance <= stateMachine.HuntDistance && (_player._inSafeZone || (!_player._isMad && _player._inShadow)))
        { 
            _speed = 0.0f;
            stateMachine.RotationSpeed = 0.0f;
        }
        else if(_player._isMad && !_player._inSafeZone)
        { 
            _speed = stateMachine.HuntSpeed;
        }
        else
        {
            _speed = stateMachine.HuntSpeed;
        }
    }
}