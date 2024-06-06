using UnityEngine;

public class Plank : Interactable
{
    public SoundManager _soundmanager;

    void Start() {
        _type = "plank";
    }
    public override void Interact() {
        if(Inventory.Instance.TryToGatherPlank(this)) {
            InteractableManager.Instance.RemoveInteractable(this);
            Destroy(gameObject);
        }
    }
}