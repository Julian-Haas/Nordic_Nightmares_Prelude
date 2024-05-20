//The person responsible for this code is Nils Oskar Henningsen 
using UnityEngine;
using System.Collections;

public enum NaddiStateEnum
{
    Patrol=0,
    Chase=1,
    LookForPlayer=2,
    Digging=3,
    Idle = 4,
    Attack = 5,
    DigToPlayer = 6
  
}
public class NaddiStateMaschine : MonoBehaviour
{
    [SerializeField]
    private Naddi _naddi;
    public Naddi Naddi { get { return _naddi;  } }
    private NaddiStateEnum _currentState = NaddiStateEnum.Digging; 
    public NaddiStateEnum CurrentState { get { return _currentState; } }

    public void LookForPlayer()
    {
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
        if ((_naddi.HeardPlayer ||  _naddi.NaddiEye.isInsideCone()) && _naddi.PlayerInSafeZone==false)
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
        SetState(NaddiStateEnum.Digging);
    }

    public void FinishedDigging()
    {
        SetState(NaddiStateEnum.Patrol);
    }

    private void SetState(NaddiStateEnum state)
    {
        _naddi._executingState = false;
        _currentState = state;
        _naddi.State = _currentState; 
    }
    public void AttackPlayer()
    {
        SetState(NaddiStateEnum.Attack);
    }

    public void FinishedAttacking(bool seesPlayer)
    {
        if (seesPlayer)
        {
            SetState(NaddiStateEnum.Chase);
        }
        else
        {
            SetState(NaddiStateEnum.LookForPlayer); 
        }
    }
}