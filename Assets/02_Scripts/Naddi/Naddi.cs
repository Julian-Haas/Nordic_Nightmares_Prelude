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
    public NaddiViewField NaddiEye { get { return _naddiEye; } }
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
    [SerializeField]
    private Terrain _terrain; 
    public float Speed { get { return _speed; } }
    [SerializeField]
    private NavMeshAgent _agent;
    private SplineAnimate _splineAnimate;
    public NaddiStateEnum _state = NaddiStateEnum.Digging;
    public bool _executingState = false;
    private Vector3 _playerPosLastSeen;
    private bool _startedPatrol = false;
    private bool _attackedPlayer = false;
    private bool _chasesPlayer = false;
    private s_PlayerCollider _playerCol;
    public bool PlayerInSafeZone; 
    public bool ChasesPlayer { get { return _chasesPlayer; } }
    private bool _heardPlayer = false;
    public bool HeardPlayer { get { return _heardPlayer; } set { _heardPlayer = true;  } }
    private Vector3 targetPosition;
    private float _time;
    private NaddiHearing _naddiHearing;
    public Transform PlayerPos { get { return _playerPos;  } }
    bool lookForPlayer;
    public NavMeshAgent Agent { get { return _agent; } }
    [SerializeField]
    private bool digToPlayer = false; 
    public NaddiStateEnum State
    {
        get { return _state; }
        set { _state = value; }
    }
    Vector3 oldPos;
    private Transform newRot;

    //public bool FoundPlayer
    //{
    //    get { return _foundPlayer; }
    //}

    private void Awake()
    {
        InitSplineAnimate();
        _agent.speed = _speed;
        _naddiHearing = this.GetComponent<NaddiHearing>();
    }

    private void Start()
    {
        _naddiHearing.LookForPlayerAction += SusSoundHeard;
        _naddiHearing.AttackPlayerAction += HeardPlayerNearby;
        _playerCol = PlayerPos.gameObject.GetComponent<s_PlayerCollider>(); 
    }

    void InitSplineAnimate()
    {
        _splineAnimate = gameObject.AddComponent<SplineAnimate>();
        _splineAnimate.AnimationMethod = SplineAnimate.Method.Speed;
        _splineAnimate.enabled = false;
        _splineAnimate.PlayOnAwake = false;
        _splineAnimate.MaxSpeed = _speed;
    }
    bool RendererEnabled = true;
    public void DisableRenderer() 
    { 
        RendererEnabled = false;
        _naddiStateMachiene.GetNaddiMeshRenderer.enabled = false;
    }
    public bool KilledPlayer = false;
    private bool canChasePlayer;

    private void Update()
    {
        if(KilledPlayer)
        {
            canChasePlayer = false;
        }

        if(_state != NaddiStateEnum.Digging && !RendererEnabled) 
        {
            RendererEnabled = true; 
            _naddiStateMachiene.GetNaddiMeshRenderer.enabled = true;

        }
        PlayerInSafeZone = _playerCol._inSafeZone; 
        if (PlayerInSafeZone && _state == NaddiStateEnum.Attack)
        {
            _naddiStateMachiene.PlayerVanished();
        }
        if (digToPlayer)
        {
            _state = NaddiStateEnum.DigToPlayer; 
            digToPlayer = false;
            DigToPlayer(); 
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
        if (_startedPatrol == false)
        {
            _startedPatrol = true;
            canChasePlayer = false;
            _splineAnimate.Container = _patrolPath.GetActivePatrolPath();
            KilledPlayer = false; 
            if (_splineAnimate.ElapsedTime > 0) 
            {
                _splineAnimate.ElapsedTime = 0f; 
            }
            Vector3 newPos = _patrolPath.CalculateDistanceForEachKnot();
            transform.position = newPos;
            _naddiStateMachiene.GetNaddiMeshRenderer.enabled = true;
        }
        if (_naddiStateMachiene.GetNaddiMeshRenderer.enabled == false)
        {
            print("Failed to enable SkinnedMeshRenderer...Retrying...");
            _startedPatrol = false;
            _naddiStateMachiene.GetNaddiMeshRenderer.enabled = true; 
        }
        _naddiStateMachiene.GetNaddiMeshRenderer.enabled = true;
        _splineAnimate.enabled = true;
        _splineAnimate.Play();
        if (_naddiEye.isInsideCone())
        {
            _splineAnimate.Pause();
            _splineAnimate.ElapsedTime = 0f; 
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
                _chasesPlayer = false; 
                _startedPatrol = false;
                WalkToLastPlayerPosition();
                break;
            case NaddiStateEnum.Attack:
                _startedPatrol = false;
                _agent.isStopped = true;
                break;
            case NaddiStateEnum.PlayerVanished: 
                _agent.isStopped = true;
                _startedPatrol = false;
                _naddiStateMachiene.LookForPlayer(); 
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
        if(canChasePlayer == false) 
        {
            if (_playerCol._inSafeZone)
            {
                _naddiStateMachiene.PlayerVanished();
                return;
            }
            float sqrMagnitude = (_playerPos.position - this.transform.position).sqrMagnitude;
            if (_naddiEye.isInsideCone() && sqrMagnitude <= Mathf.Pow(_agent.stoppingDistance, 2) && !PlayerInSafeZone)
            {
                _chasesPlayer = true;
                _agent.isStopped = true;
                _naddiStateMachiene.AttackPlayer();
            }
            else if (((_naddiEye.isInsideCone() && sqrMagnitude > Mathf.Pow(_agent.stoppingDistance, 2)) || (sqrMagnitude < Mathf.Pow(_agent.stoppingDistance * 5, 2) && sqrMagnitude > Mathf.Pow(_agent.stoppingDistance, 2))) && !PlayerInSafeZone)
            {
                _chasesPlayer = true;
                _agent.isStopped = false;
                _splineAnimate.enabled = false;
                _executingState = true;
                targetPosition = _playerPos.position;
                _agent.SetDestination(_playerPos.position);
            }
            else if (!_naddiEye.isInsideCone() && sqrMagnitude > Mathf.Pow(_agent.stoppingDistance * 5, 2) && !PlayerInSafeZone)
            {
                _executingState = false;
                _chasesPlayer = false;
                _naddiStateMachiene.LostPlayer();
            }
        }

    }

    private void WalkToLastPlayerPosition()
    {
        lookForPlayer = true;
        targetPosition = _playerPosLastSeen;
        _agent.SetDestination(_playerPosLastSeen);
        float sqrDistance = (_playerPosLastSeen - this.transform.position).sqrMagnitude; 
        if (sqrDistance <= Mathf.Pow(5, 2))
        {
            _naddiStateMachiene.LookForPlayer();
            _agent.isStopped = true;
            _chasesPlayer = false;
        }
    }

    public void SusSoundHeard(Vector3 pos)
    {
        if (_state != NaddiStateEnum.Chase && _state != NaddiStateEnum.Attack && _state != NaddiStateEnum.Digging && !_heardPlayer)
        {
            _heardPlayer = true;
            DeactivatePatrol();
            this.transform.LookAt(pos);
            _naddiStateMachiene.LookForPlayer();
        }
    }
    private void DeactivatePatrol()
    {
        _splineAnimate.Pause();
        _splineAnimate.enabled = false;
        _startedPatrol = false;
    }

    public void HeardPlayerNearby()
    {
        _naddiStateMachiene.FoundPlayer(); 
    }
    public IEnumerator HearingDelay()
    {
        _heardPlayer = true;
        yield return new WaitForSeconds(10f);
        _heardPlayer = false;

    }

    public void DigToPlayer()
    {
        bool _validPos=false;
        float offsetX;
        float offsetZ; 
        while (_validPos==false)
        {
            //random offset of new naddi position
            offsetX = Random.Range(5, 7);
            offsetZ = Random.Range(5, 7);
            float randomXVorzeichen = Random.Range(0, 1);
            float randomZVorzeichen = Random.Range(0, 1);
            if (randomXVorzeichen < 0) { offsetX *= -1; }
            if (randomZVorzeichen < 0) { offsetZ *= -1; }


            Vector3 digOutPos = new Vector3(_playerPos.position.x + offsetX, 0, _playerPos.position.z + offsetZ);
            float terrainhight = _terrain.SampleHeight(digOutPos);
            digOutPos.y = terrainhight;
            if (IsValidNavMesh(digOutPos))
            {
                this.transform.position = digOutPos;
                _validPos = true; 
            } 
        }
        _naddiStateMachiene.FoundPlayer(); 
    }
    bool IsValidNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas);
    }

    public void ResetNaddiPosition() 
    {
        _startedPatrol = false;
        _naddiHearing.ResetSoundSum();
        _agent.isStopped = true;
        StartCoroutine(_naddiHearing.ListenerDelay()); 
    }
}