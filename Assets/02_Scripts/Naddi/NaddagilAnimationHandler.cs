//The person responsible for this code is Nils Oskar Henningsen 
using UnityEngine;

public class NaddagilAnimationHandler : MonoBehaviour
{
    [SerializeField]
    private Animator _anim;
    [SerializeField]
    private Naddagil _naddi; 

    private void Update()
    {
        AnimStateHandle();
    }

    void AnimStateHandle()
    {
        switch (_naddi.StateMachiene.CurrentState)
        {
            case NaddiStates.Patrol:
                SetAnimationState(true, false, false, _naddi.Speed); 
                break;
            case NaddiStates.Chase:
                SetAnimationState(true, false, false, _naddi.Speed);
                break;
            case NaddiStates.Digging:
                SetAnimationState(false, true, false, 0);
                break;
            case NaddiStates.LookForPlayer:
                if (_naddi.AttackBehaviour.Agent.isStopped)
                {
                    SetAnimationState(false, false, false, 0);
                }
                else
                {
                    SetAnimationState(true, false, false, _naddi.Speed);
                }
                break; 
            case NaddiStates.Idle:
                SetAnimationState(false, false, false, 0);
                break;
            case NaddiStates.Attack:
                SetAnimationState(false, false, true, 0);
                break; 
        }
    }

    private void SetAnimationState(bool isMoving, bool isComouflaging, bool isAttacking, float vel)
    {
        _anim.SetBool("IsMoving", isMoving);
        _anim.SetBool("IsCamouflaging", isComouflaging);
        _anim.SetBool("IsAttacking", isAttacking);
        _anim.SetFloat("Velocity", vel);
    }
}