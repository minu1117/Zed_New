using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != EnumConverter.GetString(CharacterEnum.Player))
            return;

        StartCoroutine(Teleport());
    }

    private IEnumerator Teleport()
    {
        stageManager.FadeIn();
        var moveController = Zed.Instance.GetMoveController();
        moveController.StopMove();

        yield return new WaitUntil(() => !stageManager.isFade);

        stageManager.NextStage();
        stageManager.FadeOut();
        moveController.StartMove();
    }
}
