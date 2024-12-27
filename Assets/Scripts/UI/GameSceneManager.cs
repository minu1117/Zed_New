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
}
