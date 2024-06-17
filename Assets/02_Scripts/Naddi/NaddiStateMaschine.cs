//The person responsible for this code is Nils Oskar Henningsen 
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.LowLevel;
using TMPro; 

public enum NaddiStateEnum
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
public class NaddiStateMaschine : MonoBehaviour
{
    [SerializeField]
    private Naddi _naddi;
    [SerializeField]
    private SkinnedMeshRenderer _naddiMeshRenderer;
    public SkinnedMeshRenderer GetNaddiMeshRenderer { get { return _naddiMeshRenderer; } }
    public Naddi Naddi { get { return _naddi;  } }
    private NaddiStateEnum _currentState = NaddiStateEnum.Digging; 
    public NaddiStateEnum CurrentState { get { return _currentState; } }
    bool alreadyTriggert = false;
    [SerializeField]
    private TextMeshProUGUI StateTXT;

    private void Start()
    {
        if (_naddi.enableDebugInfos == false && StateTXT != null)
            StateTXT.gameObject.SetActive(false); 
    }
    public void LookForPlayer()
    {
        if(_naddi.State == NaddiStateEnum.LookForPlayer) 
        {
            return; 
        }
        SetState(NaddiStateEnum.LookForPlayer);
    }

    public void FoundPlayer()
    {
        SetState(NaddiStateEnum.Chase);
    }

    public void LostPlayer()
    {
        SetState(NaddiStateEnum.LookForPlayer);
    }

    public void FinishedLookForPlayer()
    {
        if ((_naddi.HeardPlayer || _naddi.NaddiEye.isInsideCone()) && _naddi.PlayerInSafeZone==false)
        {
            _naddi.HeardPlayer = false; 
            FoundPlayer();
        }
        else
        {
            StartCoroutine(_naddi.HearingDelay());
            StartDigging(); 
        }
    }
    public void StartDigging()
    {
        alreadyTriggert = false; 
        SetState(NaddiStateEnum.Digging);
    }

    public void FinishedDigging()
    {
        _naddi.DisableRenderer();
        DebugFileLogger.Log("FlagLogger", "Startet Patrol: " + _naddi.StartedPatrol.ToString());
        _naddi.StartedPatrol = false; 
        SetState(NaddiStateEnum.Patrol);
    }

    public void PlayerVanished() 
    {
        SetState(NaddiStateEnum.PlayerVanished); 
    }
    public void SetState(NaddiStateEnum state)
    {
        //EditorHelper.ClearConsoleLogs();

        _naddi._executingState = false;

        _currentState = state;
        _naddi.State = _currentState;
        if (_naddi.enableDebugInfos)
        {
            EditorHelper.SetDebugText<NaddiStateEnum>(ref StateTXT, _currentState);  
        }
    }
    public void AttackPlayer()
    {
        SetState(NaddiStateEnum.Attack);
    }

    public void DigToPlayer()
    {
        SetState(NaddiStateEnum.DigToPlayer); 
    }
    public void HearedSomething()
    {
        SetState(NaddiStateEnum.HearedSomething); 
    }
    public void FinishedAttacking(bool seesPlayer)
    {
        if (seesPlayer && _naddi.KilledPlayer == false)
        {
            FoundPlayer(); 
        }
        if(!seesPlayer && _naddi.KilledPlayer == false)
        {
            LookForPlayer(); 
        }
    }

}