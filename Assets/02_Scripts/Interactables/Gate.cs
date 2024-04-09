using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class Gate : Interactable
{
    [SerializeField] s_SoundManager _soundmanager; 
    public float angle = 0.0f;
    private Animation _animation;
    private s_PlayerCollider _playerCollider;
    private NoiseEmitter _noise;
    private int state = 0; // 0 = closed, 1 = openInward, 2 = openOutward

    void Start()
    {
        _animation = GetComponent<Animation>();
        _playerCollider = GameObject.Find("PlayerAnimated").GetComponent<s_PlayerCollider>();
        //_soundmanager = GameObject.Find("SoundManager").GetComponentInChildren<s_SoundManager>();
        _noise = gameObject.GetComponentInChildren<NoiseEmitter>();
    }

    public override bool Interact(bool started)
    {
        if (started)
        {
            Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z);
            Vector3 dirPlayer = new Vector3(_playerCollider.transform.position.x - transform.position.x, 0, _playerCollider.transform.position.z-transform.position.z);   
            angle = Vector3.Angle(forward, dirPlayer);
            switch (state)
            {
                case 0:
                    if(angle < 100)
                    {
                        _animation.Play("GateOpenInward");
                        _noise.MakeSound(0);
                        state = 1;
                    }
                    else {
                        _animation.Play("GateOpenOutward");
                        _noise.MakeSound(3);
                        state = 2;
                    }
                    break;
                case 1:
                    if (angle < 100)
                    {
                        _animation.Play("GateShudder");
                        _noise.MakeSound(1);
                    }
                    else
                    {
                        _animation.Play("GateCloseInward");
                        _noise.MakeSound(2);
                        state = 0;
                    }
                    break;
                case 2:
                    if (angle < 100)
                    {
                        _animation.Play("GateCloseOutward");
                        _noise.MakeSound(2);
                        state = 0;
                    }
                    else
                    {
                        _animation.Play("GateShudder");
                        _noise.MakeSound(1);
                    }
                    break;
                default:
                    break;
            }
            UpdateState();
        }
            return true;
    }
}