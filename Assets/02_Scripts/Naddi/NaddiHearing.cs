using UnityEngine;
using System;

public class NaddiHearing : MonoBehaviour
{
    [SerializeField, Range(0.5f, 1), Tooltip("Is the Player walking or standing?The Values should be between 0.5f - 1, where 0.5 = sneeking, 1 = normal walking ")]
    private float _soundModifyer = 1;
    [SerializeField, Range(0, 1), Tooltip("How loud is the Sound on the given ground? The Values should be between 0 - 1, where 0 = insulating material, 1 = Gravel")]
    private float _groundModifyer = 1;
    [SerializeField]
    private float _soundSum = 0f;
    [SerializeField, Tooltip("When should the Naddi start looking at the player?")]
    private float _suspisiosTrigger = 0.5f;
    [SerializeField, Tooltip("When should the Naddi start hounting the Player?")]
    private float _minValForHount = 1f;
    [SerializeField]
    private float _decay = 0.99f;
    [SerializeField]
    float _minDistance = 10f; 

    private Naddi _naddi;
    private Transform _playerPos;


    public Action<Vector3> OnSoundHeardAtPosition;
    public Action OnPlayerSoundHeardNearBy;

    public float SoundMofifyer
    {
        set
        {
            if (value >= 0.5 && value <= 1)
            {
                _soundModifyer = value;
            }
            else
            {
                throw new System.ArgumentOutOfRangeException("The value should be between 0.5 and 2!");
            }
        }
    }

    public float GroundModifyer
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
        _playerPos = _naddi.PlayerPos; 
    }

    private void Update()
    {
        AddSoundValue(); 
    }

    public void AddSoundValue()
    {
        float distance = Vector3.Distance(this.transform.position, _playerPos.position); 
        if (distance < _minDistance)
        {
            _soundSum += 1 - (distance / _minDistance) * _soundModifyer * _groundModifyer;

            if (_soundSum > _suspisiosTrigger)
            {
                OnSoundHeardAtPosition.Invoke(_playerPos.position);
            }

            if(_soundSum > _minValForHount)
            {
                OnPlayerSoundHeardNearBy.Invoke(); 
            }
        }

        if (_naddi.State != NaddiStateEnum.LookForPlayer && _naddi.State != NaddiStateEnum.Attack && _naddi.State != NaddiStateEnum.Attack)
        {
            _soundSum *= _decay;
        }
    }
} 