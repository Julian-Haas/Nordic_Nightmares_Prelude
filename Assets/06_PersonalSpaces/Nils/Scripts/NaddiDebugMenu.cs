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
    private Slider NaddiHearingSB; 
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
        OnNaddiSoundSumChanged();

    }

    public void OnDropdownValueChanged(int index)
    {
        // Hole den ausgew?hlten Wert aus dem Dropdown-Men?
        _naddi.State = (NaddiStateEnum)NaddiStateEnum.Parse(typeof(NaddiStateEnum), NaddiStatesDD.options[index].text);
    }

    public void OnNaddiSoundSumChanged() 
    {
        NaddiHearingSB.value = _hearing.GetSoundSum; 
    }
}
