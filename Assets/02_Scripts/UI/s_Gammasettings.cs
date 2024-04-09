using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.UI;



public class s_Gammasettings : MonoBehaviour
{
    [SerializeField] private Slider _gammaSlider;
    [SerializeField] private TextMeshProUGUI _gammaText;
    [SerializeField] private Volume _volume;
    private ColorAdjustments _colorAdjustments;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            //LiftGammaGainController.SetGammaAlpha(2.0f);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            //SetGammaAlpha();
        }
    }

    public void AdjustGammaSettings()
    {
        _volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments);
        if (_colorAdjustments == null)
            Debug.LogError("No ColorAdjustments found on profile");
        else
        {
            _colorAdjustments.postExposure.value = 0.3f + _gammaSlider.value;
        }
        _gammaText.text = (100.0f * _gammaSlider.value).ToString("0");
    }
}

public class LiftGammaGainController : MonoBehaviour
{

    public static LiftGammaGainController instance;

    public Volume renderingVolume;
    LiftGammaGain liftGammaGain;

    private void Awake()
    {
        instance = this;
        renderingVolume = GetComponent<Volume>();
        if (!renderingVolume.profile.TryGet(out liftGammaGain)) throw new System.NullReferenceException(nameof(liftGammaGain));
    }

    public void SetGammaAlpha(float gammaAlpha)
    {
        liftGammaGain.gamma.Override(new Vector4(1f, 1f, 1f, gammaAlpha));
    }

    public float GetGammaAlpha() { return liftGammaGain.gamma.value.w; }

}