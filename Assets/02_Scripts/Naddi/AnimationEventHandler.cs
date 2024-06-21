using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public NaddagilViewingSensor _viewingSensor;
    public NaddagilStateMaschine _state;

    public void FinishedDigging()
    {
        _state.FinishedDigging(); 
    }

    public void FinishedAttacking()
    {
        _state.FinishedAttacking(_viewingSensor.isInsideCone()); 
    }

    void FinishedLookForPlayer()
    {
        _state.FinishedLookForPlayer(); 
    }
}
