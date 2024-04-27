using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[Serializable]
public class MoveEvent : ScriptedEvent
{

    [SerializeField] private SplineContainer _movePath;
    [SerializeField] private float _splineMoveSpeed;
    [SerializeField] private SplineAnimate _splineAnimator;

    public override void StartEvent() {
        base.StartEvent();
        _splineAnimator.enabled = true;
        _splineAnimator.Container = _movePath;
        _splineAnimator.AnimationMethod = SplineAnimate.Method.Speed;
        _splineAnimator.MaxSpeed = _splineMoveSpeed;
        _splineAnimator.Play();
    }

    public override bool UpdateEvent(float deltaTime) {
        if(base.UpdateEvent(deltaTime) == false) {
            return false;
        }
        if(_splineAnimator.ElapsedTime > 0 && _splineAnimator.IsPlaying == false) {
            InvokeEventFinished();

        }
        else if(_splineAnimator.IsPlaying == false) {
            _splineAnimator.Play();
        }

        return true;
    }
}
