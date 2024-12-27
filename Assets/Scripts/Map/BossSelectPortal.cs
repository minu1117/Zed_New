using System.Collections;
using UnityEngine;

public class BossSelectPortal : Portal
{
    [SerializeField] private BossSelectUI bossSelectUI;
    [SerializeField] private Map boss_1_Map;
    [SerializeField] private AudioClip boss_1_Map_AudioClip;

    [SerializeField] private Map boss_2_Map;
    [SerializeField] private AudioClip boss_2_Map_AudioClip;

    [SerializeField] private Map boss_3_Map;
    [SerializeField] private AudioClip boss_3_Map_AudioClip;

    [SerializeField] private Map boss_4_Map;
    [SerializeField] private AudioClip boss_4_Map_AudioClip;

    protected override void Awake()
    {
        base.Awake();
        var gm = GameSceneManager.Instance;

        bossSelectUI.AddListenerBoss_1(() => bossSelectUI.SetActiveUI(false));
        bossSelectUI.AddListenerBoss_1(() => StartCoroutine(CoTeleportBossMap(boss_1_Map, boss_1_Map_AudioClip)));

        bossSelectUI.AddListenerBoss_2(() => bossSelectUI.SetActiveUI(false));
        bossSelectUI.AddListenerBoss_2(() => StartCoroutine(CoTeleportBossMap(boss_2_Map, boss_2_Map_AudioClip)));

        bossSelectUI.AddListenerBoss_3(() => bossSelectUI.SetActiveUI(false));
        bossSelectUI.AddListenerBoss_3(() => StartCoroutine(CoTeleportBossMap(boss_3_Map, boss_3_Map_AudioClip)));

        bossSelectUI.AddListenerBoss_4(() => bossSelectUI.SetActiveUI(false));
        bossSelectUI.AddListenerBoss_4(() => StartCoroutine(CoTeleportBossMap(boss_4_Map, boss_4_Map_AudioClip)));
    }

    private IEnumerator CoTeleportBossMap(Map map, AudioClip clip)
    {
        var customSceneMgr = CustomSceneManager.Instance;
        customSceneMgr.FadeIn();

        yield return new WaitUntil(() => !customSceneMgr.isFade);
        TeleportBossMap(map.startingPos);

        var stageMgr = GameSceneManager.Instance.GetStageManager();
        map.SetActiveLight(true);
        stageMgr.ChangeSunSource(map.directionalLight);
        map.gameObject.SetActive(true);
        StartBGM(clip);
        stageMgr.SetActiveStartStageLight(false);
        stageMgr.StopStartStageBGM();
        customSceneMgr.FadeOut();
    }

    private void TeleportBossMap(Transform pos)
    {
        var zed = Zed.Instance;
        zed.GetMoveController().GetAgent().Warp(pos.position);
        zed.transform.position = pos.position;
        zed.GetMoveController().StartMove();
    }

    private void StartBGM(AudioClip clip)
    {
        SoundManager.Instance.Play(clip);
        SoundManager.Instance.SetLoop(clip, true);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.tag != EnumConverter.GetString(CharacterEnum.Player))
            return;

        var moveController = Zed.Instance.GetMoveController();
        moveController.StopMove();
        bossSelectUI.SetActiveUI(true);
    }
}
