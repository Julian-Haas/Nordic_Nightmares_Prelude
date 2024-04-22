using UnityEngine;

public enum NaddiStateEnum
{
    Patrol,
    Chase,
    LookForPlayer,
    Digging
}
public class NaddiStateMaschine : MonoBehaviour
{
    [SerializeField]
    private Naddi _naddi;

    public void LookForPlayer()
    {
        _naddi.State = NaddiStateEnum.LookForPlayer; 
    }

    public void FoundPlayer()
    {
        _naddi.State = NaddiStateEnum.Chase; 
    }

    public void LostPlayer()
    {
        _naddi.State = NaddiStateEnum.LookForPlayer;
    }

    public void FinishedLookForPlayer()
    {
        StartDigging();
    }
    public void StartDigging()
    {
        _naddi.State = NaddiStateEnum.Digging; 
    }

    public void FinishedDigging()
    {
        _naddi.State = NaddiStateEnum.Patrol;
    }
}
