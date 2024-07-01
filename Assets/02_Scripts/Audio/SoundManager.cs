using FMOD.Studio;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] public float _naddiMusicMultiplier = 0.02f;
    private Dictionary<GameObject,EventEmitterObject> eventEmitter = new(); // for organizing 3D Sounds
    public ParentEventInstance ambientInstance; // 2D permanent Sound, always playing when set
    public ParentEventInstance musicInstance; // 2D permanent Sound, always playing when set
    private Bus MusicBus, SFXBus, SFXNoUIBus, MasterBus;
    public float _masterVolume = 100.0f;
    public float _sfxVolume = 100.0f;
    public float _musicVolume = 100.0f;
    private static SoundManager _SoundManagerInstance;
    private float pauseVolumeReduction = 0.6f;
    bool _isMusicPlaying = false;
    bool _isAmbientPlaying = false;
    private struct EventEmitterObject // Data Type for 3D Sound
    {
        public GameObject GameObject; // GameObject to which the Sound is bound
        public Transform Transform; // passing the transform saves performance
        //public Rigidbody Rigidbody; // every 3D sound will "need" this, but it can be set later manually
        public Dictionary<string,EventInstance> EventInstances; // there can be several dictionaries which manage different things
    }
    private void Awake() {
        if(Instance != null && Instance != this) {
            Debug.Log("test");
            Destroy(gameObject);
        }
        else {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        MusicBus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        //MusicBusSetVolume(0.1f);
        SFXBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        //SFXBusSetVolume(1.0f);
        SFXNoUIBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX/SFXNoUI");
        //MasterBusSetVolume(1.0f);
        MasterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
    }
    public void RegisterEventEmitter(GameObject krachmacher,string path) // Start 3D Sound
    {
        if(!eventEmitter.ContainsKey(krachmacher)) {
            EventEmitterObject emitter = new() {
                GameObject = krachmacher,
                Transform = krachmacher.transform,
                //Rigidbody = GameObject.Rigidbody,
                EventInstances = new Dictionary<string,EventInstance>(),
            };
            eventEmitter.Add(krachmacher,emitter);
        }
        if(!eventEmitter[krachmacher].EventInstances.ContainsKey(path)) {
            EventInstance eventEmitterInstance = FMODUnity.RuntimeManager.CreateInstance(path);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(eventEmitterInstance,eventEmitter[krachmacher].Transform,eventEmitter[krachmacher].GameObject.GetComponent<Rigidbody>());
            eventEmitterInstance.start();
            eventEmitter[krachmacher].EventInstances.Add(path,eventEmitterInstance);
        }
    }
    public void UnregisterEventEmitter(GameObject krachmacher,string path) // End 3D Sound
    {
        eventEmitter[krachmacher].EventInstances[path].release();
        eventEmitter[krachmacher].EventInstances[path].stop(STOP_MODE.ALLOWFADEOUT);
        eventEmitter[krachmacher].EventInstances.Remove(path);
    }
    public void SetParameterToEventEmitter(GameObject krachmacher,string path,string parameterName,float parameterValue) // Change Parameter of Sound
    {
        eventEmitter[krachmacher].EventInstances[path].setParameterByName(parameterName,parameterValue);
    }
    public void PlaySound3D(string path,Vector3 position) // 3D - Sound (one shot)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path,position);
    }
    // 1. Eventname: Footstep
    // 2. Object das den sound abspielt: player
    // 3. Parameternamen: Material
    // 4. Value 0.0-0.2
    public void PlaySound3DwithParameter(string path,GameObject gameObject,string parameterName,float parameterValue) // 3D - Sound (one shot) with parameter
    {
        EventInstance eventInstance = FMODUnity.RuntimeManager.CreateInstance(path);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(eventInstance,gameObject.transform,gameObject.GetComponent<Rigidbody>());
        eventInstance.setParameterByName(parameterName,parameterValue);
        eventInstance.start();
        eventInstance.release();
    }
    public void PlaySound2D(string path) // UI Sound / 2D - Sound (one shot)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path);
    }
    public class ParentEventInstance
    {
        public EventInstance EventInstance {
            get;
        }
        public ParentEventInstance(string name,ParentEventInstance myEventInstance) {
            if(myEventInstance != null) {
                myEventInstance.Stop();
            }
            EventInstance = FMODUnity.RuntimeManager.CreateInstance(name);
            EventInstance.start();
        }
        public void Stop() {
            if(IsPlaying()) {
                EventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
        public bool IsPlaying() {
            EventInstance.getPlaybackState(out PLAYBACK_STATE state);
            return state == PLAYBACK_STATE.PLAYING;
        }
        public void SetParameter(string parameter,float parametervalue) {
            EventInstance.setParameterByName(parameter,parametervalue);
        }
    }
    public void MusicBusSetVolume(float volume) {
        _musicVolume = volume;
        MusicBus.setVolume(volume);
    }
    public void SFXBusSetVolume(float volume) {
        _sfxVolume = volume;
        SFXBus.setVolume(volume);
    }
    public void MasterBusSetVolume(float volume) {
        _masterVolume = volume;
        MasterBus.setVolume(volume);
    }
    public void Pause() {
        SFXNoUIBus.setPaused(true);
        MusicBus.setVolume(_musicVolume * pauseVolumeReduction);
    }
    public void Resume() {
        SFXNoUIBus.setPaused(false);
        MusicBus.setVolume(_musicVolume);
    }
    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene,LoadSceneMode mode) {
        if(!_isMusicPlaying) {
            musicInstance = new("event:/Music/MusicMain",musicInstance); // the Instance is permanent and when set will always play
            _isMusicPlaying = true;
        }
        if(SceneManager.GetActiveScene().buildIndex == 1) {
            musicInstance.SetParameter("Level",1f);
        }
        if(SceneManager.GetActiveScene().buildIndex != 0 && !_isAmbientPlaying) {
            ambientInstance = new("event:/SFX/AmbientMain",ambientInstance); // the Instance is permanent and when set will always play
            _isAmbientPlaying = true;
        }
        else {
            if(ambientInstance != null) {
                ambientInstance.Stop();
                _isAmbientPlaying = false;
            }
        }
    }
}