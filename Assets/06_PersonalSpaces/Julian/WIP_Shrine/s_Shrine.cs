using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_Shrine : MonoBehaviour
{
    public int _shrineChargesLeft = 3;
    //public List<GameObject> _ShrineCharges;
    //public GameObject CompleteSafeSpace;
    public GameObject _shrineCharge1, _shrineCharge2, _shrineCharge3, _shrineFireBowl;
    public bool _shrineIsRechargable = true;
    public bool _rechargeFromFirstUseOn = false;
    public int _rechargeTime = 7;
    private int _index = 3;

    private void Start()
    {
        _shrineFireBowl.SetActive(false);
    }

    public void LeaveShrine()
    {
        _shrineFireBowl.SetActive(false);
        //Debug.Log("triggered funktion of leave safespace");
        //ShrineChargesLeft--;
        //if (ShrineChargesLeft <= 0)
        //{
        //    Destroy(CompleteSafeSpace);
        //}
        //_charges[ShrineChargesLeft].SetActive(false);
        
    }

    public void EnterShrine()
    {
        switch (_shrineChargesLeft)
        {
            case 0:
                GameObject.Find("PlayerAnimated").GetComponentInChildren<Guidance>().displayGuidanceTooltipWithSpecificText("The magic of this place needs time to recharge.");
                return;
            case 1:
                WorldStateData.Instance.UpdateInteractableState(_index, 0);
                _shrineFireBowl.SetActive(true);
                _shrineCharge1.SetActive(false);
                _shrineChargesLeft--;
                return;
            case 2:
                WorldStateData.Instance.UpdateInteractableState(_index, 1);
                _shrineFireBowl.SetActive(true);
                _shrineCharge2.SetActive(false);
                _shrineChargesLeft--;
                return;
            case 3:
                WorldStateData.Instance.UpdateInteractableState(_index, 2);
                _shrineFireBowl.SetActive(true);
                _shrineCharge3.SetActive(false);
                _shrineChargesLeft--;
                if (_shrineIsRechargable && _rechargeFromFirstUseOn)
                {
                    RechargeShrine();
                }
                return;
        }
    }

    private void RechargeShrine()
    {
        StartCoroutine(ReloadShrine());
        //Debug.Log("start recharging");
    }
    IEnumerator ReloadShrine()
    {
        yield return new WaitForSeconds(_rechargeTime);
        _shrineChargesLeft += 1;
        //Debug.Log("recharge ended. charges now: " + _shrineChargesLeft);
        if (_shrineChargesLeft == 1)
        {
            WorldStateData.Instance.UpdateInteractableState(_index, 2);
            _shrineCharge1.SetActive(true);
            RechargeShrine();
        }
        if (_shrineChargesLeft == 2)
        {
            WorldStateData.Instance.UpdateInteractableState(_index, 2);
            _shrineCharge2.SetActive(true);
            RechargeShrine();
        }
        if (_shrineChargesLeft == 3)
        {
            WorldStateData.Instance.UpdateInteractableState(_index, 3);
            _shrineCharge3.SetActive(true);
        }
    }
}
