using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MoveToPositionEvent : ScriptedEvent
{

    [SerializeField] private Transform position;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private GameObject _moveObject;
    private Vector3 _moveDirection;

    public override void StartEvent() {
        base.StartEvent();
        _moveDirection = (position.position - _moveObject.transform.position).normalized;
    }


    public override void UpdateEvent(float deltaTime) {

        if(_eventStarted == false) {
            return;
        }

        Vector3 dir = position.position - _moveObject.transform.position;

        Vector3 moveDistance = _moveDirection * _moveSpeed * deltaTime;

        if(dir.magnitude <= moveDistance.magnitude) {
            _moveObject.transform.position = position.position;
            InvokeEventFinished();
        }
        else {
            _moveObject.transform.position = _moveObject.transform.position + moveDistance;
        }


    }
}
