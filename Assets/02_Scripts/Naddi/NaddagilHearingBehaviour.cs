using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaddagilHearingBehaviour : MonoBehaviour
{
    [SerializeField]
    private Naddagil _naddagil;

    public bool HeardPlayer = false;
    private void Start()
    {
        _naddagil.NaddiHearing.LookForPlayerAction += SusSoundHeard;
        _naddagil.NaddiHearing.AttackPlayerAction += HeardPlayerNearby;
    }
    public void SusSoundHeard(Vector3 pos)
    {
        if (_naddagil.State != NaddiStates.Chase && _naddagil.State != NaddiStates.Attack && _naddagil.State != NaddiStates.Digging && !HeardPlayer)
        {
            HeardPlayer = true;
            _naddagil.StateMachiene.HearedSomething();
            _naddagil.PatrolBehaviour.DeactivatePatrol();
            StartCoroutine(TurnToSoundDirection(pos));
        }
    }

    public void HeardPlayerNearby()
    {
        _naddagil.StateMachiene.FoundPlayer();
    }
    public IEnumerator HearingDelay()
    {
        HeardPlayer = true;
        yield return new WaitForSeconds(10f);
        HeardPlayer = false;
    }
    private IEnumerator TurnToSoundDirection(Vector3 soundPos)
    {
        Debug.Log("executing turn to player!");
        Quaternion desiredRotation;
        Vector3 direction = (soundPos - transform.position).normalized;
        desiredRotation = Quaternion.LookRotation(direction, transform.up);
        float lerpFactor = 1 * Time.deltaTime;
        float time = 0f;
        while (time <= 2)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, time * lerpFactor);
            time += Time.deltaTime;
            yield return null;
        }
        _naddagil.StateMachiene.LookForPlayer();
    }
}
