using UnityEngine;

public enum NaddiStates
{
    Patrol,
    Chase,
    LookForPlayer,
    Digging
}
public class NaddiSM : MonoBehaviour
{
    [SerializeField]
    private NaddiAgent _naddi;

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
