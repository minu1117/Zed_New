using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueStarter dialogueStarter;
    private bool isEnter = false;

    // 충돌 처리
    private void OnTriggerEnter(Collider other)
    {
        if (isEnter)
            return;

        var player = other.GetComponent<Zed>();
        if (player == null)
            return;

        if (dialogueStarter == null)
            return;

        isEnter = dialogueStarter.StartDialogue(other.gameObject);
    }
}
