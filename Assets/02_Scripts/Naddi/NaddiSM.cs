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
    private NaddiStates _State;


    public void SetState(NaddiStates state)
    {

    }
}

