using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interferable : MonoBehaviour
{
    Animation _animation;
    public AnimationClip Temporary, Permanent, Shudder;
    private bool _damaged = false;
    private NaddiAwareness _naddi;

    private void Start()
    {
        _animation = GetComponent<Animation>();
        _naddi = GameObject.Find("Naddi").GetComponentInChildren<NaddiAwareness>();
        _animation.AddClip(Temporary, Temporary.name);
        _animation.AddClip(Permanent,Permanent.name);
        _animation.AddClip(Shudder,Shudder.name);
    }

    //on collision with Player, get strength of interferance & call according action
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(_damaged)
            {
                Interferance(Shudder.name, true);
            }
            else if (other.GetComponent<PlayerMove>()._isSneaking) {
                Interferance(Temporary.name, true);
            }
            else if (!other.GetComponent<PlayerMove>()._isSneaking) {
                Interferance(Permanent.name, false);
                _damaged = true;
            }
        }
    }

    //play fitting animation & transfer input to NaddiAwareness 
    void Interferance( string clipName, bool Temp)
    {
        _animation.Play(clipName);
        _naddi.NatureSense(Temp, transform.position);
    }
}