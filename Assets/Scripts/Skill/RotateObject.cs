using UnityEngine;
using UnityEngine.UIElements;

// 회전할 방향 enum
public enum RotateType
{
    None = -1,

    X,
    Y,
    Z,

    Count,
}

public class RotateObject : MonoBehaviour
{
    public GameObject rotateObject; // 회전시킬 오브젝트
    public RotateType rotateType;   // 회전시킬 방향
    public float rotateSpeed;       // 회전 속도
    private Vector3 rotateVec;

    private void Awake()
    {
        switch (rotateType)
        {
            case RotateType.X:
                rotateVec = new Vector3(1, 0, 0);
                break;
            case RotateType.Y:
                rotateVec = new Vector3(0, 1, 0);
                break;
            case RotateType.Z:
                rotateVec = new Vector3(0, 0, 1);
                break;
            case RotateType.Count:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        Rotate();
    }

    // 회전 시키기
    private void Rotate()
    {
        //Vector3 rotateVec = Vector3.zero;

        //float rotate = 360 * rotateSpeed;
        //switch (rotateType)
        //{
        //    case RotateType.X:
        //        rotateVec.x = rotate;
        //        break;
        //    case RotateType.Y:
        //        rotateVec.y = rotate;
        //        break;
        //    case RotateType.Z:
        //        rotateVec.z = rotate;
        //        break;
        //    default:
        //        break;
        //}

        //rotateObject.transform.DOLocalRotate(rotateVec, data.duration, RotateMode.FastBeyond360)    // 무한 회전
        //                      .SetEase(Ease.Linear);

        float x = rotateVec.x * rotateSpeed * Time.deltaTime;
        float y = rotateVec.y * rotateSpeed * Time.deltaTime;
        float z = rotateVec.z * rotateSpeed * Time.deltaTime;
        rotateObject.transform.Rotate(x, y, z);
    }
}
