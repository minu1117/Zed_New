using UnityEngine;

public class SkillWindowController : InteractiveObject
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

        if (player != null && player.GetMoveController() != null)
        {
            var moveController = player.GetMoveController();
            if (skillWindow.activeSelf == false)
            {
                player.SetAttackUse(false);
                moveController.StopMove();
                StartSound(interactionSound);
            }
            else
            {
                player.SetAttackUse(true);
                moveController.StartMove();
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
                player.SetAttackUse(true);
                player.GetMoveController().StartMove();
            }
        }

        Interaction();
    }

    protected override void Interaction()
    {
        if (!isInteractable)
            return;

        if (!CheackDistance())
            return;

        if (Input.GetKeyDown(interactionKey))
        {
            ClickContorllButton();
        }
    }
}
