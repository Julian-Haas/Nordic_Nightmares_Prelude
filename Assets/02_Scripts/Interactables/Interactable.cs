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
    public int _index = -1;
    public int _state = -1;
    protected string _type = "interactable";

    public string getType()
    {
        return _type;
    }

    void Awake()
    {
        _index = WorldStateData.Instance.AddInitialInteractableState(_state);
        _text = displayedInteractionText.GetComponentInChildren<Text>();
        displayedInteractionText.SetActive(false);
    }

    protected void UpdateState()
    {
        WorldStateData.Instance.UpdateInteractableState(_index, _state);
    }
    public int ReturnIndex()
    {
        return _index;
    }

    public void DisplayInteractionText(bool displayOrNot)
    {
        //if (displayOrNot)
        //{
        //    displayedInteractionText.SetActive(true);
        //}
        //else
        //{
        //    displayedInteractionText.SetActive(false);
        //}
    }

    protected void updateTooltipText()
    {

    }

    public abstract bool Interact(bool started);

    private void AnimateActivationKey()
    {
        if (_hoverUp)
        {
            _hover += Time.deltaTime*0.25f;
        }
        else if(!_hoverUp)
        {
            _hover -= Time.deltaTime * 0.25f;
        }
        if(_hover < -0.25f)
        {
            _hoverUp = true;
        }
        else if (_hover > 0.25f)
        {
            _hoverUp = false;
        }
        float tmpY = 3 + _hover;
        displayedInteractionText.transform.localPosition = new Vector3(0,tmpY,0);
    }
}