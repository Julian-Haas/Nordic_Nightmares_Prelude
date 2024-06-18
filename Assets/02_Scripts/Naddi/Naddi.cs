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
    public Transform PlayerPos;
    public NaddiStateMaschine StateMachiene { get; private set; }
    [SerializeField]
    private Terrain _terrain;
    public float Speed;
    [SerializeField]
    public NavMeshAgent Agent { get; private set; }
    public bool _executingState = false; 
    public Vector3 _playerPosLastSeen = new Vector3(-999999, -9999999, -999999);
    public bool StartedPatrol = false;
    public bool ChasePlayer = false;
    private s_PlayerCollider _playerCol;
    public bool PlayerInSafeZone;
    public bool HeardPlayer = false;
    public NaddiHearing NaddiHearing;
    public bool KilledPlayer = false;
    public bool CanChasePlayer = true; 
    public NaddiStateEnum State;
    private NaddiAttack _attackBehaviour;
    private bool RendererEnabled = true;
    private bool StopAgent = false;
    public bool PlayerWasInSafeZone;
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
    public NaddiPatrolBehaviour PatrolBehaviour;
    public NaddiHearingBehaviour HearingBehaviour; 
    void Awake()
    {
        DebugFileLogger.Initialize(); 
        Speed = valueStorage.NaddiSpeed; 
        StateMachiene = GetComponent<NaddiStateMaschine>();
        Agent = this.GetComponent<NavMeshAgent>();
        NaddiEye = GetComponent<NaddiViewField>();
        _attackBehaviour = GetComponent<NaddiAttack>();
        Agent.speed = Speed;
        NaddiHearing = this.GetComponent<NaddiHearing>();
    }

    private void Start()
    {
        _playerCol = PlayerPos.gameObject.GetComponent<s_PlayerCollider>();
        if (enableDebugInfos == false && DebugTextHolder!=null)
        {
            DebugTextHolder.SetActive(false); 
        }
    }

    private void Update()
    {
#if UNITY_EDITOR 
        if (enableDebugInfos)
        {
            if (PatrolBehaviour.SplineAnimate != null)
            {
                PatrolBehaviour.SplineAnimate.MaxSpeed = Speed;
            }
                Agent.speed = Speed; 
        }
#endif 
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
        if (State != NaddiStateEnum.Patrol && PatrolBehaviour.SplineAnimate.enabled)
        {
            PatrolBehaviour.SplineAnimate.enabled = false;
        }
        HandleState();
    }
    public void DisableRenderer()
    {
        RendererEnabled = false;
        StateMachiene.GetNaddiMeshRenderer.enabled = false;
    }

    private void HandleState()
    {
        switch (State)
        {
            case NaddiStateEnum.Digging:
                NaddiUtillitys.SetFlags(ref ChasePlayer, ref StopAgent, ref StartedPatrol, false, true, false);
                Agent.isStopped = StopAgent;
                CanChasePlayer = false;
                break;
            case NaddiStateEnum.Patrol:
                ChasePlayer = false;
                PatrolBehaviour.WalkOnPatrol();
                break;
            case NaddiStateEnum.Chase:
                NaddiUtillitys.SetFlags(ref StopAgent, ref StartedPatrol, ref ChasePlayer, false, false, true);
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
                NaddiUtillitys.SetFlags(ref ChasePlayer, ref StartedPatrol, false, false);
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
                NaddiUtillitys.SetFlags(ref StartedPatrol, ref ChasePlayer, ref StopAgent, false, true, true);
                Agent.isStopped = StopAgent;
                break;
            case NaddiStateEnum.PlayerVanished:
                NaddiUtillitys.SetFlags(ref StopAgent, ref StartedPatrol, true, false);
                Agent.isStopped = StopAgent;
                StateMachiene.LookForPlayer();
                break;
            default:
                break; 
        }
    }

    public void ResetNaddiPosition()
    {
        StartedPatrol = false;
        NaddiHearing.ResetSoundSum();
        Agent.isStopped = true;
        string msg = "current spline is: " + PatrolBehaviour.SplineAnimate.Container.gameObject.name;
        DebugFileLogger.Log("ResetNaddi", msg);
        StateMachiene.FinishedDigging();
        StartCoroutine(NaddiHearing.ListenerDelay());
    }
}