using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_Shrine_Interactable : Interactable
{
    public int _shrineChargesLeft = 3;
    //public List<GameObject> _ShrineCharges;
    //public GameObject CompleteSafeSpace;
    public GameObject _shrineCharge1, _shrineCharge2, _shrineCharge3, _shrineFireBowl;
    public bool _shrineIsRechargable = true;
    public bool _rechargeFromFirstUseOn = false;
    public int _rechargeTime = 7;

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

    public override bool Interact(bool started)
    {
        EnterShrine();
        if(_shrineChargesLeft > 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void EnterShrine()
    {
        if (_shrineChargesLeft == 1)
        {
            _shrineFireBowl.SetActive(true);
            _shrineCharge1.SetActive(false);
            _shrineChargesLeft--;
            if (_shrineIsRechargable && _rechargeFromFirstUseOn)
            {
                RechargeShrine();
            }
            return;
        }
        if (_shrineChargesLeft == 2)
        {
            _shrineFireBowl.SetActive(true);
            _shrineCharge2.SetActive(false);
            _shrineChargesLeft--;
            return;
        }
        if (_shrineChargesLeft == 3)
        {
            _shrineFireBowl.SetActive(true);
            _shrineCharge3.SetActive(false);
            _shrineChargesLeft--;
            if (_shrineIsRechargable && !_rechargeFromFirstUseOn)
            {
                RechargeShrine();
            }
            return;
        }
    }

    private void RechargeShrine()
    {
        StartCoroutine(ReloadShrine());
        Debug.Log("start recharging");

    }
    IEnumerator ReloadShrine()
    {
        yield return new WaitForSeconds(_rechargeTime);
        _shrineChargesLeft += 1;
        Debug.Log("recharge ended. charges now: " + _shrineChargesLeft);
        if (_shrineChargesLeft == 1)
        {
            _shrineCharge1.SetActive(true);
            RechargeShrine();
        }
        if (_shrineChargesLeft == 2)
        {
            _shrineCharge2.SetActive(true);
            RechargeShrine();
        }
        if (_shrineChargesLeft == 3)
        {
            _shrineCharge3.SetActive(true);
        }
    }
}
