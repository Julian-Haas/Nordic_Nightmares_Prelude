//The person responsible for this code is Nils Oskar Henningsen 
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.AI;
using TMPro; 

public class Naddi : MonoBehaviour
{
    [Header("Public Naddi Components")]
    public NaddiPatrolBehaviour PatrolBehaviour;
    public NaddiHearingBehaviour HearingBehaviour; 
    public NaddiAttack AttackBehaviour;
    public NaddiViewField NaddiEye;
    public NaddiHearing NaddiHearing;

    [Header("State handling")]
    public NaddiStateEnum State;
    public NaddiStateMaschine StateMachiene { get; private set; }


    [Header("Naddi stats:")]
    public NaddiValueStorage ValueStorage;
    [HideInInspector]
    public float Speed;

    [Header("AI-Behaviour")]
    public Transform PlayerPos;
    public Vector3 PlayerPosLastSeen = new Vector3(-999999, -9999999, -999999);
    public NavMeshAgent Agent { get; private set; }

    [Header("AI-Behaviour Flags")]
    private bool StopAgent = false;
    public bool CanChasePlayer = true; 
    public bool ChasePlayer = false;

    [Header("Public Attack Behaviour Flags")]
    public bool PlayerWasInSafeZone;
    public bool PlayerInSafeZone;
    public bool StartedPatrol = false;
    public bool HeardPlayer = false;
    public bool KilledPlayer = false;
    public bool RendererEnabled = true;

    [Header("Public Debug Flags")]
    [SerializeField, Tooltip("Set this to true, if you wanna have Debug informations. If true, the Debug textes have to be assgingned below!")]
    public bool EnableDebugInfos = false;

    [Header("Public References")]
    public PatrolPath PatrolPath;

    [Header("Private References")]
    [SerializeField]
    private s_PlayerCollider _playerCol;
    [SerializeField]
    private Terrain _terrain;


    [Header("Private Debug References")]
    [SerializeField]
    private GameObject DebugTextHolder; 
    [SerializeField]
    private TextMeshProUGUI targetText;
    [SerializeField]
    private TextMeshProUGUI RemainingDistanceTXT;
    [SerializeField]
    private TextMeshProUGUI pathstatusText;

    void Awake()
    {
        DebugFileLogger.Initialize(); 
        Speed = ValueStorage.NaddiSpeed; 
        Agent.speed = Speed;
    }

    private void Start()
    {
        _playerCol = PlayerPos.gameObject.GetComponent<s_PlayerCollider>();
        if (EnableDebugInfos == false && DebugTextHolder!=null)
        {
            DebugTextHolder.SetActive(false); 
        }
    }

    private void Update()
    {
#if UNITY_EDITOR 
        if (EnableDebugInfos)
        {
            if (PatrolBehaviour.SplineAnimate != null)
            {
                PatrolBehaviour.SplineAnimate.MaxSpeed = Speed;
            }
                Agent.speed = Speed; 
        }
#endif 
        PlayerInSafeZone = _playerCol._inSafeZone;
        HandleState();
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
                CanChasePlayer = true;
                ChasePlayer = false;
                PatrolBehaviour.WalkOnPatrol();
                break;
            case NaddiStateEnum.Chase:
                NaddiUtillitys.SetFlags(ref StopAgent, ref StartedPatrol, ref ChasePlayer, false, false, true);
                Agent.isStopped = StopAgent;
                AttackBehaviour.ChasePlayer(PlayerPos);
                break;
            case NaddiStateEnum.LookForPlayer:
                NaddiUtillitys.SetFlags(ref ChasePlayer, ref StartedPatrol, false, false);
                AttackBehaviour.WalkToLastPlayerPosition(PlayerPosLastSeen);
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
}