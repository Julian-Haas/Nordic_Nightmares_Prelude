using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour
{
    public int ChargesLeft = 3;
    public List<GameObject> _charges;
    public GameObject CompleteSafeSpace;

    //private void Start()
    //{
    //}
    //void OnTriggerEnter(Collider other)
    //{

    //}


    public void LeaveSafeSpace()
    {
        Debug.Log("triggered funktion of leave safespace");
        ChargesLeft--;
        if (ChargesLeft <= 0)
        {
            Destroy(CompleteSafeSpace);
        }
        _charges[ChargesLeft].SetActive(false);
    }

    //void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        ChargesLeft--;
    //        if (ChargesLeft <= 0)
    //        {
    //            Destroy(CompleteSafeSpace);
    //        }
    //        _charges[ChargesLeft].SetActive(false);
    //    }
    //}
}
