using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Groundcheck : MonoBehaviour
{
    [SerializeField] private PlayerMove PlayerM;
    private void OnTriggerEnter(Collider other)
    {
        PlayerM.IsGround = true;
    }
}
