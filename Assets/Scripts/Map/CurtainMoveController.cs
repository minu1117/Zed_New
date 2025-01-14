using DG.Tweening;
using UnityEngine;

public class CurtainMoveController : MonoBehaviour
{
    [SerializeField] private GameObject leftCurtain;
    [SerializeField] private GameObject rightCurtain;
    [SerializeField] private float doorMoveDuration;
    [SerializeField] private float movePos;
    [SerializeField] private RotateType rotateType;
    [SerializeField] private Ease ease;
    [SerializeField] private AudioClip curtainSoundClip;
    private float defalutLeftPos;
    private float defalutRightPos;

    private bool isTrigged = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != EnumConverter.GetString(CharacterEnum.Enemy))
            return;

        isTrigged = true;
    }

    public bool GetIsTrigged() { return isTrigged; }
    public void SetIsTrigged(bool set) { isTrigged = set; }

    private void Awake()
    {
        switch (rotateType)
        {
            case RotateType.X:
                defalutLeftPos = leftCurtain.transform.localPosition.x;
                defalutRightPos = rightCurtain.transform.localPosition.x;
                break;
            case RotateType.Y:
                defalutLeftPos = leftCurtain.transform.localPosition.y;
                defalutRightPos = rightCurtain.transform.localPosition.y;
                break;
            case RotateType.Z:
                defalutLeftPos = leftCurtain.transform.localPosition.z;
                defalutRightPos = rightCurtain.transform.localPosition.z;
                break;
        }
    }

    public void Open()
    {
        SoundManager.Instance.PlayOneShot(curtainSoundClip);
        Move(true);
    }

    public void Close()
    {
        SoundManager.Instance.PlayOneShot(curtainSoundClip);
        Move(false);
    }

    private void Move(bool open)
    {
        if (leftCurtain == null || rightCurtain == null)
            return;

        var leftMove = leftCurtain.transform.localPosition;
        var rightMove = rightCurtain.transform.localPosition;

        switch (rotateType)
        {
            case RotateType.X:
                leftMove.x = open ? defalutLeftPos - movePos : defalutLeftPos;
                rightMove.x = open ? defalutRightPos + movePos : defalutRightPos;
                break;
            case RotateType.Y:
                leftMove.y = open ? defalutLeftPos - movePos : defalutLeftPos;
                rightMove.y = open ? defalutRightPos + movePos : defalutRightPos;
                break;
            case RotateType.Z:
                leftMove.z = open ? defalutLeftPos - movePos : defalutLeftPos;
                rightMove.z = open ? defalutRightPos + movePos : defalutRightPos;
                break;
        }

        leftCurtain.transform.DOLocalMove(leftMove, doorMoveDuration).SetEase(ease);
        rightCurtain.transform.DOLocalMove(rightMove, doorMoveDuration).SetEase(ease);
    }

    public void ResetDoor()
    {
        if (leftCurtain == null || rightCurtain == null)
            return;

        var leftDoorPos = leftCurtain.gameObject.transform.localPosition;
        var rightDoorPos = rightCurtain.gameObject.transform.localPosition;

        switch (rotateType)
        {
            case RotateType.X:
                leftCurtain.gameObject.transform.localPosition = new Vector3(defalutLeftPos, leftDoorPos.y, leftDoorPos.z);
                rightCurtain.gameObject.transform.localPosition = new Vector3(defalutRightPos, rightDoorPos.y, rightDoorPos.z);
                break;
            case RotateType.Y:
                leftCurtain.gameObject.transform.localPosition = new Vector3(leftDoorPos.x, defalutLeftPos, leftDoorPos.z);
                rightCurtain.gameObject.transform.localPosition = new Vector3(rightDoorPos.x, defalutRightPos, rightDoorPos.z);
                break;
            case RotateType.Z:
                leftCurtain.gameObject.transform.localPosition = new Vector3(leftDoorPos.x, leftDoorPos.y, defalutLeftPos);
                rightCurtain.gameObject.transform.localPosition = new Vector3(rightDoorPos.x, rightDoorPos.y, defalutRightPos);
                break;
        }
    }
}
