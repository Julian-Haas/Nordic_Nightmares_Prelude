using UnityEngine;

public class Plank : Interactable
{
    public Inventory _inventory;
    public s_SoundManager _soundmanager; // FMOD Sound Manager

    void Start() {
        _type = "plank";
        _soundmanager = GameObject.Find("SoundManager").GetComponentInChildren<s_SoundManager>();
        _inventory = GameObject.Find("Inventory").GetComponentInChildren<Inventory>();
    }

    //public void RemovePlankFromScene()
    //{
    //    this.transform.Find("PlankCollider").gameObject.SetActive(false);
    //    this.transform.Find("SM_BridgePlank_00_Blockout").gameObject.SetActive(false);
    //    gameObject.SetActive(false);
    //}
    //public void ReturnPlankToScene()
    //{
    //    this.transform.Find("PlankCollider").gameObject.SetActive(true);
    //    this.transform.Find("SM_BridgePlank_00_Blockout").gameObject.SetActive(true);
    //}

    public override bool Interact(bool started) {
        if(started) {
            //_soundmanager.PlaySound("event:/pickUpWood_sfx", this.transform.position);
            if(Inventory.Instance.TryToGatherPlank(this)) {
                Destroy(gameObject);
                //RemovePlankFromScene();
                return false;
            }
        }
        return true;
    }
}