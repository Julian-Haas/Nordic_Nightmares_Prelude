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
    [SerializeField, Tooltip("The timespan the Naddi needs to dig to a new Patrol Spline in Seconds.")]
    private float _digDuration = 5f;
    [SerializeField]
    private float _speed;
    public float Speed { get {return _speed; }  }
    [SerializeField]
    private NavMeshAgent _agent;
    private SplineAnimate _splineAnimate;
    private NaddiStateEnum _state = NaddiStateEnum.Digging;
    private bool _executingState = false;
    private bool _foundPlayer;
    private Vector3 _playerPosLastSeen;
    private bool _startedPatrol = false;

    private Vector3 PatrolPoint;

    public NaddiStateEnum State
    {
        get{ return _state; }
        set { _state = value; }
    }

    public bool FoundPlayer
    {
        get { return _foundPlayer; }
    }

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
        _foundPlayer = _naddiEye.isInsideCone();
        if (_foundPlayer)
        {
            _playerPosLastSeen = _playerPos.position;
            //StopAllCoroutines(); 
            _naddiStateMachiene.FoundPlayer();
        }
       HandleState();
    }

    private void WalkOnPatrol()
    {
        _splineAnimate.enabled = true;
        if (_startedPatrol == false)
        {
            _startedPatrol = true;
            _splineAnimate.ElapsedTime = 0;
        }
        _splineAnimate.Play();
        if (_foundPlayer)
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
                if (_executingState == false) //-> die courintine startet mehrmals ohne die if abfrage todo: bessere lösung finden
                {
                    StartCoroutine(Digging());
                    _startedPatrol = false;
                }
                break;
            case NaddiStateEnum.Patrol:
                WalkOnPatrol();
                break;
            case NaddiStateEnum.Chase:
                _startedPatrol = false;
                ChasePlayer();
                break;
            case NaddiStateEnum.LookForPlayer:
                _startedPatrol = false;
                if (_executingState == false)
                {
                    WalkToLastPlayerPosition();
                }
                break;
        }
    }

    private IEnumerator Digging()
    {
        _executingState = true;
        yield return new WaitForSeconds(_digDuration);
        _splineAnimate.Container = _patrolPath.ActivatePatrolPath(); 
        Vector3 newPos = _patrolPath.GetFarthesPoint();
        transform.position = newPos;
        _naddiStateMachiene.FinishedDigging();
        _executingState = false;
    }

    private void ChasePlayer()
    {
        if (_foundPlayer)
        {
            _agent.isStopped = false; 
            _splineAnimate.enabled = false;
            _executingState = true;
            _agent.SetDestination(_playerPos.position);
        }
        else
        {
            _executingState = false;
            _naddiStateMachiene.LostPlayer();
        }

    }

    private IEnumerator LookForPlayer()
    {
        _executingState = true;
        float time = 0;
        while (time <= 3)
        {
            if (_naddiEye.isInsideCone())
            {
                _naddiStateMachiene.FoundPlayer();
                _executingState = false;
                yield break; 
            }
            time += Time.deltaTime;
            yield return null;  
        }
        _executingState = false;

    }
    private void WalkToLastPlayerPosition()
    {
        _agent.SetDestination(_playerPosLastSeen);
        if (Vector3.Distance(_playerPosLastSeen, transform.position) <= 20)
        {
            _agent.isStopped = true;
            _naddiStateMachiene.FinishedLookForPlayer();
            StartCoroutine(LookForPlayer());
        }
    }  
}