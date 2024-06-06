using UnityEngine;

public class Plank : Interactable
{
    void Start() {
        _type = "plank";
    }
    public override void Interact() {
        if(Inventory.Instance.TryToGatherPlank()) {
            InteractableManager.Instance.RemoveInteractable(this);
            Destroy(gameObject);
        }
    }
}