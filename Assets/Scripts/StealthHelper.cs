using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthHelper : MonoBehaviour
{
    [Header("Delete this script when Player has a Stealth functionality coppled with the Naddi")]
    public NaddagilHearingSensor NaddiHearing;

    private Vector3 position; 

    // Start is called before the first frame update
    private void Start()
    {
        position = transform.position; 
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            NaddiHearing.SetSoundMofiyer(NaddiHearing.GetHalfVolumeModifyer);
        }
        else if (position == transform.position)
        {
            NaddiHearing.SetSoundMofiyer(NaddiHearing.MinVolumeModifyer);
        }
        else
        {
            NaddiHearing.SetSoundMofiyer(NaddiHearing.MaxVolumeModifyer);
        }
    }

    private void LateUpdate()
    {
        position = transform.position;
    }
}
