using UnityEngine;
using System;
using System.Collections;

public class NaddagilHearingSensor : MonoBehaviour
{
    [Header("Public Propertys")]
    public float MaxDistance = 10f;
    public float MinVolumeModifyer { get; private set; } = 0f;
    public float MaxVolumeModifyer { get; private set; } = 1f;
    public float GetHalfVolumeModifyer { get { return MaxVolumeModifyer / 2;  } }
    public float SoundSum { get; private set; } = 0f;
    [Tooltip("When should the Naddi start looking at the player?")]
    public float LookForPlayerTrigger = 0.5f;
    [Tooltip("When should the Naddi start hounting the Player?")]
    public float AttackPlayerTrigger = 1f;

    //public Events
    public Action<Vector3> LookForPlayerAction;
    public Action AttackPlayerAction;

    //private Propertys
    [Range(0.5f, 1), Tooltip("Is the Player walking or sneeking?The Values should be between 0.5f - 1, where 0.5 = sneeking, 1 = normal walking ")]
    private float _soundModifyer;
    [Range(0, 1), Tooltip("How loud is the Sound on the given ground? The Values should be between 0 - 1, where 0 = insulating material, 1 = Gravel")]
    private float _groundModifyer;
    private float _decay = 0.99f;

    [Header("Private References")]
    [SerializeField, Tooltip("Player reference is needed to check if the listener chould listen for Player noises or not")]
    private PlayerControl _playerRef;
    [SerializeField]
    private Naddagil _naddagil;

    //private flags
    private bool _canHearSomething = true;

    private void Awake()
    {
        _naddagil.AttackBehaviour.PlayerPos = _playerRef.gameObject.transform;
        _soundModifyer = MaxVolumeModifyer;
        _groundModifyer = MaxVolumeModifyer;
        AttackPlayerTrigger = _naddagil.ValueStorage.AttackTriggerVal;
        LookForPlayerTrigger = _naddagil.ValueStorage.LookForPlayerVal;
        MaxDistance = _naddagil.ValueStorage.HearingRange; 
    }

    private void Update()
    {
        if(_playerRef.GetSneakingStatus() == true) 
        {
            _soundModifyer = GetHalfVolumeModifyer; 
        } else 
        {
            _soundModifyer = MaxVolumeModifyer; 
        }
        AddSoundValue(); 
    }

    private void AddSoundValue()
    {
        if (_canHearSomething == false)
            return;

        if (SoundSum >= 0)
        {
            float distance = Vector3.Distance(this.transform.position, _naddagil.AttackBehaviour.PlayerPos.position);

            if (SoundSum > 0 && _naddagil.State != NaddiStates.LookForPlayer && _naddagil.State != NaddiStates.Chase || distance > (MaxDistance*1.33f))
                SoundSum *= _decay;

            if (distance > MaxDistance || _naddagil.PlayerCol._inSafeZone == true)
                return;

            if (SoundSum > LookForPlayerTrigger && SoundSum < AttackPlayerTrigger)
                LookForPlayerAction.Invoke(_naddagil.AttackBehaviour.PlayerPos.position);
            else if (SoundSum > AttackPlayerTrigger)
            {
                _naddagil.AttackBehaviour.PlayerPosLastSeen = _naddagil.AttackBehaviour.PlayerPos.position; 
                AttackPlayerAction.Invoke();
            }



            if (_playerRef.IsMoving() == false)
                return;

            SoundSum += 1 - (distance / MaxDistance) * _soundModifyer * _groundModifyer * Time.deltaTime;
            //Debug.Log("Sound Sum:\n"+ SoundSum); 
        }
    }

    public void ResetSoundSum() 
    {
        SoundSum = 0;
    }
    public IEnumerator ListenerDelay() 
    {
        _canHearSomething = false;
        yield return new WaitForSeconds(2);
        _canHearSomething = true; 
    }

    public void SetSoundMofiyer(float val)
    {
        _soundModifyer = val; 
    }
} 