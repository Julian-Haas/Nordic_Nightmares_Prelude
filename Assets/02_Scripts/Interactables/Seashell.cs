using UnityEngine;

public class Seashell : Interactable
{
    void Start() {
        _type = "seashell";
    }
    public override void Interact() {
        if(Inventory.Instance.TryToPickUpSeashell()) {
            InteractableManager.Instance.RemoveInteractable(this);
            Destroy(gameObject);
        }
    }
}
