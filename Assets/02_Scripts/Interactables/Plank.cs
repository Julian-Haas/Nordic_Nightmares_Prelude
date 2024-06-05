using UnityEngine;

public class Plank : Interactable
{
    public s_SoundManager _soundmanager;

    void Start() {
        _type = "plank";
    }
    public override bool Interact(bool started) {
        if(started) {
            if(Inventory.Instance.TryToGatherPlank(this)) {
                Destroy(gameObject);
                return false;
            }
        }
        return true;
    }
}