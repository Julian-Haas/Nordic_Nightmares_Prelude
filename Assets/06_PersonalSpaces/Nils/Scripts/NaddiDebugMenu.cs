using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
public class NaddiDebugMenu : MonoBehaviour
{
    [SerializeField]
    private NaddiHearing _hearing; 
    [SerializeField]
    private TMP_Dropdown NaddiStatesDD;
    [SerializeField]
    private Scrollbar NaddiHearingSB; 
    [SerializeField]
    private Naddi _naddi; 
    

    // Start is called before the first frame update
    void Start()
    {
        NaddiStatesDD.AddOptions(new List<string>(NaddiStateEnum.GetNames(typeof(NaddiStateEnum)))); 
    }

    // Update is called once per frame
    void Update()
    {
        OnNaddiSoundSumChanged(); 
    }

    public void OnDropdownValueChanged(int index)
    {
        // Hole den ausgewählten Wert aus dem Dropdown-Menü
        _naddi.State = (NaddiStateEnum)NaddiStateEnum.Parse(typeof(NaddiStateEnum), NaddiStatesDD.options[index].text);
    }

    public void OnNaddiSoundSumChanged() 
    {
        NaddiHearingSB.value = _hearing.GetSoundSum; 
    }
}
