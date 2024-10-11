using UnityEngine;

public class FallowTarget : MonoBehaviour
{
    public GameObject target;       // 따라다닐 타겟

    private void FixedUpdate()
    {
        if (target == null) // 따라다닐 타겟이 없을 경우 return
            return; 

        gameObject.transform.position = target.transform.position;  // 타겟 따라다니기
    }
}
