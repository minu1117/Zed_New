using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] protected StageManager stageManager;
    [SerializeField] protected GameObject blockObj;
    [SerializeField] protected GameObject leftDoor;
    [SerializeField] protected GameObject rightDoor;
    [SerializeField] protected float doorMoveDuration;
    protected float movePos = 0.5f;
    protected float defalutPos = 0.5f;
    protected BoxCollider coll;

    protected virtual void Awake()
    {
        coll = GetComponent<BoxCollider>();
        coll.enabled = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag != EnumConverter.GetString(CharacterEnum.Player))
            return;

        Teleport();
    }

    protected void Teleport()
    {
        StartCoroutine(CoTeleport());
    }

    protected IEnumerator CoTeleport()
    {
        var sceneManager = CustomSceneManager.Instance;
        sceneManager.FadeIn();
        var moveController = Zed.Instance.GetMoveController();
        moveController.StopMove();

        yield return new WaitUntil(() => !sceneManager.isFade);

        ResetDoor();
        stageManager.NextStage();
        sceneManager.FadeOut();
        moveController.StartMove();
    }

    protected void TeleportSelectMap()
    {
        StartCoroutine (CoTeleportSelectMap());
    }

    protected IEnumerator CoTeleportSelectMap()
    {
        var sceneManager = CustomSceneManager.Instance;
        sceneManager.FadeIn();
        var moveController = Zed.Instance.GetMoveController();
        moveController.StopMove();

        yield return new WaitUntil(() => !sceneManager.isFade);

        ResetDoor();
        stageManager.MoveCurrentStage();
        sceneManager.FadeOut();
        moveController.StartMove();
    }

    public void Open()
    {
        if (blockObj != null)
        {
            blockObj.SetActive(false);
        }

        Move(true);
        coll.enabled = true;
    }

    public void Close()
    {
        if (blockObj != null)
        {
            blockObj.SetActive(true);
        }

        Move(false);
        coll.enabled = false;
    }

    protected void Move(bool open)
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