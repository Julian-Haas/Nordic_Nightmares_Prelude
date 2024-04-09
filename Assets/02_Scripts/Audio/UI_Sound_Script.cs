using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Sound_Script : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip ButtonHoverSound;
    public AudioClip ButtonClickSound;

    public void HoverSound ()
    {
        AudioSource.PlayOneShot(ButtonHoverSound);
    }
    public void ClickSound()
    {
        AudioSource.PlayOneShot(ButtonClickSound);
    }





    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
