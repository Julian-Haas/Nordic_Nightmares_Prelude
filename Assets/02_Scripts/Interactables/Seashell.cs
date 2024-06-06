using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seashell : Interactable
{
    void Start() {
        _type = "seashell";
    }
    public override void Interact() {
        if(Inventory.Instance.TryToPickUpSeashell()) {
            Destroy(gameObject);
        }
    }
}
