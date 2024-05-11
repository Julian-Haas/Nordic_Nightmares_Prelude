using UnityEngine;
using System;

public class NaddiHearing : MonoBehaviour
{

    [SerializeField]
    private float minVolumeModifyer = 0f;
    [SerializeField]
    private float maxVolumeModifyer = 1f;
    public float GetMinValumeModifyer { get { return minVolumeModifyer; } }
    public float GetHalfVolumeModifyer { get { return maxVolumeModifyer / 2;  } }
    public float GetMaxValumeModifyer { get { return maxVolumeModifyer; } } 

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
    [SerializeField]
    float _maxDistance = 10f;
    [SerializeField, Tooltip("Player reference is needed to check if the listener chould listen for Player noises or not")]
    private PlayerControl _playerRef; 
    private Naddi _naddi;
    private Transform _playerPos;


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
    }

    private void Update()
    {
        AddSoundValue(); 
    }

    public void AddSoundValue()
    {
        if (_soundSum >= 0)
        {
            float distance = Vector3.Distance(this.transform.position, _playerPos.position);
            if (_soundSum > 0 && _naddi.State != NaddiStateEnum.LookForPlayer && _naddi.State != NaddiStateEnum.Chase || distance > (_maxDistance*1.33f))
            {
                _soundSum *= _decay;
            }

            if (distance > _maxDistance)
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

            _soundSum += 1 - (distance / _maxDistance) * _soundModifyer * _groundModifyer * Time.deltaTime;
        }
    }
} 