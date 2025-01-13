using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public List<AudioClip> bgmSounds;       // BGM 사운드들
    public List<AudioClip> sfxSounds;       // SFX 사운드들
    public List<AudioClip> voiceSounds;     // 음성 사운드들
    private Dictionary<string, AudioSource> createdClips = new();   // 생성된 오디오 소스들을 저장하는 곳

    protected override void Awake()
    {
        base.Awake();
    }

    protected void Start()
    {
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
            if (clip == null)
                continue;

            GameObject audioObject = new GameObject(clip.name);             // 사운드 오브젝트 생성 (사운드 클립의 이름으로)
            AudioSource source = audioObject.AddComponent<AudioSource>();   // 오디오 소스 컴포넌트 붙이기
            source.clip = clip;                                             // 사운드 클립 할당
            source.playOnAwake = false;                                     // 첫 시작 시 사운드가 나오는 것을 방지하기 위해 playOnAwake 비활성화

            source.gameObject.transform.SetParent(parentObj.transform, false);                  // 생성된 사운드 오브젝트 부모 변경
            source.outputAudioMixerGroup = AudioMixerController.Instance.GetAudioMixer(type);   // 아웃풋 설정
            createdClips.Add(clip.name, source);                                                // 사운드 오브젝트 저장
        }
    }

    // 사운드 한 번 재생
    public void PlayOneShot(AudioClip clip)
    {
        if (clip == null || !Exist(clip.name))                  // 클립이 없을 경우 return
            return;

        var audio = createdClips[clip.name];    // 사운드 가져오기
        audio.PlayOneShot(audio.clip);          // 사운드 한 번 재생
    }

    // 사운드 재생
    public void Play(AudioClip clip)
    {
        if (clip == null || !Exist(clip.name))          // 클립이 없을 경우 return
            return;

        createdClips[clip.name].Play(); // 사운드 재생
    }

    // 사운드 정지
    public void Stop(AudioClip clip)
    {
        if (clip == null || !Exist(clip.name))          // 클립이 없을 경우 return
            return;

        createdClips[clip.name].Stop(); // 사운드 정지
    }

    // 사운드 일시 정지
    public void Pause(AudioClip clip)
    {
        if (clip == null || !Exist(clip.name))              // 클립이 없을 경우 return
            return;

        createdClips[clip.name].Pause();    // 일시 정지
    }

    // 사운드 반복 재생 설정
    public void SetLoop(AudioClip clip, bool set)
    {
        if (clip == null || !Exist(clip.name))              // 클립이 없을 경우 return
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

    public void StopAllBgm()
    {
        foreach (var bgmClip in bgmSounds)
        {
            if (!createdClips.ContainsKey(bgmClip.name))
                continue;

            createdClips[bgmClip.name].Stop();
        }
    }

    public void StartSound(List<AudioClip> clipList)
    {
        if (clipList == null || clipList.Count == 0)           // 사운드가 없을 경우 return
            return;

        int index = GetRandomIndex(0, clipList.Count);         // 랜덤 인덱스 (사운드 클립들 중 하나를 재생하기 위함)
        PlayOneShot(clipList[index]);                          // 시전 사운드들 중 랜덤 인덱스에 위치한 사운드 재생
    }

    private int GetRandomIndex(int min, int max) { return Random.Range(min, max); }
}
