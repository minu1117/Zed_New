using System.Collections;
using UnityEngine;

public class StartStageTeleporter : InteractiveObject
{
    [SerializeField] private Map map;
    [SerializeField] private EnemyGeneratorController enemyGeneratorController;
    [SerializeField] private GameSceneManager gameSceneManager;

    protected override void Start()
    {
        base.Start();
    }

    public void Update()
    {
        if (!Input.GetKeyDown(interactionKey))
            return;

        if (!CheckDistance())
            return;

        Interaction();
    }

    protected override void Interaction()
    {
        StartCoroutine(CoInteraction());
    }

    public void SetMap(Map map) { this.map = map; }
    public void SetEnemyGeneratorController(EnemyGeneratorController controller) { enemyGeneratorController = controller; }

    public void ReleaseCurrentMapEnemies()
    {
        var enemyGenerators = enemyGeneratorController.GetGenerators();
        foreach (var generator in enemyGenerators)
        {
            generator.ReleaseAll();
        }
    }

    public void Teleport(StageManager stageManager, StartStage startStage)
    {
        SoundManager.Instance.StopAllBgm();

        stageManager.SetActiveStartStage(true);
        stageManager.SetActiveStartStageLight(true);
        stageManager.ChangeSunSource(startStage.GetDirectionalLight());
        stageManager.ChangeSkybox(Skybox.Defalut);

        startStage.StartBgm();
        startStage.Warp();
        startStage.OpenPortals();

        if (map != null)
        {
            map.SetActiveLight(false);
            map.SetActiveVirtualCam(false);
            map.gameObject.SetActive(false);
        }

        stageManager.ResetAllMaps();
        var hpController = Zed.Instance.GetStatusController(SliderMode.HP);
        if (hpController != null)
        {
            hpController.SetMaxHp();
        }
    }

    private IEnumerator CoInteraction()
    {
        if (gameSceneManager == null)
            yield break;

        var stageManager = gameSceneManager.GetStageManager();
        if (stageManager == null)
            yield break;

        var startStage = stageManager.GetStartStage();
        if (startStage == null)
            yield break;

        ReleaseCurrentMapEnemies();

        var customSceneMgr = CustomSceneManager.Instance;
        customSceneMgr.FadeIn();

        yield return new WaitUntil(() => !customSceneMgr.isFade);
        Teleport(stageManager, startStage);
        stageManager.ResetAllMaps();

        customSceneMgr.FadeOut();
    }
}
