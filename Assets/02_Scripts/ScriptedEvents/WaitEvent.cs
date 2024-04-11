using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaitEvent : ScriptedEvent
{
    [SerializeField] float duration;

    private float _currentTime;


    public override void StartEvent() {
        base.StartEvent();
        _currentTime = 0;
    }

    public override void UpdateEvent(float deltaTime) {
        if(_eventStarted == false) {
            return;
        }
        _currentTime = _currentTime + deltaTime;
        if(_currentTime > duration) {
            InvokeEventFinished();
        }
    }

}
