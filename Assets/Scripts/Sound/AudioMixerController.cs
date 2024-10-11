using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// 오디오 그룹 타입
public enum AudioType
{
    Master,
    BGM,
    SFX,
    Voice,
}

public class AudioMixerController : Singleton<AudioMixerController>
{
    [SerializeField] private AudioMixer mixer;      // 오디오 믹서
    [SerializeField] private Slider MasterSlider;   // 마스터 볼륨 슬라이더
    [SerializeField] private Slider BGMSlider;      // BGM 볼륨 슬라이더
    [SerializeField] private Slider SFXSlider;      // SFX 볼륨 슬라이더
    [SerializeField] private Slider VoiceSlider;    // Voice 볼륨 슬라이더

    public string MasterGroupName;                  // 마스터 볼륨 그룹 이름
    public string BGMGroupName;                     // BGM 볼륨 그룹 이름
    public string SFXGroupName;                     // SFX 볼륨 그룹 이름
    public string VoiceGroupName;                   // Voice 볼륨 그룹 이름

    protected override void Awake()
    {
        base.Awake();
    }

    // 오디오 믹서 그룹 가져오기
    public AudioMixerGroup GetAudioMixer(AudioType type)
    {
        AudioMixerGroup[] audioMixerGroups = mixer.FindMatchingGroups(EnumConverter.GetString(type));   // 오디오 믹서 그룹 찾기
        if (audioMixerGroups.Length > 0)    // 오디오 믹서 그룹을 찾았을 경우
        {
            return audioMixerGroups[0];     // 오디오 믹서 그룹의 첫 번째 return (타입 별로 찾았기 때문에 하나만)
        }
        else
        {
            Debug.LogError("SFX 그룹을 찾을 수 없습니다.");
            return null;
        }
    }

    // 마스터 볼륨 조절
    public void SetMasterVolume(float volume)
    {
        mixer.SetFloat(MasterGroupName, Mathf.Log10(volume) * 20);
    }

    // BGM 볼륨 조절
    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat(BGMGroupName, Mathf.Log10(volume) * 20);
    }

    // SFX 볼륨 조절
    public void SetSFXVolume(float volume)
    {
        mixer.SetFloat(SFXGroupName, Mathf.Log10(volume) * 20);
    }

    // Voice 볼륨 조절
    public void SetVoiceVolume(float volume)
    {
        mixer.SetFloat(VoiceGroupName, Mathf.Log10(volume) * 20);
    }
}
