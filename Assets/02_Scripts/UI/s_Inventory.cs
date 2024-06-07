using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class s_Inventory : MonoBehaviour
{
    private byte _amountOfKeptStones;
    private bool _isHavingPlank;
    private GameObject _uiPlank, _uiStone1, _uiStone2;
    private Plank _plankInInventory;
    private Stone _StoneInInventory1;
    private Stone _StoneInInventory2;
    private GameObject _player;
    private bool _gatheredFirstPlank;
    private s_PlayerCollider _playerCollider;
    private GameObject _plank = null;
    public s_SoundManager _soundmanager;
    public Animator _animator;

    void Start()
    {


        //_uiPlank = GameObject.Find("PlankUI");
        //_uiStone1 = GameObject.Find("Stone1UI");
        //_uiStone2 = GameObject.Find("Stone2UI");

        //_uiPlank.SetActive(false);
        //_uiStone1.SetActive(false);
        //_uiStone2.SetActive(false);

        _player = GameObject.Find("PlayerAnimated");
        _playerCollider = _player.GetComponent<s_PlayerCollider>();
        //Debug.Log(_player);
        //Debug.Log(_player.transform.Find("A_Character_03"));
        //Debug.Log(_player.transform.Find("A_Character_03").transform.Find("Rig_Player"));
        //Debug.Log(_player.transform.Find("A_Character_03").transform.Find("Rig_Player").transform.Find("Mch_snap_Wood"));
        //Debug.Log(_player.transform.Find("A_Character_03").transform.Find("Rig_Player").transform.Find("Mch_snap_Wood").transform.Find("Wood_property"));
        //Debug.Log(_player.transform.Find("A_Character_03").transform.Find("Rig_Player").transform.Find("Mch_snap_Wood").transform.Find("Wood_property").transform.Find("SM_Plank_02"));
        _plank = _player.transform.Find("A_Character_03").transform.Find("Rig_Player").transform.Find("Mch_snap_Wood").transform.Find("Wood_property").transform.Find("PlankFeedback").gameObject;
        _plank.SetActive(false);
        _animator = _plank.GetComponent<Animator>();
        //Debug.Log("Diese Planke habe ich gerade deaktiviert zum Start: " + _plank);
        //Debug.Log(_player);
        //Debug.Log(_player.transform.Find("SM_Plank_02"));
        _soundmanager = GameObject.Find("SoundManager").GetComponentInChildren<s_SoundManager>();
    }

    //public bool IsHavingPlank()
    //{
    //    return (_isHavingPlank);
    //}

    //public int AmountOfStone()
    //{
    //    return (_amountOfKeptStones);
    //}

    public bool TryToUsePlank() 
    {
        if (_isHavingPlank)
        {
            _isHavingPlank = false;
            //_uiPlank.SetActive(false);
            _plank.SetActive(false);
            return true;
        }
        else
        {
            return false;
        }
    }

    //public bool TryToDropPlank(Vector3 positionToDropPlank)
    //{
    //    if (_isHavingPlank)
    //    {
    //        _plankInInventory.ReturnPlankToScene();
    //        _plankInInventory.transform.position = positionToDropPlank;
    //        _isHavingPlank = false;
    //        _uiPlank.SetActive(false);
    //        _plankInInventory = null;
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    //public bool TryToDropStone(Vector3 positionToDropStone)
    //{
    //    Stone StoneToDrop = TryToConsumeStone();
    //    if (StoneToDrop != null)
    //    {
    //        StoneToDrop.transform.position = positionToDropStone;
    //        Debug.Log("stone was dropped to " + positionToDropStone);
    //        StoneToDrop.ReturnStoneToScene();
    //        Debug.Log("stone which was dropped" + StoneToDrop);
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    //public bool TryToDropItem(Vector3 positionToDropItem)
    //{
    //    if (_isHavingPlank)
    //    {
    //        _plankInInventory.ReturnPlankToScene();
    //        _plankInInventory.transform.position = positionToDropItem;
    //        _isHavingPlank = false;
    //        _uiPlank.SetActive(false);
    //        _plankInInventory = null;
    //        return true;
    //    }
    //    Stone StoneToDrop = TryToConsumeStone();
    //    if (StoneToDrop != null)
    //    {
    //        StoneToDrop.transform.position = positionToDropItem;
    //        Debug.Log("stone was dropped to " + positionToDropItem);
    //        StoneToDrop.ReturnStoneToScene();
    //        Debug.Log("stone which was dropped" + StoneToDrop);
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    public bool TryToThrowStone()
    {
        Vector3 positionToThrowStone = _player.transform.Find("DropPosition").transform.position; // positionToThrowStone = gameObject.Find
        Debug.Log("trying to throw stone to " + positionToThrowStone);
        Stone StoneToThrow = TryToConsumeStone();
        Debug.Log("stone to drop" + StoneToThrow);
        if (StoneToThrow != null)
        {
            StoneToThrow.transform.position = positionToThrowStone + new Vector3(1, 0, 0); ;
            StoneToThrow.GetComponent<Rigidbody>().velocity = new Vector3(10, 10, 10);
            Debug.Log("stone was thrown to " + positionToThrowStone);
            StoneToThrow.ReturnStoneToScene();
            Debug.Log("stone which was thrown" + StoneToThrow);
            return true;
        }
        else
        {
            return false;
        }
    }

    public Stone TryToConsumeStone()
    {
        if (_amountOfKeptStones == 2)
        {
            _amountOfKeptStones -= 1;
            _uiStone2.SetActive(false);
            return _StoneInInventory2;
        }
        if (_amountOfKeptStones == 1)
        {
            _amountOfKeptStones -= 1;
            _uiStone1.SetActive(false);
            return _StoneInInventory1;
        }
        return null;
    }

    public bool TryToGatherPlank(Plank plankToPickUp)
    {
        if (!_isHavingPlank && _amountOfKeptStones == 0)
        {
            _plankInInventory = plankToPickUp;
            //_plank.GetComponent<Animator>().SetTrigger("IsFeedback");
            
            _isHavingPlank = true;
            //_uiPlank.SetActive(true);
            _soundmanager.PlaySound2D("event:/SFX/PickUpPlank");
            _player.GetComponent<PlayerControl>().PlayInteractAnimation();
            _plank.SetActive(true);
            _animator.SetTrigger("IsFeedback");
            if (!_gatheredFirstPlank)
            {
                _playerCollider.GatheredPlank();
                _gatheredFirstPlank = true;
            }
            //Debug.Log("Dese Planke habe ich gerade aufgehoben: " + _plank);
            return true;
        }
        else
        {
            _soundmanager.PlaySound2D("event:/SFX/InventoryFull");
            return false;
        }  
    }

    public bool TryToGatherStone(Stone stoneToPickUp)
    {
        if (!_isHavingPlank && _amountOfKeptStones == 0)
        {
            //Debug.Log("Fackel angezündet");
            _StoneInInventory1 = stoneToPickUp;
            _amountOfKeptStones += 1;
            _uiStone1.SetActive(true);
            _soundmanager.PlaySound2D("event:/SFX/PickUpStone");
            return true;
        }
        if (!_isHavingPlank && _amountOfKeptStones == 1)
        {
            //Debug.Log("Fackel angezündet");
            _StoneInInventory2 = stoneToPickUp;
            _amountOfKeptStones += 1;
            _uiStone2.SetActive(true);
            //_soundmanager.SFXBusSetVolume(1.0f);
            //_soundmanager.MasterBusSetVolume(1.0f);
            _soundmanager.PlaySound2D("event:/SFX/PickUpStone");
            return true;
        }
        _soundmanager.PlaySound2D("event:/SFX/InventoryFull");
        return false;
    }
}