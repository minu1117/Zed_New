using UnityEngine;

public class SkillWindowController : MonoBehaviour
{
    public GameObject skillWindow;
    public GameObject opacityPanel;

    public void ClickContorllButton()
    {
        if (DialogueManager.Instance.isTalking)
        {
            if (skillWindow.activeSelf)
                skillWindow.SetActive(false);

            return;
        }

        skillWindow.SetActive(!skillWindow.activeSelf);
        opacityPanel.SetActive(skillWindow.activeSelf);
    }
}
