//The person responsible for this code is Nils Oskar Henningsen 
using UnityEngine;

public class NaddiAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator _anim;
    [SerializeField]
    private NaddiStateMaschine _naddiState;
    private NaddiViewField _viewField;
    private bool hasBitten = false; 

    private void Start()
    {
        _viewField = this.GetComponent<NaddiViewField>(); 
    }
    private void Update()
    {
        AnimStateHandle();

        if (_naddiState.CurrentState == NaddiStateEnum.Digging || _naddiState.CurrentState == NaddiStateEnum.Attack)
        {
            CheckForAnimationFinished(); 
        }
    }

    void CheckForAnimationFinished()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).length >
               _anim.GetCurrentAnimatorStateInfo(0).normalizedTime)
        {
            if (_naddiState.CurrentState == NaddiStateEnum.Attack)
            {
                if (_anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
                {
                    _naddiState.FinishedAttacking(_viewField.isInsideCone());
                }
            }
         }
    }
    void AnimStateHandle()
    {
        switch (_naddiState.CurrentState)
        {
            case NaddiStateEnum.Patrol:
                StartMoving();
                break;
            case NaddiStateEnum.Chase:
                StartMoving();
                break;
            case NaddiStateEnum.Digging:
                StartDigging();
                break;
            case NaddiStateEnum.LookForPlayer:

                if (_naddiState.Naddi.Agent.isStopped)
                {
                    IdleAround();
                    break;
                }
                StartMoving();
                break; 
            case NaddiStateEnum.Idle:
                IdleAround();
                break;
            case NaddiStateEnum.Attack:
                StartAttacking();
                break; 
        }
    }
    private void StartAttacking()
    {
        _anim.SetBool("IsAttacking", true);
        _anim.SetBool("IsMoving", false);
        _anim.SetFloat("Velocity", 0f);
        _anim.SetBool("IsCamouflaging", false);
    }
    public void StartMoving()
    {
        _anim.SetBool("IsMoving", true);
        _anim.SetFloat("Velocity", _naddiState.Naddi.Speed);
        _anim.SetBool("IsAttacking", false);
    }

    public void StartDigging()
    {
        _anim.SetBool("IsCamouflaging", true);
        _anim.SetBool("IsMoving", false);
        _anim.SetFloat("Velocity", 0f);
        _anim.SetBool("IsAttacking", false);
    }

    public void IdleAround()
    {
        _anim.SetBool("IsMoving", false);
        _anim.SetFloat("Velocity", 0f);
        _anim.SetBool("IsCamouflaging", false);
       _anim.SetBool("IsAttacking", false); 
    }

}