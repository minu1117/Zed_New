using UnityEngine;

public class StartStage : MonoBehaviour
{
    [SerializeField] private Map map;
    [SerializeField] private Light directionalLight;
    [SerializeField] private Transform startPos;
    [SerializeField] private Portal storyPortal;
    [SerializeField] private Portal bossRaidPortal;
    [SerializeField] private Portal challengePortal;
    [SerializeField] private AudioClip bgmClip;

    public void Awake()
    {
        Warp();
    }

    private void Start()
    {
        if (storyPortal == null)
            return;

        StartBgm();
        map.StartCheakMapClear();
        storyPortal.Open();
    }

    public void Warp()
    {
        Zed.Instance.GetMoveController().GetAgent().Warp(startPos.transform.position);
        Zed.Instance.gameObject.transform.position = startPos.transform.position;
    }

    public void StartBgm()
    {
        SoundManager.Instance.Play(bgmClip);
        SoundManager.Instance.SetLoop(bgmClip, true);
    }

    public void StopBgm()
    {
        SoundManager.Instance.Stop(bgmClip);
    }

    public void SetActiveLight(bool set)
    {
        map.SetActiveLight(set);
    }

    public Light GetDirectionalLight() { return directionalLight; }
}
