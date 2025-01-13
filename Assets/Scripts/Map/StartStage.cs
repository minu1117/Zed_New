using UnityEngine;

public class StartStage : MonoBehaviour
{
    [SerializeField] private Map map;
    [SerializeField] private Light directionalLight;
    [SerializeField] private Transform startPos;
    [SerializeField] private Portal storyPortal;
    [SerializeField] private Portal bossRaidPortal;
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private SkyboxChanger skyboxChanger;

    private void Start()
    {
        Warp();

        if (storyPortal == null)
            return;

        StartBgm();
        map.StartCheakMapClear();
        storyPortal.Open();
        skyboxChanger.ChangeSunSource(directionalLight);
        skyboxChanger.ChangeToDaySkybox(Skybox.Defalut);

        if (GameSceneManager.Instance.data.ending)
        {
            bossRaidPortal.Open();
        }
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
