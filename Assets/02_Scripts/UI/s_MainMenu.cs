using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using System.Net.Http;
using System.Threading.Tasks;
using System;
//using UnityEngine.UIElements;

public class s_MainMenu : MonoBehaviour
{
    public Image _background;
    public Image _gameLogo;
    public Image _teamLogo;
    public Image _gaLogo;
    public GameObject _screenSettings;
    public GameObject _screenCredits;
    public GameObject _screenLore;

    private s_gameManager _gameManager;
    private s_SoundManager _soundManager;


    private void Awake()
    {

        _soundManager = GameObject.Find("SoundManager").GetComponent<s_SoundManager>();
    }

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<s_gameManager>();
        //Debug.Log(GameObject.Find("GameManager").name);

        //Debug.Log(GameObject.Find("GameManager").GetComponent<s_gameManager>().name);
        //Debug.Log(GameObject.Find("GameManager").GetComponentInChildren<s_gameManager>().name);
        
        _screenSettings.SetActive(false);
        _screenCredits.SetActive(false);
        _screenLore.SetActive(false);
        _teamLogo.enabled = false;
        _gaLogo.enabled = false;
        //Debug.Log(_gaLogo);
        //Debug.Log(_gaLogo.GetComponent<Image>());
        _gameLogo.enabled = false;
        _background.enabled = false;

        if (!_gameManager._playedLoadingScreens)
        {
            StartCoroutine(LoadingScreensCoroutine());
        }

        //_soundManager.MusicBusSetVolume(0.0f);
        //_soundManager.MasterBusSetVolume(0.0f);
        //_soundManager.SFXBusSetVolume(0.0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            _soundManager.PlaySound2D("event:/Dummy/Test2DEvent");
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            _soundManager.SFXBusSetVolume(_soundManager._sfxVolume += 0.1f);
        }

    }

    IEnumerator LoadingScreensCoroutine()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _background.enabled = true;
        StartCoroutine(AnimateImage(_teamLogo));
        yield return new WaitForSeconds(3.5f);
        StartCoroutine(AnimateImage(_gameLogo));
        
        yield return new WaitForSeconds(3.5f);
        _soundManager.musicInstance.SetParameter("Level", 0.3f);
        //yield return new WaitForSeconds(2f);
        _background.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
    }

    //IEnumerator FadeImageOut(Image image)
    //{
    //    for (float i = 1; i >= 0; i -= Time.deltaTime)
    //    {
    //        image.color = new Color(1, 1, 1, i);
    //        yield return null;
    //    }
    //}
    //IEnumerator FadeImageIn(Image image)
    //{
    //    for (float i = 0; i <= 1; i += Time.deltaTime)
    //    {
    //        image.color = new Color(1, 1, 1, i);
    //        yield return null;
    //    }
    //}
    IEnumerator AnimateImage(Image image)
    {
        image.enabled = true;
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            image.color = new Color(1, 1, 1, i);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            image.color = new Color(1, 1, 1, i);
            yield return null;
        }
        image.enabled = false;
    }

    //async void AnimateImageAsync(Image image)
    //{
    //    image.enabled = true;
    //    for (float i = 0; i <= 1; i += Time.deltaTime)
    //    {
    //        image.color = new Color(1, 1, 1, i);
    //    }
    //    for (float i = 0; i <= 1; i += Time.deltaTime)
    //    {
    //    }
    //    for (float i = 1; i >= 0; i -= Time.deltaTime)
    //    {
    //        image.color = new Color(1, 1, 1, i);
    //    }
    //    image.enabled = false;
    //}

    IEnumerator temp()
    {
        yield return new WaitForSeconds(1.0f);
    }

    public void PlayGame ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void Tutorial()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void openCredits()
    {
        _soundManager.musicInstance.SetParameter("Level", 10f);
    }

    public void closeCredits()
    {
        _soundManager.musicInstance.SetParameter("Level", 0.5f);
    }



}


//private void Awake()
//{
//    _teamLogo.enabled = false;
//    _gaLogo.enabled = false;
//    StartCoroutine(LoadingScreensCoroutine());
//    _gameLogo.enabled = false;
//    _teamLogo.enabled = true;
//    StartCoroutine(LoadingScreensCoroutine());
//    _teamLogo.enabled = false;
//    _gaLogo.enabled = true;
//    StartCoroutine(LoadingScreensCoroutine());
//    _gaLogo.enabled = false;
//    StartCoroutine(LoadingScreensCoroutine());
//    _background.enabled = false;
//}

//IEnumerator LoadingScreensCoroutine()
//{

//    yield return new WaitForSeconds(3.0f);
//}