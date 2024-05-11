using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthHelper : MonoBehaviour
{
    public NaddiHearing naddi;
    Vector3 position; 
    // Start is called before the first frame update
    void Start()
    {
        position = transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            naddi.SetSoundModifyer = naddi.GetMinValumeModifyer;
        }
        else if (position == transform.position)
        {
            naddi.SetSoundModifyer = naddi.GetMinValumeModifyer;
        }
        else
        {
            naddi.SetSoundModifyer = naddi.GetMaxValumeModifyer;
        }
    }

    private void LateUpdate()
    {
        position = transform.position;
    }
}
