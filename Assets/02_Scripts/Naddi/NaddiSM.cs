using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NaddiStates
{
    Patrol = 0,
    Chase = 1,
    LookForPlayer = 2,
    Digging = 3
}
public class NaddiSM : MonoBehaviour
{
    private NaddiStates _state;
    [SerializeField]
    private NaddiAgent _naddi;
    private bool _canSwitchState = false;

    public void LookForPlayer()
    {
        _naddi.State = NaddiStates.LookForPlayer; 
    }

    public void FoundPlayer()
    {
        _naddi.State = NaddiStates.Chase; 
    }

    public void LostPlayer()
    {
        _naddi.State = NaddiStates.LookForPlayer;
    }

    public void FinishedLookForPlayer()
    {
        StartDigging();
    }
    public void StartDigging()
    {
        _naddi.State = NaddiStates.Digging; 
    }

    public void FinishedDigging()
    {
        _naddi.State = NaddiStates.Patrol;
    }
}
