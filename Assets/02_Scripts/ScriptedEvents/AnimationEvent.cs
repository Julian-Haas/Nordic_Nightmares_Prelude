using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationEvent : ScriptedEvent
{
    [SerializeField] private Animator _animator;

    [SerializeReference, SubclassSelector]
    private List<AnimatorVariable> _animatorVariables;


    public override bool UpdateEvent(float deltaTime) {
        if(base.UpdateEvent(deltaTime) == false) {
            return false;
        }

        foreach(AnimatorVariable variable in _animatorVariables) {
            variable.SetAnimatorVariable(_animator);
        }
        InvokeEventFinished();

        return true;
    }
}
