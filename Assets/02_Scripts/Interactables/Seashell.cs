using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seashell : Interactable
{
    void Start() {
        _type = "seashell";
    }
    public override bool Interact(bool started) {
        if(started) {
            if(Inventory.Instance.TryToPickUpSeashell()) {
                Destroy(gameObject);
                return false;
            }
        }
        return true;
    }
}
