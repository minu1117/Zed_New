using DG.Tweening;
using UnityEngine;

// 회전할 방향 enum
public enum RotateType
{
    None = -1,

    X,
    Y,
    Z,

    Count,
}

public class RotationShotSkill : ShotSkill
{
    public GameObject rotateObject; // 회전시킬 오브젝트
    public RotateType rotateType;   // 회전시킬 방향
    public float rotateSpeed;       // 회전 속도

    public override void Use(GameObject charactor)
    {
        base.Use(charactor);
        Rotate(rotateType);
    }

    // 회전 시키기
    private void Rotate(RotateType rotateType)
    {
        Vector3 rotateVec = Vector3.zero;
        float rotate = 360 * rotateSpeed;
        switch (rotateType)
        {
            case RotateType.X:
                rotateVec.x = rotate;
                break;
            case RotateType.Y:
                rotateVec.y = rotate;
                break;
            case RotateType.Z:
                rotateVec.z = rotate;
                break;
            default:
                break;
        }

        rotateObject.transform.DOLocalRotate(rotateVec, data.duration, RotateMode.FastBeyond360)    // 무한 회전
                              .SetEase(Ease.Linear);
    }
}
