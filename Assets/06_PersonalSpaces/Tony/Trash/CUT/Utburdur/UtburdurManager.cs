using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtburdurManager : MonoBehaviour
{
    private List<UtburdurLandingSpot> _landingSpots = new List<UtburdurLandingSpot>();
    private List<Utburdur_Sitting> _utburdurSitting = new List<Utburdur_Sitting>();

    public void AddSittingUtburdur(Utburdur_Sitting utSit)
    {
        _utburdurSitting.Add(utSit);
    }

    public void RemoveSittingUtburdur(Utburdur_Sitting utSit)
    {
        _utburdurSitting.Remove(utSit);
    }

    public void AddLandingSpot(UtburdurLandingSpot spot)
    {
        _landingSpots.Add(spot);
    }

    public UtburdurLandingSpot SwitchLandingSpot()
    {
        int ind = Random.Range(0, _landingSpots.Count);
        UtburdurLandingSpot spot = _landingSpots[ind];
        _landingSpots.RemoveAt(ind);
        return spot;
    }

    public void CheckIfUtburdurCanHearNoise(NoiseEmitter emitter) 
    {
        foreach (var ut in _utburdurSitting)
        {
            ut.CheckIfUtburdurCanHearNoise(emitter);
        }
    }
}