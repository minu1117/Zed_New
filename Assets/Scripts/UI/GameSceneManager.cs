using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public struct PlayerData
{
    public bool ending;
}

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;
    [SerializeField] private Button optionButton;
    [SerializeField] private CameraShakeController shakeController;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private RestartController restartController;
    [SerializeField] private StartStageTeleporter startStageTeleporter;
    public PlayerData data;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = GetComponent<GameSceneManager>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        CustomSceneManager.Instance.SetOptionButton(optionButton);
        CustomSceneManager.Instance.GetOption().SetActiveTitleOptionButtons(true);

        Load();
    }

    public void SetEnding(bool set) { data.ending = set; }

    public CameraShakeController GetCameraChakeController() { return  shakeController; }
    public StageManager GetStageManager() {  return stageManager; }

    protected void OnDestroy()
    {
        Save();
        if (Instance != null)
        {
            Instance = null;
        }
    }

    protected void OnApplicationQuit()
    {
        Save();
    }

    public void Save()
    {
        SaveLoadManager.Save(data, SaveLoadMode.PlayerData);
    }

    public void Load()
    {
        data = SaveLoadManager.Load<PlayerData>(SaveLoadMode.PlayerData);
    }

    public void Defeat()
    {
        StartCoroutine(CoDefeat());
    }

    private IEnumerator CoDefeat()
    {
        restartController.SetActiveDefeatPanel(true);

        yield return new WaitUntil(() => restartController.GetIsComplate());

        var startStage = stageManager.GetStartStage();
        var map = stageManager.GetCurrentMap();
        var enemyGeneratorController = map.enemyGeneratorController;

        startStageTeleporter.SetEnemyGeneratorController(enemyGeneratorController);
        startStageTeleporter.ReleaseCurrentMapEnemies();

        Zed.Instance.Restart();

        startStageTeleporter.SetMap(map);
        startStageTeleporter.Teleport(stageManager, startStage);
    }
}
