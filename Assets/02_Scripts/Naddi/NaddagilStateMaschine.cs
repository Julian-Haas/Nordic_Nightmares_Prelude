//The person responsible for this code is Nils Oskar Henningsen 
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.LowLevel;
using TMPro; 

public enum NaddiStates 
{
    Patrol=0,
    Chase=1,
    LookForPlayer=2,
    Digging=3,
    Idle = 4,
    Attack = 5,
    DigToPlayer = 6, 
    PlayerVanished = 7,
    HearedSomething = 8
}
public class NaddagilStateMaschine : MonoBehaviour
{
    [SerializeField]
    private Naddagil _naddagil;
    public NaddiStates CurrentState { get; private set; } = NaddiStates.Digging;

    [Header("Debug stuff")]
    [SerializeField]
    private TextMeshProUGUI StateTXT;

    private void Start()
    {
        if (_naddagil.EnableDebugInfos == false && StateTXT != null)
            StateTXT.gameObject.SetActive(false); 
    }
    public void LookForPlayer()
    {
        if(_naddagil.State == NaddiStates.LookForPlayer) 
        {
            return; 
        }
        SetState(NaddiStates.LookForPlayer);
    }

    public void FoundPlayer()
    {
        SetState(NaddiStates.Chase);
    }

    public void LostPlayer()
    {
        if(_naddagil.AttackBehaviour.PlayerPosLastSeen != _naddagil.AttackBehaviour.PlayerPos.position)
        {
            SetState(NaddiStates.LookForPlayer);
        }
    }

    public void FinishedLookForPlayer()
    {
        if ((_naddagil.HearingBehaviour.HeardPlayer || _naddagil.NaddiEye.isInsideCone()) && _naddagil.AttackBehaviour.PlayerInSafeZone == false)
        {
            _naddagil.HearingBehaviour.HeardPlayer = false; 
            FoundPlayer();
        }
        else
        {
            StartCoroutine(_naddagil.HearingBehaviour.HearingDelay());
            StartDigging(); 
        }
    }
    public void StartDigging()
    {
        SetState(NaddiStates.Digging);
    }

    public void FinishedDigging()
    {
        NaddagilUtillitys.DisableRenderer(ref _naddagil);
        DebugFileLogger.Log("FlagLogger", "Startet Patrol: " + _naddagil.PatrolBehaviour.StartedPatrol.ToString());
        _naddagil.PatrolBehaviour.StartedPatrol = false; 
        SetState(NaddiStates.Patrol);
        DebugFileLogger.Log("StateLogger", "Naddi state after FinishedDigging: " + _naddagil.State.ToString());
    }

    public void PlayerVanished() 
    {
        SetState(NaddiStates.PlayerVanished); 
    }
    public void SetState(NaddiStates state)
    {
        CurrentState = state;
        _naddagil.State = CurrentState;
        if (_naddagil.EnableDebugInfos)
        {
            EditorHelper.SetDebugText<NaddiStates>(ref StateTXT, CurrentState);  
        }
    }
    public void AttackPlayer()
    {
        SetState(NaddiStates.Attack);
    }

    public void DigToPlayer()
    {
        SetState(NaddiStates.DigToPlayer); 
    }
    public void HearedSomething()
    {
        SetState(NaddiStates.HearedSomething); 
    }
    public void FinishedAttacking(bool seesPlayer)
    {
        if (seesPlayer && _naddagil.AttackBehaviour.KilledPlayer == false)
        {
            FoundPlayer(); 
        }
        if(!seesPlayer && _naddagil.AttackBehaviour.KilledPlayer == false)
        {
            LookForPlayer(); 
        }
    }

}