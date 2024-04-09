using UnityEditor;
using UnityEngine;

public class NaddiSimpleActive : MonoBehaviour
{
    private float _localNaddiMusicMultiplier;
    NaddiSimple _activeNaddi;
    [Header("Adjustables")]
    public float PathWalkSpeed = 5.0f;
    public float ChaseSpeed = 7.0f;
    public float RotationSpeed = 7.5f;
    public float StareTime = 5.0f;
    public float OverlayStart = 10.0f;
    public float OverlayMax = 5.0f;
    public float HearingRange = 15.0f;
    public float HearingAwarenessIncreaseLight = 5.0f;
    public float HearingAwarenessIncreaseStrong = 15.0f;
    public float AwarenessReduction = 10.0f;
    [Header("Control Values")]
    [SerializeField] float Distance = 0.0f;
    [SerializeField] float HearingDistance = 0.0f;
    [SerializeField] float Awareness = 0.0f;
    [SerializeField] Material _matOverlay;
    [SerializeField] PathSimple _activePath;

    private s_SoundManager _soundManager;
    GameObject _player;
    s_PlayerCollider _playerCollider;
    NaddiTrackTarget _playerTracker;
    PlayerControl _playerControl;

    private void Start()
    {
        _soundManager = GameObject.Find("SoundManager").GetComponentInChildren<s_SoundManager>();
        _player = GameObject.Find("PlayerAnimated");
        _playerCollider = _player.GetComponent<s_PlayerCollider>();
        _playerTracker = _player.GetComponentInChildren<NaddiTrackTarget>();
        _playerControl = _player.GetComponent<PlayerControl>();
        Awareness = 0.0f;
    }
    private void Update()
    { 
        _activeNaddi?.ManualUpdate();
        if (_activeNaddi)
        {
            _localNaddiMusicMultiplier = _soundManager._naddiMusicMultiplier;
            _soundManager.musicInstance.SetParameter("NaddiR", Mathf.Clamp(Vector3.Distance(_activeNaddi.transform.position, _player.transform.position) * _localNaddiMusicMultiplier, 0.0f, 1.0f));
            //GetDistance();
            CheckHearing();
            SetNaddiOverlay();
        }
    }

    public void SetActiveNaddi( PathSimple newPath, NaddiSimple active)
    {
        _activePath?.DeactivatePath();
        _activePath = newPath;
        active._player = _playerCollider;
        active._trackerPlayer = _playerTracker;
        active.PathWalkSpeed = PathWalkSpeed;
        active.ChaseSpeed = ChaseSpeed;
        active.RotationSpeed = RotationSpeed;
        active.StareTime = StareTime;
        _activeNaddi = active;
    }

    public void ReloadAtCheckpoint() {
        _activePath?.StartFurthestPoint(_player.transform.position);
        _activeNaddi?.ResetNaddi();
        Awareness = 0.0f;
    }

    void CheckHearing()
    {
        Distance = Vector3.Distance(_activeNaddi.transform.position, _player.transform.position);

        if (Distance <= HearingRange + _playerControl.GetNoiseRange() && _playerControl.IsMoving()) 
        {
            if (_playerControl.GetSneakingStatus()) 
            {
                Awareness += HearingAwarenessIncreaseLight * Time.deltaTime;
            }
            else
            {
                Awareness += HearingAwarenessIncreaseStrong * Time.deltaTime;
            }
        
        }
        else
        {
            Awareness -= AwarenessReduction * Time.deltaTime;
        }

        Awareness = Mathf.Clamp(Awareness, 0.0f, 101.0f);

        if(Awareness >= 100.0f)
        {
            _activeNaddi.HeardPlayer();
        }
    }

    private void SetNaddiOverlay()
    {
        float tmp = Mathf.Clamp(Awareness / 100.0f, 0.0f, 1.0f);
        _matOverlay.SetFloat("_FadeInOverlay", tmp);
    }

    void GetDistance()
    {
        Distance = Vector3.Distance(_activeNaddi.transform.position, _player.transform.position);

        if (Distance < OverlayStart)
        {
            float tmp = Mathf.Clamp((OverlayStart - Distance) / OverlayMax, 0.0f, 1.0f);
            _matOverlay.SetFloat("_FadeInOverlay", tmp);

        }
        else
        {
            _matOverlay.SetFloat("_FadeInOverlay", 0.0f);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_player != null && _activeNaddi != null) 
        {
            Handles.color = Color.blue;
            Handles.DrawLine(_activeNaddi.transform.position, _player.transform.position, 10);

            //Draw Hearing Areas
            Handles.color = Color.white;
            Handles.DrawWireDisc(_activeNaddi.transform.position, Vector3.up, HearingRange, 3.0f);
        }
    }
#endif
}