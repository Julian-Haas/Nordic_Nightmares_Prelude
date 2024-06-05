//The person responsible for this code is Nils Oskar Henningsen 
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.AI;
using TMPro; 

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
    public bool CanChasePlayer = true; 
    public NaddiStateEnum State;
    private NaddiAttack _attackBehaviour;
    private bool RendererEnabled = true;
    private bool StopAgent = false;
    bool PlayerWasInSafeZone;
    [Header("Debug stuff")]
    [SerializeField, Tooltip("Set this to true, if you wanna have Debug informations. If true, the Debug textes have to be assgingned below!")]
    public bool enableDebugInfos = false;
    //this is just for Debugging:
    [SerializeField]
    private GameObject DebugTextHolder; 
    [SerializeField]
    private TextMeshProUGUI targetText;
    [SerializeField]
    private TextMeshProUGUI RemainingDistanceTXT;
    [SerializeField]
    private TextMeshProUGUI pathstatusText;
    [SerializeField]
    private NaddiValueStorage valueStorage; 

    void Awake()
    {
        Speed = valueStorage.NaddiSpeed; 
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
        if (enableDebugInfos == false && DebugTextHolder!=null)
        {
            DebugTextHolder.SetActive(false); 
        }
    }

    private void Update()
    {

        if (enableDebugInfos)
        {
            if (_splineAnimate != null)
            {
                _splineAnimate.MaxSpeed = Speed; 
            }
        }
        PlayerInSafeZone = _playerCol._inSafeZone;
        if (KilledPlayer)
        {
            CanChasePlayer = false;
        }
        if (State != NaddiStateEnum.Digging && !RendererEnabled && !StateMachiene.GetNaddiMeshRenderer.enabled)
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
            SetFlags(ref _startedPatrol, ref CanChasePlayer, true, true);
            _splineAnimate.Container = _patrolPath.GetActivePatrolPath();
            KilledPlayer = false;
            if (_splineAnimate.ElapsedTime > 0)
            {
                _splineAnimate.ElapsedTime = 0f;
            }
            Vector3 newPos = _patrolPath.CalculateDistanceForEachKnot();
            if (PlayerWasInSafeZone && !PlayerInSafeZone)
            {
                SetFlags(ref _startedPatrol, ref CanChasePlayer, false, false); 
                StateMachiene.FoundPlayer();
                return;
            }
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
                SetFlags(ref ChasePlayer, ref StopAgent, ref _startedPatrol, false, true, false);
                Agent.isStopped = StopAgent;
                CanChasePlayer = false;
                break;
            case NaddiStateEnum.Patrol:
                ChasePlayer = false;
                WalkOnPatrol();
                break;
            case NaddiStateEnum.Chase:
                SetFlags(ref StopAgent, ref _startedPatrol, ref ChasePlayer, false, false, true);
#if UNITY_EDITOR
                if (enableDebugInfos)
                {
                    EditorHelper.SetDebugText<string>(ref targetText, "player");
                    EditorHelper.SetDebugText<float>(ref RemainingDistanceTXT, Vector3.Distance(this.transform.position, PlayerPos.position));
                    EditorHelper.SetDebugText<NavMeshPathStatus>(ref pathstatusText, Agent.pathStatus);
                }
#endif
                Agent.isStopped = StopAgent;
                _attackBehaviour.ChasePlayer(PlayerPos);
                break;
            case NaddiStateEnum.LookForPlayer:
                SetFlags(ref ChasePlayer, ref _startedPatrol, false, false);
#if UNITY_EDITOR
                if (enableDebugInfos)
                {
                    EditorHelper.SetDebugText<string>(ref targetText, "Player pos last seen");
                    EditorHelper.SetDebugText<float>(ref RemainingDistanceTXT, Vector3.Distance(this.transform.position, _playerPosLastSeen));
                    EditorHelper.SetDebugText<NavMeshPathStatus>(ref pathstatusText, Agent.pathStatus);
                }
#endif
                _attackBehaviour.WalkToLastPlayerPosition(_playerPosLastSeen);
                break;
            case NaddiStateEnum.Attack:
                SetFlags(ref _startedPatrol, ref ChasePlayer, ref StopAgent, false, true, true);
                Agent.isStopped = StopAgent;
                break;
            case NaddiStateEnum.PlayerVanished:
                SetFlags(ref StopAgent, ref _startedPatrol, true, false);
                Agent.isStopped = StopAgent;
                StateMachiene.LookForPlayer();
                break;
        }
    }

    void SetFlags(ref bool flagOne, ref bool flagTwo, bool val1, bool val2)
    {
        flagOne = val1;
        flagTwo = val2;
    }

    void SetFlags(ref bool flagOne, ref bool flagTwo, ref bool flagThree, bool val1, bool val2, bool val3)
    {
        flagOne = val1;
        flagTwo = val2;
        flagThree = val3;
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