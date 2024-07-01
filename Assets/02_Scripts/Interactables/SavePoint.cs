using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : Interactable
{
    public SoundManager _soundmanager;
    public GameObject _player;
    public DeathManager _DeathManager;
    public GameObject _respawnPoint;
    [SerializeField] private Animator _animator;
    public GameObject _triggerCollider;

    void Start() {
        _type = "SavePoint";
        _soundmanager = GameObject.Find("SoundManager").GetComponentInChildren<SoundManager>();
        _player = GameObject.Find("PlayerAnimated");
        _DeathManager = _player.GetComponent<DeathManager>();

    }

    public override void Interact() {
        _soundmanager.PlaySound2D("event:/SFX/Savepoint");
        //this.transform.Rotate(0.0f, 180.0f, 180.0f, Space.Self);
        //this.transform.localPosition = this.transform.localPosition + new Vector3(0.0f, 1.0f, 0.0f);
        _animator.SetTrigger("IsActivated");
        Debug.Log("_animator");
        // aktiviere vfx von adina
        // spiele sound ab
        _triggerCollider.SetActive(false);
        _DeathManager.ActivateSavepoint(this,_respawnPoint);
    }

    public void DeactivateSavePoint() {
        //this.transform.localPosition = this.transform.localPosition + new Vector3(0.0f, -1.0f, 0.0f);
        _triggerCollider.SetActive(true);
        _animator.SetTrigger("DEACTIVATE");
        //triggere/boole animation von adina zurück
    }
}

