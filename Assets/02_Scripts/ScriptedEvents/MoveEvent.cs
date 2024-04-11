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
    [SerializeField] private SplineAnimate _animator;
    private bool _enabledThisFrame;

    public override void StartEvent() {

        Debug.Log("spline move start");
        _animator.enabled = true;
        _animator.Container = _movePath;
        _animator.AnimationMethod = SplineAnimate.Method.Speed;
        _animator.MaxSpeed = _splineMoveSpeed;


        _animator.Play();
        _enabledThisFrame = true;

        base.StartEvent();
    }

    public override void UpdateEvent(float deltaTime) {
        if(_eventStarted == false) {
            return;
        }
        if(_animator.ElapsedTime > 0 && _animator.IsPlaying == false) {
            Debug.Log(_animator.ElapsedTime);
            InvokeEventFinished();

        }
        else if(_animator.IsPlaying == false) {
            _animator.Play();
        }


    }
}
