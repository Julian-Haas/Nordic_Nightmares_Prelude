using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class S_Shrine : MonoBehaviour
{
    [SerializeField] Animator _shrineAnimator;
    [SerializeField] Animator _shrineBubbleAnimator;
    private SoundManager _soundManager;

    public int _shrineChargesLeft = 3;
    public int _rechargeTime = 7;
    //private int _index = 3;


    public void Start() {
        _soundManager = GameObject.Find("SoundManager").GetComponentInChildren<SoundManager>();
    }
    public void EnterShrine() {

        _soundManager.RegisterEventEmitter(this.gameObject,"event:/SFX/SafespaceActive");
        _soundManager.PlaySound3D("event:/SFX/ActivateSafespace",this.transform.position);
        //event:/SFX/SafespaceActive
        Debug.Log("entered collider of shrine 2");
        switch(_shrineChargesLeft) {
            case 0:
                GameObject.Find("PlayerAnimated").GetComponentInChildren<Guidance>().displayGuidanceTooltipWithSpecificText("The magic of this place needs time to recharge.");
                return;
            case 1:
                _shrineBubbleAnimator.SetBool("IsActive",true);
                RechargeShrine();
                _shrineChargesLeft = 0;
                _shrineAnimator.SetFloat("HouseAmountSZ",0);
                return;
            case 2:
                _shrineBubbleAnimator.SetBool("IsActive",true);
                _shrineChargesLeft = 1;
                _shrineAnimator.SetFloat("HouseAmountSZ",1);
                return;
            case 3:
                _shrineBubbleAnimator.SetBool("IsActive",true);
                _shrineChargesLeft = 2;
                _shrineAnimator.SetFloat("HouseAmountSZ",2);
                return;
        }
    }

    private void RechargeShrine() {
        StartCoroutine(ReloadShrine());
    }

    IEnumerator ReloadShrine() {
        yield return new WaitForSeconds(_rechargeTime);
        _shrineChargesLeft += 1;
        _shrineAnimator.SetFloat("HouseAmountSZ",_shrineAnimator.GetFloat("HouseAmountSZ") + 1.0f);
        if(_shrineChargesLeft == 1 || _shrineChargesLeft == 2) {
            RechargeShrine();
        }
    }

    public void LeaveShrine() {
        _soundManager.UnregisterEventEmitter(this.gameObject,"event:/SFX/SafespaceActive");
        _shrineBubbleAnimator.SetBool("IsActive",false);
    }
}