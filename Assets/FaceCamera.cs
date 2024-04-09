using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FaceCamera : MonoBehaviour
{
    Transform cameraTransform;
    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position - cameraTransform.up);
    }
}
