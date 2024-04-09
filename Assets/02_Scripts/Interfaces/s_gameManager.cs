using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_gameManager : MonoBehaviour
{
    //private bool _loadingScreensPlayed { get; set; } = false;
    s_gameManager _gameManagerInstance;
    public bool _playedLoadingScreens = false;
    public bool _isMusicPlaying = false;
    public bool _isAmbientPlaying = false;

    private void Awake()
    {
        if (_gameManagerInstance == null)
        {
            _gameManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 100;
    }
}
