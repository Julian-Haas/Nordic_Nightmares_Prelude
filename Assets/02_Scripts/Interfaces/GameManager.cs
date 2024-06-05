using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool _playedLoadingScreens = false;
    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
        DontDestroyOnLoad(this);
        Application.targetFrameRate = 100;
    }
    void OnSceneLoaded(Scene scene,LoadSceneMode mode) {
        if(SceneManager.GetActiveScene().buildIndex == 1) {
            _playedLoadingScreens = true;
        }
    }
}
