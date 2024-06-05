using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour
{
    public GameObject displayedInteractionText;
    public float _hover = 0.0f;
    protected bool _hoverUp = false;
    private Text _text;
    protected string _type = "interactable";

    public string getType() {
        return _type;
    }

    void Awake() {
        _text = displayedInteractionText.GetComponentInChildren<Text>();
        displayedInteractionText.SetActive(false);
    }

    public void DisplayInteractionText(bool displayOrNot) {
        //if (displayOrNot)
        //{
        //    displayedInteractionText.SetActive(true);
        //}
        //else
        //{
        //    displayedInteractionText.SetActive(false);
        //}
    }

    public abstract bool Interact(bool started);

    private void AnimateActivationKey() {
        //if (_hoverUp)
        //{
        //    _hover += Time.deltaTime*0.25f;
        //}
        //else if(!_hoverUp)
        //{
        //    _hover -= Time.deltaTime * 0.25f;
        //}
        //if(_hover < -0.25f)
        //{
        //    _hoverUp = true;
        //}
        //else if (_hover > 0.25f)
        //{
        //    _hoverUp = false;
        //}
        //float tmpY = 3 + _hover;
        //displayedInteractionText.transform.localPosition = new Vector3(0,tmpY,0);
    }
}