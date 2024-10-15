using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SoundManager : Singleton<SoundManager>
{
    public List<AudioClip> bgmSounds;       // BGM 사운드들
    public List<AudioClip> sfxSounds;       // SFX 사운드들
    public List<AudioClip> voiceSounds;     // 음성 사운드들
    private Dictionary<string, AudioSource> createdClips = new();   // 생성된 오디오 소스들을 저장하는 곳

    //public AssetLabelReference bgmLabel;
    //public AssetLabelReference sfxLabel;
    //public AssetLabelReference voiceLabel;

    protected override void Awake()
    {
        base.Awake();
        CreateSound(sfxSounds, AudioType.SFX);      // SFX 사운드 오브젝트 생성
        CreateSound(bgmSounds, AudioType.BGM);      // BGM 사운드 오브젝트 생성
        CreateSound(voiceSounds, AudioType.Voice);  // Voice 사운드 오브젝트 생성
    }

    private void CreateSound(List<AudioClip> clips, AudioType type)
    {
        if (clips == null || clips.Count == 0)  // 사운드 클립이 없을 경우 return
            return;

        var parentObj = new GameObject(EnumConverter.GetString(type));  // 생성한 오브젝트들을 한 곳에 담아둘 부모 오브젝트 생성
        parentObj.gameObject.transform.SetParent(transform, false);     // 사운드 매니저의 하위로 이동

        // 오디오 클립 List 순회
        foreach (var clip in clips)
        {
            GameObject audioObject = new GameObject(clip.name);             // 사운드 오브젝트 생성 (사운드 클립의 이름으로)
            AudioSource source = audioObject.AddComponent<AudioSource>();   // 오디오 소스 컴포넌트 붙이기
            source.clip = clip;                                             // 사운드 클립 할당
            source.playOnAwake = false;                                     // 첫 시작 시 사운드가 나오는 것을 방지하기 위해 playOnAwake 비활성화

            source.gameObject.transform.SetParent(parentObj.transform, false);                  // 생성된 사운드 오브젝트 부모 변경
            source.outputAudioMixerGroup = AudioMixerController.Instance.GetAudioMixer(type);   // 아웃풋 설정
            createdClips.Add(clip.name, source);                                                // 사운드 오브젝트 저장
        }
    }

    // 미완성 (어드레서블 무한로딩 에러남)
    private void CreateSound(AssetLabelReference label, AudioType type)
    {
        //var result = AddressableManager.LoadSounds(label).Result;
        var result = AddressableManager.LoadSoundss(label);
        var list = result == null ? null : result.Result;
        if (list == null || list.Count == 0)
            return;

        var parentObj = new GameObject(EnumConverter.GetString(type));  // 생성한 오브젝트들을 한 곳에 담아둘 부모 오브젝트 생성
        parentObj.gameObject.transform.SetParent(transform, false);     // 사운드 매니저의 하위로 이동

        // 오디오 클립 List 순회
        foreach (var clip in list)
        {
            GameObject audioObject = new GameObject(clip.name);             // 사운드 오브젝트 생성 (사운드 클립의 이름으로)
            AudioSource source = audioObject.AddComponent<AudioSource>();   // 오디오 소스 컴포넌트 붙이기
            source.clip = clip;                                             // 사운드 클립 할당
            source.playOnAwake = false;                                     // 첫 시작 시 사운드가 나오는 것을 방지하기 위해 playOnAwake 비활성화

            source.gameObject.transform.SetParent(parentObj.transform, false);                  // 생성된 사운드 오브젝트 부모 변경
            source.outputAudioMixerGroup = AudioMixerController.Instance.GetAudioMixer(type);   // 아웃풋 설정
            createdClips.Add(clip.name, source);                                                // 사운드 오브젝트 저장
        }

        Debug.Log("Done");
    }

    // 사운드 한 번 재생
    public void PlayOneShot(AudioClip clip)
    {
        if (!Exist(clip.name))                  // 클립이 없을 경우 return
            return;

        var audio = createdClips[clip.name];    // 사운드 가져오기
        audio.PlayOneShot(audio.clip);          // 사운드 한 번 재생
    }

    // 사운드 재생
    public void Play(AudioClip clip)
    {
        if (!Exist(clip.name))          // 클립이 없을 경우 return
            return;

        createdClips[clip.name].Play(); // 사운드 재생
    }

    // 사운드 정지
    public void Stop(AudioClip clip)
    {
        if (!Exist(clip.name))          // 클립이 없을 경우 return
            return;

        createdClips[clip.name].Stop(); // 사운드 정지
    }

    // 사운드 일시 정지
    public void Pause(AudioClip clip)
    {
        if (!Exist(clip.name))              // 클립이 없을 경우 return
            return;

        createdClips[clip.name].Pause();    // 일시 정지
    }

    // 사운드 반복 재생 설정
    public void SetLoop(AudioClip clip, bool set)
    {
        if (!Exist(clip.name))              // 클립이 없을 경우 return
            return;

        createdClips[clip.name].loop = set; // 반복 재생 활성화
    }

    // 사운드 오브젝트가 생성되었고, 존재하는지 확인
    private bool Exist(string name)
    {
        if (createdClips == null || createdClips.Count == 0)    // 생성된 사운드 오브젝트가 없을 경우 false return
            return false;

        if (!createdClips.ContainsKey(name))    // 같은 이름을 가진 사운드 오브젝트가 없을 경우 false return
            return false;

        return true;    // 위에서 없을 때의 과정을 모두 통과하였으니 true return
    }
}
