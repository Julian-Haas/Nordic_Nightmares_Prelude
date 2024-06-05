using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanityZoom : MonoBehaviour
{
    [SerializeField] float maxSanitySize = 5f;
    [SerializeField] float mindSanitySize = 1.5f;

    Camera cam;

    private void Start() {
        cam = GetComponent<Camera>();
    }

    private void UpdateZoom(float newSanity) {
        float normalisedSanity = newSanity / 100f;

        cam.orthographicSize = Mathf.Lerp(mindSanitySize,maxSanitySize,normalisedSanity);
    }

}
