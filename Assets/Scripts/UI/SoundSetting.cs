using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] private GameObject soundSettingViewport;
    [SerializeField] private Button soundSettingButton;

    [SerializeField] private Slider MasterSlider;           // 마스터 볼륨 슬라이더
    [SerializeField] private TextMeshProUGUI masterTmp;     // 마스터 볼륨 텍스트

    [SerializeField] private Slider BGMSlider;              // BGM 볼륨 슬라이더
    [SerializeField] private TextMeshProUGUI bgmTmp;        // BGM 볼륨 텍스트

    [SerializeField] private Slider SFXSlider;              // SFX 볼륨 슬라이더
    [SerializeField] private TextMeshProUGUI sfxTmp;        // SFX 볼륨 텍스트

    [SerializeField] private Slider VoiceSlider;            // Voice 볼륨 슬라이더
    [SerializeField] private TextMeshProUGUI voiceTmp;      // Voice 볼륨 텍스트

    private AudioMixerController audioMixerController;

    public void Init()
    {
        soundSettingButton.onClick.AddListener(() => soundSettingViewport.SetActive(true));

        audioMixerController = AudioMixerController.Instance;

        MasterSlider.onValueChanged.AddListener(value => SetValueText(masterTmp, value));
        MasterSlider.onValueChanged.AddListener(value => audioMixerController.SetMasterVolume(value));

        BGMSlider.onValueChanged.AddListener(value => SetValueText(bgmTmp, value));
        BGMSlider.onValueChanged.AddListener(value => audioMixerController.SetMusicVolume(value));

        SFXSlider.onValueChanged.AddListener(value => SetValueText(sfxTmp, value));
        SFXSlider.onValueChanged.AddListener(value => audioMixerController.SetSFXVolume(value));

        VoiceSlider.onValueChanged.AddListener(value => SetValueText(voiceTmp, value));
        VoiceSlider.onValueChanged.AddListener(value => audioMixerController.SetVoiceVolume(value));

        var loadData = audioMixerController.Load();

        MasterSlider.value = loadData.masterVolumeValue;
        BGMSlider.value = loadData.bgmVolumeValue;
        SFXSlider.value = loadData.sfxVolumeValue;
        VoiceSlider.value = loadData.voiceVolumeValue;

        SetValueText(masterTmp, loadData.masterVolumeValue);
        SetValueText(bgmTmp, loadData.bgmVolumeValue);
        SetValueText(sfxTmp, loadData.sfxVolumeValue);
        SetValueText(voiceTmp, loadData.voiceVolumeValue);
    }
    
    private void SetValueText(TextMeshProUGUI tmp, float value)
    {
        float newValue = value * 100f;
        string text = newValue.ToString("F0");  // 소수점 표시 X
        tmp.text = text;
    }

    public void SetActiveViewport(bool set) { soundSettingViewport.SetActive(set); }
    public void AddSoundSettingButtonOnClick(UnityAction method)
    {
        soundSettingButton.onClick.AddListener(method);
    }
}
