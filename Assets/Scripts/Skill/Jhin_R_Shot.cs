using System.Collections;
using UnityEngine;

public class Jhin_R_Shot : ShotSkill
{
    [SerializeField] private GameObject bulletObj;
    [SerializeField] private Effect shootEffect;
    private WaitForSeconds waitDeltaTime;

    public override void Awake()
    {
        base.Awake();
        waitDeltaTime = new WaitForSeconds(Time.deltaTime);
    }

    public override void Use(GameObject character)
    {
        if (!SubMP())
        {
            Release();
            return;
        }
        StartCoroutine(CoShot());
    }

    private IEnumerator CoShot()
    {
        bulletObj.gameObject.SetActive(false);
        Vector3 direction = (usePoint - transform.position).normalized;
        Vector3 movePoint = transform.position + (direction * data.distance);
        transform.LookAt(movePoint);

        yield return waitUseDelay;

        bulletObj.gameObject.SetActive(true);
        StartSound(data.useClips);      // 스킬 시전 사운드 재생
        StartSound(data.voiceClips);    // 시전 보이스 재생
        SetActiveTrailRenderer(true);   // TrailRenderer 활성화

        UseEffect(gameObject);
        var shoot = EffectManager.Instance.UseEffect(shootEffect, transform);
        isCollide = false;

        float startTime = Time.time;
        float distanceToTarget = Vector3.Distance(transform.position, movePoint);
        while (distanceToTarget > 0.1f)
        {
            distanceToTarget = Vector3.Distance(transform.position, movePoint);

            float distanceCovered = (Time.time - startTime) * data.speed;
            float fractionOfJourney = distanceCovered / distanceToTarget;

            transform.position = Vector3.Lerp(transform.position, movePoint, fractionOfJourney);
            yield return null;
        }

        if (shoot != null)
        {
            shoot.Stop();
            EffectManager.Instance.ReleaseEffect(shoot);
        }

        Release();
    }
}
