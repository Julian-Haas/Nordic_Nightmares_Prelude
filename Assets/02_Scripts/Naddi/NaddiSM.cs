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


    public void SelectNewState()
    {
        _state = _naddi.State;
        switch (_state)
        {
            case NaddiStates.Patrol: 
            {
                if (_naddi.FoundPlayer)
                {
                    _naddi.State = NaddiStates.Chase; 
                }
                break; 
            }
            case NaddiStates.Chase:
            {
                if (_naddi.FoundPlayer == false)
                {
                    _naddi.State = NaddiStates.LookForPlayer;
                }
                break; 
            }
            case NaddiStates.Digging:
            {
                if (_naddi.FoundPlayer == false)
                {
                    _naddi.State = NaddiStates.Patrol;
                }
                else
                {
                    _naddi.State = NaddiStates.Chase; 
                }
                break;
            }
            case NaddiStates.LookForPlayer:
            {
                if (_naddi.FoundPlayer == false)
                {
                    _naddi.State = NaddiStates.Digging;
                }
                else
                {
                    _naddi.State = NaddiStates.Chase;
                }
                break;
            }
        }
    }

}

