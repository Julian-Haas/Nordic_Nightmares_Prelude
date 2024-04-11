using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScriptedCharacter : MonoBehaviour
{

    [SerializeField] private List<ScriptedCharacter> _scriptedCharacterList;

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            foreach(ScriptedCharacter scriptedCharacter in _scriptedCharacterList) {
                scriptedCharacter.StartScriptedEvents();
            }
            gameObject.SetActive(false);
        }

    }
}
