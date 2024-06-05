using UnityEngine;
using System;
using System.Collections;

public class NaddiHearing : MonoBehaviour
{

    [SerializeField]
    private float minVolumeModifyer = 0f;
    [SerializeField]
    private float maxVolumeModifyer = 1f;
    public float GetMinValumeModifyer { get { return minVolumeModifyer; } }
    public float GetHalfVolumeModifyer { get { return maxVolumeModifyer / 2;  } }
    public float GetMaxValumeModifyer { get { return maxVolumeModifyer; } } 
    public float GetSoundSum { get { return _soundSum; } }
    public float LookForPlayerTrigger { get { return _LookForPlayerTrigger;  } set { _LookForPlayerTrigger = value; } }
    public float AttackTrigger { get { return _attackPlayerTrigger;  } set { _attackPlayerTrigger = value;  } }
    [SerializeField, Range(0.5f, 1), Tooltip("Is the Player walking or sneeking?The Values should be between 0.5f - 1, where 0.5 = sneeking, 1 = normal walking ")]
    private float _soundModifyer;
    [SerializeField, Range(0, 1), Tooltip("How loud is the Sound on the given ground? The Values should be between 0 - 1, where 0 = insulating material, 1 = Gravel")]
    private float _groundModifyer;
    [SerializeField]
    private float _soundSum = 0f;
    [SerializeField, Tooltip("When should the Naddi start looking at the player?")]
    private float _LookForPlayerTrigger = 0.5f;
    [SerializeField, Tooltip("When should the Naddi start hounting the Player?")]
    private float _attackPlayerTrigger = 1f;
    [SerializeField]
    private float _decay = 0.99f;
    public float MaxDistance = 10f;
    [SerializeField, Tooltip("Player reference is needed to check if the listener chould listen for Player noises or not")]
    private PlayerControl _playerRef; 
    private Naddi _naddi;
    private Transform _playerPos;
    private s_PlayerCollider _playerCollider;
    [SerializeField]
    private NaddiValueStorage valStorage; 

    public Action<Vector3> LookForPlayerAction;
    public Action AttackPlayerAction;

    public float SetSoundModifyer
    {
        set
        {
            if (value >= minVolumeModifyer && value <= maxVolumeModifyer)
            {
                _soundModifyer = value;
            }
            else
            {
                throw new System.ArgumentOutOfRangeException("The value should be between: " + minVolumeModifyer + "and: " + maxVolumeModifyer);
            }
        }
    }

    public float SetGroundModifyer
    {
        set
        {
            if (value >= 0 && value <= 1)
            {
                _groundModifyer = value;
            }
            else
            {
                throw new System.ArgumentOutOfRangeException("The value should be between 0 and 1!");
            }
        }
    }

    private void Awake()
    {
        _naddi = this.GetComponent<Naddi>();
        _playerPos = _playerRef.gameObject.transform;
        _soundModifyer = GetMaxValumeModifyer;
        _groundModifyer = GetMaxValumeModifyer;
        AttackTrigger = valStorage.AttackTriggerVal;
        LookForPlayerTrigger = valStorage.LookForPlayerVal;
        MaxDistance = valStorage.HearingRange; 
    }
    private void Start()
    {
        _playerCollider = _playerPos.gameObject.GetComponent<s_PlayerCollider>();
    }
    private void Update()
    {

        if(_playerRef.GetSneakingStatus() == true) 
        {
            _soundModifyer = GetHalfVolumeModifyer; 
        } else 
        {
            _soundModifyer = GetMaxValumeModifyer; 
        }
            AddSoundValue(); 
    }

    public void AddSoundValue()
    {
        if (canHearSomething == false)
            return; 
        if (_soundSum >= 0)
        {
            float distance = Vector3.Distance(this.transform.position, _playerPos.position);
            if (_soundSum > 0 && _naddi.State != NaddiStateEnum.LookForPlayer && _naddi.State != NaddiStateEnum.Chase || distance > (MaxDistance*1.33f))
            {
                _soundSum *= _decay;
            }

            if (distance > MaxDistance || _playerCollider._inSafeZone == true)
            {
                return;
            }

            if (_soundSum > _LookForPlayerTrigger && _soundSum < _attackPlayerTrigger)
            {
                LookForPlayerAction.Invoke(_playerPos.position);
            }
            else if (_soundSum > _attackPlayerTrigger)
            {
                AttackPlayerAction.Invoke();
            }

            if (_playerRef.IsMoving() == false)
            {
                return;
            } 

            _soundSum += 1 - (distance / MaxDistance) * _soundModifyer * _groundModifyer * Time.deltaTime;
        }
    }

    public void ResetSoundSum() 
    {
        _soundSum = 0;
    }
    bool canHearSomething = true; 
    public IEnumerator ListenerDelay() 
    {
        canHearSomething = false;
        yield return new WaitForSeconds(2);
        canHearSomething = true; 
    }
} 