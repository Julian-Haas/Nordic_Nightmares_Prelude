using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Guidance : MonoBehaviour
{
    [SerializeField] GameObject _guidanceTooltip;
    [SerializeField] Text _guidanceTooltipText;
    int _guidanceTooltipDuration = 5;
    Coroutine GuidanceTooltipCoroutine;
    bool _TooltipCoroutineRunning;

    void Start()
    {
        _guidanceTooltip.SetActive(false);
        displayGuidanceTooltipWithSpecificText("I need to find a way back home.");
    }

    public void displayGuidanceTooltipWithSpecificText(string guidanceTooltipText)
    {
        if (_TooltipCoroutineRunning)
        {
            StopCoroutine(GuidanceTooltipCoroutine);
        }
        _guidanceTooltipText.text = guidanceTooltipText;
        _guidanceTooltip.SetActive(true);
        GuidanceTooltipCoroutine = StartCoroutine(DisplayTooltipForSeconds(_guidanceTooltipDuration));
    }

    IEnumerator DisplayTooltipForSeconds(int secondsToDisplayTooltip)
    {
        yield return new WaitForSeconds(secondsToDisplayTooltip);
        _guidanceTooltip.SetActive(false);
    }
}
