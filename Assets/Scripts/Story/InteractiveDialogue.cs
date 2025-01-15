using UnityEngine;

public class InteractiveDialogue : InteractiveObject
{
    [SerializeField] private DialogueStarter dialogueStarter;

    private void Update()
    {
        if (dialogueStarter == null)
            return;

        if (!CheckDistance())
            return;

        if (Input.GetKeyDown(interactionKey))
        {
            Interaction();
        }
    }

    protected override void Interaction()
    {
        dialogueStarter.StartDialogue(player.gameObject);
    }
}
