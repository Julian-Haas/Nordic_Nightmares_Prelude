//The person responsible for this code is Nils Oskar Henningsen 
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.AI;
using TMPro; 

public class Naddagil : MonoBehaviour
{
    [Header("Public Naddi Components"), SerializeField]
    public NaddagilPatrolBehaviour PatrolBehaviour; 
    public NaddagilHearingBehaviour HearingBehaviour { get; private set; }
    public NaddagilAttackBehaviour AttackBehaviour { get; private set; }
    public NaddagilViewingSensor NaddiEye { get; private set; }
    public NaddagilHearingSensor NaddiHearing { get; private set;  }
    public SkinnedMeshRenderer MeshRenderer { get; private set; }

    [Header("State handling")]
    public NaddiStates State;
    public NaddagilStateMaschine StateMachiene { get; private set; }

    [Header("Naddi stats:")]
    public NaddiValueStorage ValueStorage;
    [HideInInspector]
    public float Speed;

    [Header("Public Flags")]
    public bool RendererEnabled = true;

    [Header("Public References")]
    public PatrolPath PatrolPath;
    public s_PlayerCollider PlayerCol { get; private set; }

    [SerializeField]
    private Terrain _terrain;

    [Header("Public Debug Flag")]
    public bool EnableDebugInfos = false;

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
        AttackBehaviour.Agent.speed = Speed;
    }

    private void Start()
    {
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
                AttackBehaviour.Agent.speed = Speed; 
        }
#endif 
        HandleState();
    }
    private void HandleState()
    {
        switch (State)
        {
            case NaddiStates.Digging:
                NaddagilUtillitys.SetFlags(ref AttackBehaviour.ChasePlayer, ref AttackBehaviour.StopAgent, ref PatrolBehaviour.StartedPatrol, false, true, false);
                AttackBehaviour.Agent.isStopped = AttackBehaviour.StopAgent;
                AttackBehaviour.CanChasePlayer = false;
                break;
            case NaddiStates.Patrol:
                AttackBehaviour.CanChasePlayer = true;
                AttackBehaviour.ChasePlayer = false;
                PatrolBehaviour.WalkOnPatrol();
                break;
            case NaddiStates.Chase:
                NaddagilUtillitys.SetFlags(ref AttackBehaviour.StopAgent, ref PatrolBehaviour.StartedPatrol, ref AttackBehaviour.ChasePlayer, false, false, true);
                AttackBehaviour.Agent.isStopped = AttackBehaviour.StopAgent;
                AttackBehaviour.Chase();
                break;
            case NaddiStates.LookForPlayer:
                NaddagilUtillitys.SetFlags(ref AttackBehaviour.ChasePlayer, ref PatrolBehaviour.StartedPatrol, false, false);
                AttackBehaviour.WalkToLastPlayerPosition();
                break;
            case NaddiStates.Attack:
                NaddagilUtillitys.SetFlags(ref PatrolBehaviour.StartedPatrol, ref AttackBehaviour.ChasePlayer, ref AttackBehaviour.StopAgent, false, true, true);
                AttackBehaviour.Agent.isStopped = AttackBehaviour.StopAgent;
                break;
            case NaddiStates.PlayerVanished:
                NaddagilUtillitys.SetFlags(ref AttackBehaviour.StopAgent, ref PatrolBehaviour.StartedPatrol, true, false);
                AttackBehaviour.Agent.isStopped = AttackBehaviour.StopAgent;
                StateMachiene.LookForPlayer();
                break;
            default:
                break; 
        }
    }
}