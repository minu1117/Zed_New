using UnityEngine;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;
    [SerializeField] private Button optionButton;
    [SerializeField] private CameraShakeController shakeController;

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
    }

    public CameraShakeController GetCameraChakeController() { return  shakeController; }

    protected void OnDestroy()
    {
        if (Instance != null)
        {
            Instance = null;
        }
    }
}
