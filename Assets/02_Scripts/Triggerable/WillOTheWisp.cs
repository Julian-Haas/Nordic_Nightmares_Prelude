using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WillOTheWisp : Triggerable
{
    [SerializeField] private GameObject _willOTheWisp;
    [SerializeField] private GameObject _triggerCollider;
    [SerializeField] private List<GameObject> _waypoints;
    [SerializeField] private bool _moves;
    [SerializeField] private float _movementspeed;
    [SerializeField] private float _shrinktime;

    public override void Trigger() {
        if(_waypoints.Count == 0) {
            StartCoroutine(Vanish());
        }
        else {
            if(_moves) {

            }
            else {
                StartCoroutine(Teleport());
            }
        }
    }

    private IEnumerator Teleport() {
        _triggerCollider.SetActive(false);
        Vector3 originalScale = _willOTheWisp.transform.localScale;
        float timer = 0f;
        while(timer < _shrinktime) {
            float scaleProgress = timer / _shrinktime;
            Vector3 newScale = Vector3.Lerp(originalScale,Vector3.zero,scaleProgress);
            _willOTheWisp.transform.localScale = newScale;
            timer += Time.deltaTime;
            yield return null;
        }
        _willOTheWisp.transform.localScale = Vector3.zero;
        _willOTheWisp.transform.position = _waypoints[0].transform.position;
        timer = 0f;
        while(timer < _shrinktime) {
            float scaleProgress = timer / _shrinktime;
            Vector3 newScale = Vector3.Lerp(Vector3.zero,originalScale,scaleProgress);
            _willOTheWisp.transform.localScale = newScale;
            timer += Time.deltaTime;
            yield return null;
        }
        _waypoints.RemoveAt(0);
        _triggerCollider.SetActive(true);
    }

    private IEnumerator Vanish() {
        Destroy(_triggerCollider);
        Vector3 originalScale = _willOTheWisp.transform.localScale;
        float timer = 0f;
        while(timer < _shrinktime) {
            float scaleProgress = timer / _shrinktime;
            Vector3 newScale = Vector3.Lerp(originalScale,Vector3.zero,scaleProgress);
            _willOTheWisp.transform.localScale = newScale;
            timer += Time.deltaTime;
            yield return null;
        }
        _willOTheWisp.transform.localScale = Vector3.zero;
        Destroy(transform.root.gameObject);
    }

    private IEnumerator Move() {
        Vector3 originalScale = _willOTheWisp.transform.localScale;
        float timer = 0f;
        while(timer < _shrinktime) {
            float scaleProgress = timer / _shrinktime;
            Vector3 newScale = Vector3.Lerp(originalScale,Vector3.zero,scaleProgress);
            _willOTheWisp.transform.localScale = newScale;
            timer += Time.deltaTime;
            yield return null;
        }
        _willOTheWisp.transform.localScale = Vector3.zero;
        Destroy(transform.root.gameObject);
    }

}
