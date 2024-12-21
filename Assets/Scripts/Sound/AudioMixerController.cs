using System;
using UnityEngine;
using UnityEngine.Audio;

// 오디오 그룹 타입
public enum AudioType
{
    Master,
    BGM,
    SFX,
    Voice,
}

[Serializable]
public struct SoundSettingData
{
    public float masterVolumeValue;
    public float bgmVolumeValue;
    public float sfxVolumeValue;
    public float voiceVolumeValue;
}

public class AudioMixerController : Singleton<AudioMixerController>
{
    [SerializeField] private AudioMixer mixer;              // 오디오 믹서
    private string MasterGroupName;                          // 마스터 볼륨 그룹 이름
    private string BGMParamName;                             // BGM 볼륨 파라미터 이름
    private string SFXParamName;                             // SFX 볼륨 파라미터 이름
    private string VoiceParamName;                           // Voice 볼륨 파라미터 이름

    private SoundSettingData data;

    protected override void Awake()
    {
        base.Awake();
        data = new();
        MasterGroupName = EnumConverter.GetString(AudioType.Master);
        BGMParamName = EnumConverter.GetString(AudioType.BGM);
        SFXParamName = EnumConverter.GetString(AudioType.SFX);
        VoiceParamName = EnumConverter.GetString(AudioType.Voice);
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
            return null;
        }
    }

    // 마스터 볼륨 조절
    public void SetMasterVolume(float volume)
    {
        if (volume <= 0.0001f)
        {
            mixer.SetFloat(MasterGroupName, -80f); // 음소거
        }
        else
        {
            mixer.SetFloat(MasterGroupName, Mathf.Log10(volume) * 20);
        }

        data.masterVolumeValue = volume * 100f;
    }

    // BGM 볼륨 조절
    public void SetMusicVolume(float volume)
    {
        if (volume <= 0.0001f)
        {
            mixer.SetFloat(BGMParamName, -80f); // 음소거
        }
        else
        {
            mixer.SetFloat(BGMParamName, Mathf.Log10(volume) * 20);
        }

        data.bgmVolumeValue = volume * 100f;
    }

    // SFX 볼륨 조절
    public void SetSFXVolume(float volume)
    {
        if (volume <= 0.0001f)
        {
            mixer.SetFloat(SFXParamName, -80f); // 음소거
        }
        else
        {
            mixer.SetFloat(SFXParamName, Mathf.Log10(volume) * 20);
        }

        data.sfxVolumeValue = volume * 100f;
    }

    // Voice 볼륨 조절
    public void SetVoiceVolume(float volume)
    {
        if (volume <= 0.0001f)
        {
            mixer.SetFloat(VoiceParamName, -80f); // 음소거
        }
        else
        {
            mixer.SetFloat(VoiceParamName, Mathf.Log10(volume) * 20);
        }

        data.voiceVolumeValue = volume * 100f;
    }

    public void Save()
    {
        SaveLoadManager.Save(data, SaveLoadMode.SoundSetting);
    }

    public SoundSettingData Load()
    {
        var loadData = SaveLoadManager.Load<SoundSettingData>(SaveLoadMode.SoundSetting);
        SetMasterVolume(loadData.masterVolumeValue);
        SetMusicVolume(loadData.bgmVolumeValue);
        SetSFXVolume(loadData.sfxVolumeValue);
        SetVoiceVolume(loadData.voiceVolumeValue);

        data = loadData;
        return data;
    }

    public void OnDestroy()
    {
        Save();
    }

    public void OnApplicationQuit()
    {
        Save();
    }
}
