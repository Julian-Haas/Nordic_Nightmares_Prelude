using UnityEngine;
using System.Collections;

public enum NaddiStateEnum
{
    Patrol=0,
    Chase=1,
    LookForPlayer=2,
    Digging=3,
    Idle = 4
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
        StartCoroutine(ResetState(NaddiStateEnum.LookForPlayer));
    }

    public void FoundPlayer()
    {
        StartCoroutine(ResetState(NaddiStateEnum.Chase));
    }

    public void LostPlayer()
    {
        StartCoroutine(ResetState(NaddiStateEnum.LookForPlayer));
    }

    public void FinishedLookForPlayer()
    {
        StartDigging(); 
    }
    public void StartDigging()
    {
        StartCoroutine(ResetState(NaddiStateEnum.Digging));
    }

    public void FinishedDigging()
    {
        StartCoroutine(ResetState(NaddiStateEnum.Patrol)); 
    }

    private void SetState(NaddiStateEnum state)
    {
        _currentState = state;
        _naddi.State = _currentState; 
    }

    private IEnumerator ResetState(NaddiStateEnum nextState)
    {
        SetState(NaddiStateEnum.Idle);
        yield return new WaitForSeconds(0.1f);
        SetState(nextState);
    }

}