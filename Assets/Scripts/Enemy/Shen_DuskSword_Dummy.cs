using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum ShenDuskSwordAudio
{
    Use,
    Catch,
    Attack,
    Voice,
}

public class Shen_DuskSword_Dummy : MonoBehaviour
{
    [SerializeField] private LineRenderer duskSwordLineRenderer;
    public Transform LineRendererTr;
    public float upPos;
    private Shen shen;

    [SerializeField] private List<AudioClip> useClips;
    [SerializeField] private List<AudioClip> catchClips;
    [SerializeField] private List<AudioClip> attackClips;
    [SerializeField] private List<AudioClip> voiceClips;
    [SerializeField] private List<AudioClip> attackVoiceClips;

    public void SetShen(Shen shen) { this.shen = shen; }

    public void Update()
    {
        DrawLine();
    }

    private void DrawLine()
    {
        if (shen == null)
            return;

        duskSwordLineRenderer.positionCount = 2;
        duskSwordLineRenderer.SetPosition(0, shen.lineLendererTransform.position);
        duskSwordLineRenderer.SetPosition(1, LineRendererTr.position);
    }

    public AudioClip GetAudioClip(ShenDuskSwordAudio type)
    {
        AudioClip clip = null;

        switch (type)
        {
            case ShenDuskSwordAudio.Use:
                clip = GetRandomClip(useClips);
                break;
            case ShenDuskSwordAudio.Catch:
                clip = GetRandomClip(catchClips);
                break;
            case ShenDuskSwordAudio.Attack:
                clip = GetRandomClip(attackClips);
                break;
            case ShenDuskSwordAudio.Voice:
                clip = GetRandomClip(voiceClips);
                break;
            default:
                break;
        }

        return clip;
    }

    private AudioClip GetRandomClip(List<AudioClip> clip)
    {
        if (clip == null || clip.Count == 0)
            return null;

        int index = Random.Range(0, clip.Count);
        return clip[index];
    }
}
