using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    public static InteractableManager Instance;
    private bool _pressedButton = false; 
    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
            //DontDestroyOnLoad(this);
        }
    }
    private List<Interactable> _nearbyInteractables = new List<Interactable>();
    //bool _alreadyKindledAFire = false;
    //bool _alreadyHadCollectedAPlank = false;
    //bool _alreadyHadBeenCloseToABridge = false;
    public void AddInteractable(Interactable interactableToAdd) {
        _nearbyInteractables.Add(interactableToAdd);
        if(_nearbyInteractables.Count == 1) {
            interactableToAdd.DisplayInteractionText(true);
            string typeOfOther = interactableToAdd.getType();
            switch(typeOfOther) {
                case "torch":
                    //if(!_alreadyKindledAFire) {
                    //    this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("Maybe I should kindle this fire?");
                    //}
                    break;
                case "bridge":
                    //if(!_alreadyHadBeenCloseToABridge) {
                    //    this.GetComponentInParent<Guidance>().displayGuidanceTooltipWithSpecificText("Maybe there is a way to repair this bridge.");
                    //    _alreadyHadBeenCloseToABridge = true;
                    //}
                    break;
                case "seashell":
                    //bla
                    break;
                default:
                    break;
            }
        }
    }
    public void RemoveInteractable(Interactable interactableToRemove) {
        _nearbyInteractables.Remove(interactableToRemove);
        interactableToRemove.GetComponentInParent<Interactable>().DisplayInteractionText(false);
        if(_nearbyInteractables.Count >= 1) {
            _nearbyInteractables[0].GetComponentInParent<Interactable>().DisplayInteractionText(true);
        }
    }
    private void Update() {
        Interactable objectToSwap;
        for(int i = 0; i < _nearbyInteractables.Count - 1; i++) {
            if((this.transform.position - _nearbyInteractables[i].transform.position).sqrMagnitude > (this.transform.position - _nearbyInteractables[i + 1].transform.position).sqrMagnitude) {
                if(i == 0) {
                    _nearbyInteractables[0].GetComponentInParent<Interactable>().DisplayInteractionText(false);
                    _nearbyInteractables[1].GetComponentInParent<Interactable>().DisplayInteractionText(true);
                }
                objectToSwap = _nearbyInteractables[i];
                _nearbyInteractables[i] = _nearbyInteractables[i + 1];
                _nearbyInteractables[i + 1] = objectToSwap;
            }
        }
    }
    public void Interact() {
        if(_nearbyInteractables.Count > 0 && _pressedButton==false) {
            _pressedButton = true; 
            _nearbyInteractables[0].Interact();
            StartCoroutine(ButtonCoolDown()); 
        }
    }

    IEnumerator ButtonCoolDown()
    {
        yield return new WaitForSeconds(0.001f);
        _pressedButton = false; 
    }
}