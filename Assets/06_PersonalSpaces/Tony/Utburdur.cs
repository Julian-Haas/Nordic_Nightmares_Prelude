using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.VFX;

public class Utburdur : MonoBehaviour {
    /*
    [SerializeField] private HitArea _hitArea;
    public VisualEffect Alerted;
    private SplineAnimate _splineAnimate;

    private void OnEnable() {
        _hitArea.TriggerHit += PlayerFound;
        _hitArea.TriggerLeft += ContinuePatroling;
    }
    private void OnDisable() {
        _hitArea.TriggerHit -= PlayerFound;
        _hitArea.TriggerLeft -= ContinuePatroling;
    }
    private void Awake() {
        _splineAnimate = GetComponent<SplineAnimate>();
        Alerted.Play();
        Alerted.pause = true;
    }

    private void Start()
    {
        _splineAnimate.Pause();
    }

    private void PlayerFound(Vector3 position) {
        GlobalData.Instance.Enemy?.AlertPlayerPosition();
        _splineAnimate.Pause();
        Alerted.pause = false;
    }
    public void ContinuePatroling() {
        _splineAnimate.Play();
        GlobalData.Instance.Enemy?.LostPlayerPosiiton();
        Alerted.pause = true;
    }
     */
}