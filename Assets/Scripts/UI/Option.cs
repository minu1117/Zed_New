using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private SoundSetting soundSetting;
    private Button optionButton;

    private void Start()
    {
        soundSetting.Init();
        SetActiveOptionPanel(false);
        exitButton.onClick.AddListener(() => SetActiveOptionPanel(false));
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
}
