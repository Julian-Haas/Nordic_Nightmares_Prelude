//The person responsible for this code is Nils Oskar Henningsen 
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.AI;

public class Naddi : MonoBehaviour
{
    [SerializeField]
    private NaddiViewField _naddiEye;
    [SerializeField]
    private PatrolPath _patrolPath;
    [SerializeField]
    private Transform _playerPos;
    [SerializeField]
    private NaddiStateMaschine _naddiStateMachiene;
    public NaddiStateMaschine StateMachine { get { return _naddiStateMachiene; } } 
    [SerializeField, Tooltip("The timespan the Naddi needs to dig to a new Patrol Spline in Seconds.")]
    private float _digDuration = 5f;
    [SerializeField]
    private float _speed;
    public float Speed { get {return _speed; }  }
    [SerializeField]
    private NavMeshAgent _agent;
    private SplineAnimate _splineAnimate;
    public NaddiStateEnum _state = NaddiStateEnum.Digging;
    public bool _executingState = false;
    private Vector3 _playerPosLastSeen;
    private bool _startedPatrol = false;
    private bool _attackedPlayer = false;
    private bool _chasesPlayer = false;
    public bool ChasesPlayer { get { return _chasesPlayer;  } }
    private Vector3 targetPosition;
    private float _time;
    bool lookForPlayer; 
    public NavMeshAgent Agent { get { return _agent;  } }

    public NaddiStateEnum State
    {
        get{ return _state; }
        set { _state = value; }
    }
    Vector3 oldPos; 

  //public bool FoundPlayer
  //{
  //    get { return _foundPlayer; }
  //}

    private void Awake()
    {
        InitSplineAnimate();
        _agent.speed = _speed; 
    }

    void InitSplineAnimate()
    {
        _splineAnimate = gameObject.AddComponent<SplineAnimate>();
        _splineAnimate.AnimationMethod = SplineAnimate.Method.Speed;
        _splineAnimate.enabled = false;
        _splineAnimate.PlayOnAwake = false;
        _splineAnimate.MaxSpeed = _speed; 
    }
    private void Update()
    {
        if (lookForPlayer || _chasesPlayer)
        {
            Debug.DrawLine(transform.position, targetPosition, color: Color.white); 
        }
        if (_naddiEye.isInsideCone() && _state != NaddiStateEnum.Digging)
        {
            _playerPosLastSeen = _playerPos.position;
            _naddiStateMachiene.FoundPlayer();
        }

        if (_state != NaddiStateEnum.Patrol)
        {
            _splineAnimate.enabled = false; 
        }
       HandleState();
    }
    private void WalkOnPatrol()
    {
        _splineAnimate.Container = _patrolPath.GetActivePatrolPath();
        if (_startedPatrol == false)
        {
            transform.position = _patrolPath.GetFarthesPoint();
            _startedPatrol = true;
            _splineAnimate.ElapsedTime = 0;
        }
        _splineAnimate.enabled = true;
        _splineAnimate.Play();
        if (_naddiEye.isInsideCone())
        {
            _splineAnimate.Pause();
            _splineAnimate.enabled = false;
        }
    }

    private void HandleState()
    {
        switch (_state)
        {
            case NaddiStateEnum.Digging:
                _agent.isStopped = true;
                Digging();
                _startedPatrol = false;
                break;
            case NaddiStateEnum.Patrol:
                _agent.isStopped = true;
                WalkOnPatrol();
                break;
            case NaddiStateEnum.Chase:
                _agent.isStopped = false;
                _startedPatrol = false;
                ChasePlayer();
                break;
            case NaddiStateEnum.LookForPlayer:
                _startedPatrol = false;
                WalkToLastPlayerPosition();
                break;
            case NaddiStateEnum.Attack:
                _startedPatrol = false;
                _agent.isStopped = true;
                break;

        }
    }

    private void Digging()
    {
        _attackedPlayer = false;
        _executingState = true;
    }

    private void ChasePlayer()
    {
        float sqrMagnitude = (_playerPos.position - this.transform.position).sqrMagnitude;
        if (_naddiEye.isInsideCone() && sqrMagnitude <= Mathf.Pow(_agent.stoppingDistance, 2))
        {
            _chasesPlayer = true;
            _agent.isStopped=true; 
          _naddiStateMachiene.AttackPlayer();
        }
        else if (_naddiEye.isInsideCone() && sqrMagnitude > Mathf.Pow(_agent.stoppingDistance, 2) || sqrMagnitude < Mathf.Pow(_agent.stoppingDistance*5, 2) && sqrMagnitude > Mathf.Pow(_agent.stoppingDistance, 2))
        {
            _chasesPlayer = true;
            _agent.isStopped = false; 
            _splineAnimate.enabled = false;
            _executingState = true;
            targetPosition = _playerPos.position; 
            _agent.SetDestination(_playerPos.position);
        }
        else if(!_naddiEye.isInsideCone() && sqrMagnitude > Mathf.Pow(_agent.stoppingDistance * 5, 2))
        {
            _executingState = false;
            _chasesPlayer = false;
            _naddiStateMachiene.LostPlayer();
        }

    }

    private void WalkToLastPlayerPosition()
    {
        lookForPlayer = true;
        targetPosition = _playerPosLastSeen; 
        _agent.SetDestination(_playerPosLastSeen);
        if (_agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            _naddiStateMachiene.LookForPlayer();
            _agent.isStopped = true;
            _chasesPlayer = false; 
        }
    }
}