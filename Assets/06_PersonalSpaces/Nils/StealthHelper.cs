using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthHelper : MonoBehaviour
{
    public NaddiHearing naddi; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            naddi.SoundMofifyer = 0.5f;
        }
        else
        {
            naddi.SoundMofifyer = 1f;
        }
    }
}
