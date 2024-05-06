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

    private Vector3 PatrolPoint;
    private float _time;

    public NavMeshAgent Agent { get { return _agent;  } }
    public NaddiStateEnum State
    {
        get{ return _state; }
        set { _state = value; }
    }

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
                if (_executingState == false) //-> die courintine startet mehrmals ohne die if abfrage todo: bessere lösung finden
                {
                    StartCoroutine(Digging());
                    _startedPatrol = false;
                }
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

    private IEnumerator Digging()
    {
        _attackedPlayer = false;
        _executingState = true;
        yield return new WaitForSeconds(_digDuration);
        //_naddiStateMachiene.FinishedDigging();
        Debug.Log("started patrol value: " + _startedPatrol + "if true, this is unexepected and not correct!"); 
    }

    private void ChasePlayer()
    {
        float sqrMagnitude = (_playerPos.position - this.transform.position).sqrMagnitude;
        if (sqrMagnitude <= Mathf.Pow(_agent.stoppingDistance, 2)) //3.5 is round about the distance from origin to mouth 
        {
            _agent.isStopped=true; 
          _naddiStateMachiene.AttackPlayer();
        }
        if (_naddiEye.isInsideCone() && sqrMagnitude > Mathf.Pow(_agent.stoppingDistance, 2))
        {
            _agent.isStopped = false; 
            _splineAnimate.enabled = false;
            _executingState = true;
            _agent.SetDestination(_playerPos.position);
        }
        else if(!_naddiEye.isInsideCone() && sqrMagnitude > Mathf.Pow(_agent.stoppingDistance*3, 2))
        {
            _executingState = false;
            _naddiStateMachiene.LostPlayer();
        }

    }

    private void LookForPlayer()
    {
        if (_naddiEye.isInsideCone())
        {
            _executingState = false;
            _naddiStateMachiene.FoundPlayer();
            return;
        }
    }
    private void WalkToLastPlayerPosition()
    {
        _agent.SetDestination(_playerPosLastSeen);
        if (Vector3.Distance(_playerPosLastSeen, transform.position) <= _agent.stoppingDistance)
        {
            _agent.isStopped = true;
            LookForPlayer();
        }
    }
}