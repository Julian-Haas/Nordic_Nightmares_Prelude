//The person responsible for this code is Nils Oskar Henningsen 
using UnityEngine;

public class NaddiAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator _anim;
    [SerializeField]
    private NaddiStateMaschine _naddiState;


    private void Update()
    {
        AnimStateHandle(); 
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
                StartMoving(); 
                break; 
            case NaddiStateEnum.Idle:
                IdleAround();
                break;
        }
    }
     
    public void StartMoving()
    {
        _anim.SetBool("IsMoving", true);
        _anim.SetFloat("Velocity", _naddiState.Naddi.Speed); 
    }

    public void StartDigging()
    {
        _anim.SetBool("IsCamouflaging", true);
    }

    public void IdleAround()
    {
        _anim.SetBool("IsMoving", false);
        _anim.SetFloat("Velocity", 0f);
        _anim.SetBool("IsCamouflaging", false);
    }

}