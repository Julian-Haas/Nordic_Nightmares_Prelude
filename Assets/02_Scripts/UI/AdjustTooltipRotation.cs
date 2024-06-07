using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustTooltipRotation : MonoBehaviour
{
    // Start is called before the first frame update
    float rotationblaaa = 0.25f;
    float _isometricCorrectionAngle = 0.25f;
    void Start()
    {
        //this.transform.Rotate(0, - transform.parent.eulerAngles.y, 0);
        transform.rotation = new Quaternion(0.109381668f, -0.875426114f, 0.234569758f, 0.408217877f); 
    }
}
