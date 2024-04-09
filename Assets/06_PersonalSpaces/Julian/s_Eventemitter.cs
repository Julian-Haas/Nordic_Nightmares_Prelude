using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class s_Eventemitter : MonoBehaviour
{
    public List<string> Spielnamen;
    public event Action<string> PublishAGame;
    //private static string gameName = "Nordic Nightmares";
    //private static string gameName = "Nordic Nightmares";
    //public void Start()
    //{
    //    gameName = "Nordic Nightmares";

    //    GameStarted?.Invoke(gameName);
    //}

    public void ChangeGamename(string NameDesneuenSpiels)
    {
        Spielnamen.Add(NameDesneuenSpiels);

        PublishAGame?.Invoke(NameDesneuenSpiels);
    }

    private void Start()
    {
        ChangeGamename("Nordic Nightmares");
    }

}


//s_Eventemitter _eventemitter;
//public void DebugFunction(string a)
//{
//    Debug.Log(a);
//}
//private void Awake()
//{
//    _eventemitter = GameObject.Find("Eventemitter").GetComponent<s_Eventemitter>();
//    _eventemitter.PublishAGame += DebugFunction;
//}