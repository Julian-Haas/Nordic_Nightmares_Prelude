using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public NaddiViewField viewField;
    public NaddiStateMaschine _state;

    public void FinishedDigging()
    {
        _state.FinishedDigging(); 
    }

    public void FinishedAttacking()
    {
        _state.FinishedAttacking(viewField.isInsideCone()); 
    }

    void FinishedLookForPlayer()
    {
        _state.FinishedLookForPlayer(); 
    }
}
