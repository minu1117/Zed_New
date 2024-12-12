using UnityEngine;

public class InteractiveDialogue : InteractiveObject
{
    [SerializeField] private DialogueStarter dialogueStarter;

    private void Update()
    {
        if (!CheackDistance())
            return;

        Interaction();
    }

    protected override void Interaction()
    {
        if (dialogueStarter == null)
            return;

        if (Input.GetKeyDown(interactionKey))
        {
            dialogueStarter.StartDialogue(player.gameObject);
        }
    }
}
