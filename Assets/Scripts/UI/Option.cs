using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject guidePanel;
    [SerializeField] private SoundSetting soundSetting;
    [SerializeField] private Button guideButton;
    [SerializeField] private Button titleExitButton;
    [SerializeField] private Button quitButton;
    private Button optionButton;

    private void Start()
    {
        soundSetting.Init();
        soundSetting.AddSoundSettingButtonOnClick(() => guidePanel.SetActive(false));

        guideButton.onClick.AddListener(() => soundSetting.SetActiveViewport(false));
        guideButton.onClick.AddListener(() => guidePanel.SetActive(true));

        SetActiveOptionPanel(false);

        exitButton.onClick.AddListener(() => SetActiveOptionPanel(false));
        quitButton.onClick.AddListener(() => Application.Quit());

        titleExitButton.onClick.AddListener(() => SetActiveOptionPanel(false));
        titleExitButton.onClick.AddListener(() => SoundManager.Instance.StopAllBgm());
        titleExitButton.onClick.AddListener(() => DialogueManager.Instance.ResetDialogue());
        titleExitButton.onClick.AddListener(() => CustomSceneManager.Instance.LoadTitleScene());
    }

    public void SetOptionButton(Button button)
    {
        optionButton = button;
        optionButton.onClick.AddListener(() => SetActiveOptionPanel(true));
    }

    public void SetActiveOptionPanel(bool set)
    {
        optionPanel.SetActive(set);
    }

    public void SetActiveTitleOptionButtons(bool set)
    {
        titleExitButton.gameObject.SetActive(set);
        quitButton.gameObject.SetActive(set);
    }
}
