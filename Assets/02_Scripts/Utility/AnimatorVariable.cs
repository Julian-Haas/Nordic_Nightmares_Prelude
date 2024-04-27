using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class AnimatorVariable
{
    [SerializeField] protected string _key;

    public abstract void SetAnimatorVariable(Animator animator);
}
[Serializable]
public class AnimatorVariableFloat : AnimatorVariable
{
    [SerializeField] private float _value;

    public float Value {
        get {
            return _value;
        }

        set {
            _value = value;
        }
    }



    public override void SetAnimatorVariable(Animator animator) {
        animator.SetFloat(_key,_value);
    }
}
[Serializable]
public class AnimatorVariableBool : AnimatorVariable
{
    [SerializeField] private bool _value;

    public bool Value {
        get {
            return _value;
        }

        set {
            _value = value;
        }
    }



    public override void SetAnimatorVariable(Animator animator) {
        animator.SetBool(_key,_value);
    }
}
[Serializable]
public class AnimatorVariableInteger : AnimatorVariable
{
    [SerializeField] private int _value;

    public int Value {
        get {
            return _value;
        }

        set {
            _value = value;
        }
    }


    public override void SetAnimatorVariable(Animator animator) {
        animator.SetInteger(_key,_value);
    }
}
[Serializable]
public class AnimatorVariableTrigger : AnimatorVariable
{


    public override void SetAnimatorVariable(Animator animator) {
        animator.SetTrigger(_key);
    }
}