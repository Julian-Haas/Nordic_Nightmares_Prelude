using System.Collections;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public static DeathManager Instance;
    [SerializeField] private Naddi _naddi;
    SavePoint _lastSavePointTotem = null;
    Vector3 _respawnPoint;
    public s_PlayerCollider _playerScript;
    private s_SoundManager _soundManager;
    private Animator _DeathScreenAnimator;
    private GameObject _player;

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
            return;
        }
        else {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }
    void Start() {
        _soundManager = GameObject.Find("SoundManager").GetComponentInChildren<s_SoundManager>();
        _player = this.transform.root.gameObject;
        _playerScript = this.GetComponent<s_PlayerCollider>();
        _respawnPoint = this.transform.root.position;
        _DeathScreenAnimator = GameObject.Find("InGame_Canvas").transform.Find("DeathScreenOverlay").GetComponent<Animator>();
    }
    public void PlayerDies(bool NaddiCauseOfDeath) {
        _DeathScreenAnimator.SetBool("IsDeathScreen",true);
        if(NaddiCauseOfDeath) {
            _soundManager.PlaySound3D("event:/SFX/BiteDeath",_player.transform.position);
        }
        else {
            _soundManager.PlaySound2D("event:/SFX/WaterDeath");
        }
        StartCoroutine(RespawnCoroutine());
    }
    IEnumerator RespawnCoroutine() {
        yield return new WaitForSeconds(2.0f);
        _naddi.KilledPlayer = true;
        GameObject simplNaddiActive = GameObject.Find("SimpleNaddiManager");
        _naddi.ResetNaddiPosition();

        _playerScript._sanity = 100.0f;
        _player.transform.position = _respawnPoint;
        _DeathScreenAnimator.SetBool("IsDeathScreen",false);
        yield return new WaitForSeconds(0.1f);
        _soundManager.PlaySound3D("event:/SFX/PlayerAwakes",_player.transform.position);
        _playerScript.gameObject.GetComponent<PlayerControl>()?.PlayRespawnAnimation();
    }
    public void ActivateSavepoint(SavePoint SavepointToActivate,GameObject RespawnPoint) {
        _respawnPoint = RespawnPoint.transform.position;
        if(_lastSavePointTotem != null) {
            _lastSavePointTotem.DeactivateSavePoint();
        }
        _lastSavePointTotem = SavepointToActivate;
    }
}
