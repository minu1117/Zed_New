using UnityEngine;
using UnityEngine.UI;

public class SkillWindowController : InteractiveObject
{
    public GameObject skillWindow;
    public GameObject opacityPanel;

    protected override void Start()
    {
        base.Start();
    }

    public void ClickContorllButton()
    {
        if (DialogueManager.Instance.isTalking)
        {
            if (skillWindow.activeSelf)
                skillWindow.SetActive(false);

            return;
        }

        if (player != null && player.GetMoveController() != null)
        {
            if (skillWindow.activeSelf == false)
            {
                SetPlayerInput(false);
                StartSound(interactionSound);
            }
            else
            {
                SetPlayerInput(true);
                StartSound(escapeSound);
            }
        }

        skillWindow.SetActive(!skillWindow.activeSelf);
        opacityPanel.SetActive(skillWindow.activeSelf);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (skillWindow.activeSelf)
            {
                skillWindow.SetActive(false);
                opacityPanel.SetActive(false);
                StartSound(escapeSound);
            }

            if (player != null && player.GetMoveController() != null)
            {
                SetPlayerInput(true);
            }
        }

        Interaction();
    }

    protected override void Interaction()
    {
        if (!isInteractable)
            return;

        if (!CheckDistance())
            return;

        if (Input.GetKeyDown(interactionKey))
        {
            ClickContorllButton();
        }
    }

    private void SetPlayerInput(bool set)
    {
        player.SetAttackUse(set);

        var moveController = player.GetMoveController();
        if (set)
        {
            moveController.StartMove();
        }
        else
        {
            moveController.StopMove();
        }
    }
}
