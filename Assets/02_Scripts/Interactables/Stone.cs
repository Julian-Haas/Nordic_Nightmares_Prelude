using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Interactable
{
    public Inventory _inventory;
    public s_SoundManager _soundmanager; // FMOD Sound Manager

    void Start()
    {
        _soundmanager = GameObject.Find("SoundManager").GetComponentInChildren<s_SoundManager>();
        _inventory = GameObject.Find("Inventory").GetComponentInChildren<Inventory>();
    }

    public void RemoveStoneFromScene()
    {
        this.transform.Find("StoneCollider").gameObject.SetActive(false);
        this.transform.Find("SM_Stone_00_Blockout").gameObject.SetActive(false);
    }
    public void ReturnStoneToScene()
    {
        this.transform.Find("StoneCollider").gameObject.SetActive(true);
        this.transform.Find("SM_Stone_00_Blockout").gameObject.SetActive(true);
    }

    public override bool Interact(bool started)
    {
        if (started)
        {
            if (_inventory.TryToGatherStone(this))
            {
                RemoveStoneFromScene();
                return false;
            }
        }
        return true;
    }
}