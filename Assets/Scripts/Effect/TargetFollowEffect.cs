using UnityEngine;

public class TargetFollowEffect : Effect
{
    public float newYPos;
    private Vector3 newPos;
    private GameObject target;      // 따라다닐 타겟

    // 이펙트 사용
    public override void Use()
    {
        base.Use();
    }

    public void SetTarget(GameObject obj)
    {
        target = obj;
    }

    public override void Stop()
    {
        base.Stop();
        target = null;
    }

    // 타겟 따라다니기
    private void FollowTargetPos()
    {
        if (target == null || particle == null || particle.isStopped)       // 타겟이 없거나, 파티클이 없거나, 파티클이 멈춰있는 경우 return
            return;

        newPos = target.transform.position;
        newPos.y += newYPos;
        particle.gameObject.transform.position = newPos; // 따라다니기
    }

    private void Update()
    {
        FollowTargetPos();
    }
}
