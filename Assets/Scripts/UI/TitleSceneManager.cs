using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        StartVideo();
        OnClickStartButton();
        CustomSceneManager.Instance.SetOptionButton(optionButton);
        CustomSceneManager.Instance.GetOption().SetActiveTitleOptionButtons(false);
        exitButton.onClick.AddListener(() => Application.Quit());
    }

    public void OnClickStartButton()
    {
        if (startButton == null)
            return;

        startButton.onClick.AddListener(CustomSceneManager.Instance.LoadGameScene);
    }

    public Button GetOptionButton()
    {
        return optionButton;
    }

    private void StartVideo()
    {
        if (videoPlayer == null)
            return;

        var mainCam = Camera.main;
        videoPlayer.targetCamera = mainCam;
        videoPlayer.Play();
    }
}
