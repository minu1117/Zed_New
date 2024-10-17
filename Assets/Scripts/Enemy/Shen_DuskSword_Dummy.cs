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
    private Shen shen;

    [SerializeField] private List<AudioClip> useClips;
    [SerializeField] private List<AudioClip> catchClips;
    [SerializeField] private List<AudioClip> attackClips;
    [SerializeField] private List<AudioClip> voiceClips;
    [SerializeField] private List<AudioClip> attackVoiceClips;

    private float defaultYPos;
    private float upYPos;
    private float up = 0.5f;
    private float moveTime = 1f;
    private Coroutine moveCoroutine;

    public void SetShen(Shen shen) { this.shen = shen; }

    public void Update()
    {
        DrawLine();
        UpAndDownMove();
    }

    private void DrawLine()
    {
        if (shen == null)
            return;

        duskSwordLineRenderer.positionCount = 2;
        duskSwordLineRenderer.SetPosition(0, shen.lineLendererTransform.position);
        duskSwordLineRenderer.SetPosition(1, LineRendererTr.position);
    }

    private void UpAndDownMove()
    {
        if (moveCoroutine != null)
            return;

        float yPos = 0f;
        if (Mathf.Abs(transform.position.y - upYPos) > 0.01f)
        {
            yPos = upYPos;
        }
        else
        {
            yPos = defaultYPos;
        }

        moveCoroutine = StartCoroutine(CoMove(yPos));
    }

    private IEnumerator CoMove(float yPos)
    {
        Vector3 currentPos = transform.position;
        Vector3 movePos = new Vector3(transform.position.x, upYPos, transform.position.z);

        while (Mathf.Abs(currentPos.y - yPos) > 0.01f)
        { 
            currentPos = transform.position;
            movePos = new Vector3(transform.position.x, yPos, transform.position.z);
            transform.position = Vector3.Lerp(currentPos, movePos, moveTime * Time.deltaTime);
            yield return null;
        }

        moveCoroutine = null;
    }

    public void SetInitPosY(float yPos)
    {
        defaultYPos = yPos;
        upYPos = yPos + up;
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
