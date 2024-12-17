using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Button startButton;

    private void Awake()
    {
        StartVideo();
        OnClickStartButton();
    }

    public void OnClickStartButton()
    {
        if (startButton == null)
            return;

        startButton.onClick.AddListener(CustomSceneManager.Instance.LoadGameScene);
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
