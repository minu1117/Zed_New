using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private AudioClip titleSceneBgm;

    private void Start()
    {
        StartVideo();
        OnClickStartButton();
        CustomSceneManager.Instance.SetOptionButton(optionButton);
        CustomSceneManager.Instance.GetOption().SetActiveTitleOptionButtons(false);
        exitButton.onClick.AddListener(() => Application.Quit());
        SoundManager.Instance.Play(titleSceneBgm);
        SoundManager.Instance.SetLoop(titleSceneBgm, true);
    }

    public void OnClickStartButton()
    {
        if (startButton == null)
            return;

        startButton.onClick.AddListener(() => SoundManager.Instance.Stop(titleSceneBgm));
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
