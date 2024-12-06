using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    [SerializeField] private float doorMoveDuration;
    private float movePos = 0.5f;
    private float defalutPos = 0.5f;
    private BoxCollider coll;

    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
        coll.enabled = false;
    }

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

        ResetDoor();
        stageManager.NextStage();
        stageManager.FadeOut();
        moveController.StartMove();
    }

    public void Open()
    {
        Move(true);
        coll.enabled = true;
    }

    public void Close()
    {
        Move(false);
        coll.enabled = false;
    }

    private void Move(bool open)
    {
        if (leftDoor == null || rightDoor == null)
            return;

        var leftMove = leftDoor.transform.localPosition;
        var rightMove = rightDoor.transform.localPosition;

        leftMove.x = open ? -defalutPos - movePos : -defalutPos;
        rightMove.x = open ? defalutPos + movePos : defalutPos;

        leftDoor.transform.DOLocalMove(leftMove, doorMoveDuration);
        rightDoor.transform.DOLocalMove(rightMove, doorMoveDuration);
    }

    public void ResetDoor()
    {
        if (leftDoor == null || rightDoor == null)
            return;

        var leftDoorPos = leftDoor.gameObject.transform.localPosition;
        var rightDoorPos = rightDoor.gameObject.transform.localPosition;

        leftDoor.gameObject.transform.localPosition = new Vector3(-defalutPos, leftDoorPos.y, leftDoorPos.z);
        rightDoor.gameObject.transform.localPosition = new Vector3(defalutPos, rightDoorPos.y, rightDoorPos.z);
    }
}
