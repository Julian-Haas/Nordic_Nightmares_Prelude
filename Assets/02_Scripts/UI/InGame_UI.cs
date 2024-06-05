using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGame_UI : MonoBehaviour
{
    private bool _isPaused = false;
    public GameObject Overlay, ContinueButton;
    public Text OverlayText;
    private Slider _awareness, _sanity;
    private s_SoundManager _soundManager;
    [SerializeField] public GameObject _settingsScreen;
    [SerializeField] public GameObject _ControlsScreen;
    [SerializeField] public GameObject _IngameMenu;
    [SerializeField] Animation _animation;
    [SerializeField] Image _fadeIn, _pauseOverlay;
    [SerializeField] GameObject _victoryScreen;
    private void Awake() {
        _soundManager = GameObject.Find("SoundManager").GetComponentInChildren<s_SoundManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        _awareness = transform.Find("NaddiAwareness")?.Find("AwarenessSlider")?.GetComponent<Slider>();
        _sanity = transform.Find("Sanity")?.Find("SanitySlider")?.GetComponent<Slider>();
    }

    private void Start() {
        //_naddi = GameObject.Find("Naddi").GetComponent<NaddiAwareness>();
        OverlayText.text = "PAUSE";
    }

    //bool 

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {

            if(_settingsScreen.activeInHierarchy) {
                CloseSettings();
            }
            else if(_ControlsScreen.activeInHierarchy) {
                CloseControls();
            }
            else if(!_IngameMenu.activeInHierarchy) {
                Pause();
                //Debug.Log("Test1");
            }
        }


        AwarenessUpdate();
        //_sanity.value = GlobalData.Instance.GetSanity();
    }

    private void AwarenessUpdate() {
        /*
        float awareness = _naddi.GetAwareness();
        if (awareness < _suspicious)
        {
            _awareness.image.color = Color.blue;
        }
        else if (awareness >= _suspicious && _awareness.value < _aware)
        {
            _awareness.image.color = Color.yellow;
        }
        else if (awareness >= _aware && _awareness.value < _alert)
        {
            _awareness.image.color = Color.magenta;
        }
        else if (awareness >= _alert && _awareness.value < _hunting)
        {
            _awareness.image.color = Color.red;
        }
        else if (awareness >= _hunting)
        {
            _awareness.image.color = Color.white;
        }
         */
    }

    private void SanityUpdate() {
        //_sanity.value = GlobalData.Instance.GetSanity();

    }

    void Pause() {
        if(!_isPaused) {
            //Debug.Log("Test2");
            OverlayText.text = "PAUSE";
            _fadeIn.color = new Color(0.0f,0.0f,0.0f,0.0f);
            _pauseOverlay.color = new Color(0.0f,0.01f,0.9f,0.5f);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //Debug.Log("Test3");
            //Debug.Log("Escape key was pressed");
            ContinueButton.SetActive(true);
            _IngameMenu.SetActive(true);
            //OverlayText.text = "PAUSE";
            Time.timeScale = 0;
            //Debug.Log("Test4");
            _isPaused = true;
        }
        else if(_isPaused) {
            OverlayText.text = "PAUSE";
            //Debug.Log("angekommen 1");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //Debug.Log("Escape key was pressed");
            _IngameMenu.SetActive(false);
            Time.timeScale = 1;
            _isPaused = false;
        }
    }

    public void GameOver() {
        //StartCoroutine(FadeInCoroutine());
        //    Debug.Log("You died!");
        OverlayText.text = "GAME OVER";
        Overlay.SetActive(true);
        //   _animation.Play();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ContinueButton.SetActive(false);
        Time.timeScale = 0;
    }

    public void Win() {
        //Debug.Log("Congrats!");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _victoryScreen.SetActive(true);

        //ContinueButton.SetActive(false);
        //Overlay.SetActive(true);
        //OverlayText.text = "VICTORY!";
        Time.timeScale = 0;
    }

    public void BackToMainMenu() {
        _soundManager.musicInstance.SetParameter("Sanity",1.0f);
        _soundManager.musicInstance.SetParameter("Level",0.5f);
        _soundManager.musicInstance.SetParameter("NaddiR",1.0f);
        _soundManager.musicInstance.SetParameter("NaddiHunt",0.0f);


        SceneManager.LoadScene(0);
    }

    public void Replay() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //GlobalData.Instance.ResetValues();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenSettings() {
        _settingsScreen.SetActive(true);
        _IngameMenu.SetActive(false);
    }

    public void OpenControls() {
        _ControlsScreen.SetActive(true);
        _IngameMenu.SetActive(false);
    }

    public void CloseSettings() {
        _settingsScreen.SetActive(false);
        _IngameMenu.SetActive(true);
    }

    public void CloseControls() {
        _ControlsScreen.SetActive(false);
        _IngameMenu.SetActive(true);
    }

    public void Continue() {
        //Debug.Log("versuche spiel fortzusetzen");
        Pause();
    }

    public void QuitGame() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Application.Quit();
    }

    private IEnumerator FadeInCoroutine() {

        Debug.Log("You died!");
        OverlayText.text = "GAME OVER";
        ContinueButton.SetActive(false);
        Overlay.SetActive(true);
        _animation.Play();
        yield return new WaitForSeconds(1);
        _fadeIn.color = new Color(0.0f,0.0f,0.0f,0.0f);
        _pauseOverlay.color = new Color(0.0f,0.01f,0.9f,0.5f);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}