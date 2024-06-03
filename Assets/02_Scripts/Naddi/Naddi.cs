//The person responsible for this code is Nils Oskar Henningsen 
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.AI;

public class Naddi : MonoBehaviour
{
    [SerializeField]
    public NaddiViewField NaddiEye;
    [SerializeField]
    private PatrolPath _patrolPath;
    [SerializeField]
    private Transform PlayerPos;
    public NaddiStateMaschine StateMachiene { get; private set; }
    [SerializeField]
    private Terrain _terrain;
    public float Speed;
    [SerializeField]
    public NavMeshAgent Agent { get; private set; }
    private SplineAnimate _splineAnimate;
    public bool _executingState = false;
    private Vector3 _playerPosLastSeen;
    private bool _startedPatrol = false;
    public bool ChasePlayer = false;
    private s_PlayerCollider _playerCol;
    public bool PlayerInSafeZone; 
    public bool HeardPlayer = false;
    private NaddiHearing _naddiHearing;
    public bool KilledPlayer = false;
    public bool CanChasePlayer { get; private set; }
    public NaddiStateEnum State;
    private NaddiAttack _attackBehaviour;
    private bool RendererEnabled = true;

    private void Awake()
    {
        InitSplineAnimate();
        StateMachiene = GetComponent<NaddiStateMaschine>(); 
        Agent = this.GetComponent<NavMeshAgent>();
        NaddiEye = GetComponent<NaddiViewField>();
        _attackBehaviour = GetComponent<NaddiAttack>();
        Agent.speed = Speed;
        _naddiHearing = this.GetComponent<NaddiHearing>();
    }

    private void Start()
    {
        _naddiHearing.LookForPlayerAction += SusSoundHeard;
        _naddiHearing.AttackPlayerAction += HeardPlayerNearby;
        _playerCol = PlayerPos.gameObject.GetComponent<s_PlayerCollider>(); 
    }


    private void Update()
    {
        PlayerInSafeZone = _playerCol._inSafeZone; 
        if(KilledPlayer)
        {
            CanChasePlayer = false;
        }

        if(State != NaddiStateEnum.Digging && !RendererEnabled && !StateMachiene.GetNaddiMeshRenderer.enabled) 
        {
            RendererEnabled = true;
            StateMachiene.GetNaddiMeshRenderer.enabled = true;

        }
        if (PlayerInSafeZone && (State == NaddiStateEnum.Chase || State == NaddiStateEnum.Attack))
        {
            StateMachiene.PlayerVanished();
        }
        if (NaddiEye.isInsideCone() && State != NaddiStateEnum.Digging)
        {
            _playerPosLastSeen = PlayerPos.position;
            StateMachiene.FoundPlayer();
        }
        if (State != NaddiStateEnum.Patrol && _splineAnimate.enabled)
        {
            _splineAnimate.enabled = false; 
        }
        HandleState();
    }
    void InitSplineAnimate()
    {
        _splineAnimate = gameObject.AddComponent<SplineAnimate>();
        _splineAnimate.AnimationMethod = SplineAnimate.Method.Speed;
        _splineAnimate.enabled = false;
        _splineAnimate.PlayOnAwake = false;
        _splineAnimate.MaxSpeed = Speed;
    }
    public void DisableRenderer() 
    { 
        RendererEnabled = false;
        StateMachiene.GetNaddiMeshRenderer.enabled = false;
    }
    private void WalkOnPatrol()
    {
        if (_startedPatrol == false)
        {
            _startedPatrol = true;
            CanChasePlayer = true;
            _splineAnimate.Container = _patrolPath.GetActivePatrolPath();
            KilledPlayer = false; 
            if (_splineAnimate.ElapsedTime > 0) 
            {
                _splineAnimate.ElapsedTime = 0f; 
            }
            Vector3 newPos = _patrolPath.CalculateDistanceForEachKnot();
            transform.position = newPos;
            StateMachiene.GetNaddiMeshRenderer.enabled = true;
            _splineAnimate.enabled = true;
        }
        _splineAnimate.Play(); //needs to be called every frame cause unity is stupid and other wise Naddi wouldnt walk along spline
    }
    private void HandleState()
    {
        switch (State)
        {
            case NaddiStateEnum.Digging:
                ChasePlayer=false;
                Agent.isStopped = true;
                CanChasePlayer = false;
                _startedPatrol = false;
                break;
            case NaddiStateEnum.Patrol:
                WalkOnPatrol();
                break;
            case NaddiStateEnum.Chase:
                Agent.isStopped = false;
                _startedPatrol = false;
                ChasePlayer = true;
                _attackBehaviour.ChasePlayer(PlayerPos);
                break;
            case NaddiStateEnum.LookForPlayer:
                ChasePlayer = false; 
                _attackBehaviour.WalkToLastPlayerPosition(_playerPosLastSeen);
                break;
            case NaddiStateEnum.Attack:
                ChasePlayer = true;
                Agent.isStopped = true;
                break;
            case NaddiStateEnum.PlayerVanished: 
                Agent.isStopped = true;
                StateMachiene.LookForPlayer(); 
                break; 
        }
    }
    public void SusSoundHeard(Vector3 pos)
    {
        if (State != NaddiStateEnum.Chase && State != NaddiStateEnum.Attack && State != NaddiStateEnum.Digging && !HeardPlayer)
        {
            HeardPlayer = true;
            DeactivatePatrol();
            this.transform.LookAt(pos);
            StateMachiene.LookForPlayer();
        }
    }
    private void DeactivatePatrol()
    {
        _splineAnimate.Pause();
        _splineAnimate.enabled = false;
        _splineAnimate.ElapsedTime = 0;
        _startedPatrol = false;
    }
    public void HeardPlayerNearby()
    {
        StateMachiene.FoundPlayer(); 
    }
    public IEnumerator HearingDelay()
    {
        HeardPlayer = true;
        yield return new WaitForSeconds(10f);
        HeardPlayer = false;
    }
    public void ResetNaddiPosition() 
    {
        _startedPatrol = false;
        Agent.ResetPath();
        _naddiHearing.ResetSoundSum();
        Agent.isStopped = true;
        StateMachiene.FinishedDigging(); 
        StartCoroutine(_naddiHearing.ListenerDelay()); 
    }
}