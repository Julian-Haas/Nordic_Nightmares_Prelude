using UnityEngine;

public abstract class NaddiBaseState : State
{
    //states that Naddi can be in & act accordingly too 
    public enum NaddiStates
    {
        Base,
        Regular,
        Suspicious,
        Alert,
        Aware,
        Hunt,
        Pause,
        InspectPOI,
        InspectCA
    }

    public NaddiStates _state;
    protected readonly NaddiStateMachine stateMachine;
    protected GameObject _target;
    protected bool _targetVisibility = true;
    protected Vector3 _direction = new Vector3(0.1f, 0.0f, 0.1f), _tempTarget;
    protected float _speed = 1.0f;
    protected bool _returnToPath = false;

    //set stateMachine & state according to which state will be entered
    protected NaddiBaseState(NaddiStateMachine stateMachine, NaddiStates state)
    {
        this.stateMachine = stateMachine;
        _state = state;
    }

    //Naddi returned to path, path can restart & Naddi will continue follow the PathFollower
    protected void ContinueOnPath()
    {
        stateMachine.currentPath.ContinueOnPath();
    }

    //if Naddi arrived at target, stop moving
    //otherwise, Naddi continues moving towards it, depending on state related speed
    protected void FollowTarget(Vector3 targetPosition)
    {
        if(CheckDistanceToTarget(targetPosition)) { }
        else
        {
            _direction = (targetPosition - stateMachine.transform.position).normalized;
            stateMachine.transform.position += (_speed * Time.deltaTime * _direction);
            stateMachine.transform.rotation = Quaternion.Slerp(stateMachine.transform.rotation, Quaternion.LookRotation(_direction), Time.deltaTime * stateMachine.RotationSpeed);
        }
        stateMachine.SPEED = _speed/10.0f;
        stateMachine.ActiveState = _state.ToString();
        stateMachine.PlayMovingOrIdleAnimation(_state, _speed/10.0f);
    }

    //if Naddi is walking outside of path, check if target can be reached in straight line 
    //if target not reachable in clear line, get last created reachable trackpoint of target && walk towards that
    protected void TrackTarget()
    {
        if (!CheckLineOfSightToTarget(_target)) {
            _tempTarget = _target.GetComponentInChildren<NaddiTrackTarget>().ReturnReachableTrackpoint(stateMachine.transform.position);
            FollowTarget(_tempTarget);
        }
        else { FollowTarget(_target.transform.position); }
    }

    //return if distance to target is <= 0.1f (if Naddi arrived at target)
    protected bool CheckDistanceToTarget(Vector3 target)
    {
        target.y = stateMachine.transform.position.y;
        if(Vector3.Distance(target, stateMachine.transform.position) <= 0.1f)
        {
            //Debug.Log("Arrived at target!");
            return true;
        }
        else
        {
            return false;
        }
    }

    //return if Naddi has direct view of target, or if it is blocked / hidden
    protected bool CheckLineOfSightToTarget(GameObject target)
    {
        RaycastHit hitinfo;
        Vector3 tmpPos = new Vector3(stateMachine.transform.position.x, target.transform.position.y, stateMachine.transform.position.z);
        bool clearLineOfSight = !Physics.Linecast(tmpPos, target.transform.position, out hitinfo, -1);
        _targetVisibility = clearLineOfSight;
        return clearLineOfSight;      
    }

    //switches Naddis position & rotation according to new path
    protected void SwitchToNewLocation()
    {
        stateMachine.currentPath = stateMachine.newPath;
        _target = stateMachine.currentPath.StartPathWalking(_speed);
        stateMachine.transform.position = _target.transform.position;
        stateMachine.transform.rotation = _target.transform.rotation;
        stateMachine._switchToNewPath = false;
    }

    //depending on state, enables Naddi to inspect registered input
    protected void InspectWhatGotRegistered()
    {
        stateMachine.currentPath.StopPathWalking();
        stateMachine.SwitchState(new N_21_InspectCAState(stateMachine));
    }

    //Naddi returns to where path was left off last, movement depending on state & if direct way possible or backtracking needed
    protected void ReturnToPath(float speed)
    {
        NaddiBaseState checkState = stateMachine.GetCurrentStateByAwareness();
        if (checkState._state != NaddiStates.Hunt)
        {
            TrackTarget();
            if (Vector3.Magnitude(_target.transform.position - stateMachine.transform.position) <= 5.0f)
            {
                stateMachine.currentPath.StartPathWalking(speed);
            }
            stateMachine.SwitchState(checkState);
        }
        else if (checkState._state == NaddiStates.Hunt)
        {
            stateMachine.SwitchState(checkState);
        }
    }

    protected void PauseAtPauseLocation()
    {
        stateMachine.timeIdled += Time.deltaTime;
        NaddiBaseState checkState = stateMachine.GetCurrentStateByAwareness();
        if (checkState.GetType().Name == "N_00_RegularState" && stateMachine.timeIdled > stateMachine.PauseRegular)
        {
            stateMachine.SwitchState(checkState);
        }
        else if (checkState.GetType().Name == "N_01_SuspiciousState" && stateMachine.timeIdled > stateMachine.PauseSuspicious)
        {
            stateMachine.SwitchState(checkState);
        }
        else if (checkState.GetType().Name != "N_00_RegularState" && checkState.GetType().Name != "N_01_SuspiciousState")
        {
            stateMachine.SwitchState(checkState);
        }

        stateMachine.SPEED = _speed / 10.0f;
        stateMachine.ActiveState = _state.ToString();
        stateMachine.PlayMovingOrIdleAnimation(_state, _speed / 10.0f);
    }

    protected void PauseAtPOI() {
        stateMachine.timeIdled += Time.deltaTime;
        NaddiBaseState checkState = stateMachine.GetCurrentStateByAwareness();
        if (checkState._state == NaddiStates.Suspicious && stateMachine.timeIdled > stateMachine.InspectPOISuspicious)
        {
            stateMachine.SwitchState(checkState);
        }
        else if (checkState._state != NaddiStates.Suspicious && stateMachine.timeIdled > stateMachine.InspectPOIAlert)
        {
            stateMachine.SwitchState(checkState);
        }

        stateMachine.SPEED = _speed / 10.0f;
        stateMachine.ActiveState = _state.ToString();
        stateMachine.PlayMovingOrIdleAnimation(_state, _speed / 10.0f);
    }

    protected void InspectCA()
    {
        if (stateMachine.GetCurrentStateByAwareness()._state == NaddiStates.Hunt)
        {
            stateMachine.SwitchStateByAwareness(this);
        }
        if (stateMachine._registeredSomething) { InspectWhatGotRegistered(); }
        if (!_returnToPath) { FollowTarget(_target.transform.position); }
        else if (_returnToPath) { ReturnToPath(_speed); }

        stateMachine.SPEED = _speed / 10.0f;
        stateMachine.ActiveState = _state.ToString();
        stateMachine.PlayMovingOrIdleAnimation(_state, _speed / 10.0f);
    }
}