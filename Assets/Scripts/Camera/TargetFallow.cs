using UnityEngine;

public class FallowTarget : MonoBehaviour
{
    public GameObject target;       // 따라다닐 타겟
    private Collider targetCollider;
    private Vector3 targetPos;
    private float upPos;

    private void Awake()
    {
        if (target == null)
        {
            target = Zed.Instance.gameObject;
        }

        if (target.TryGetComponent<Collider>(out var coll))
        {
            targetCollider = coll;
            upPos = (target.transform.position + Vector3.up * targetCollider.bounds.size.y).y;
        }
    }

    private void FixedUpdate()
    {
        if (target == null || targetCollider == null) // 따라다닐 타겟이 없을 경우 return
            return;

        targetPos = target.transform.position;
        targetPos.y = upPos;
        gameObject.transform.position = targetPos;  // 타겟 따라다니기
    }
}
