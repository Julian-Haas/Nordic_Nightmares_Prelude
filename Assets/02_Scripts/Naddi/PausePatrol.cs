using UnityEngine;
using UnityEngine.Splines;

public class PausePatrol : MonoBehaviour
{
    [SerializeField]
    private BezierKnot _knotToPaurseAt;
    [SerializeField]
    private Naddagil _naddagil;
    [SerializeField]
    private float breakDuration = 2.0f; 

    private void OnTriggerEnter(Collider other)
    {
        _naddagil.StateMachiene.SetState(NaddiStates.Idle);
        _naddagil.transform.position = _knotToPaurseAt.Position; // add dig out animation right here 
    }

}
