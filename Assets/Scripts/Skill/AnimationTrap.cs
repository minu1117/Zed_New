using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrap : Trap
{
    [SerializeField] private Animator animator;
    [SerializeField] private string fireComplateTriggerName;
    [SerializeField] private string detonateTriggerName;
    [SerializeField] private string resetTriggerName;
    [SerializeField] protected Effect sensingParticle;
    [SerializeField] protected Effect idleParticle;
    [SerializeField] private List<TrapSensorCollider> sensors;
    [SerializeField] private float waitSensorAnimationDelay;
    [SerializeField] private float burstDelay;
    [SerializeField] private AudioClip sensingAudio;

    private Effect usedIdleEffect;

    private bool isSensing = false;
    
    public override void Use(GameObject character)
    {
        StartSound(data.useClips);      // 스킬 시전 사운드 재생
        StartSound(data.voiceClips);    // 시전 보이스 재생
        StartCoroutine(CoUse());
    }

    private IEnumerator CoUse()
    {
        SetSensorsColliderEnable(false);

        isSensing = false;
        SoundManager.Instance.PlayOneShot(countdownSound);

        isCollide = true;
        foreach (var coll in colliders)
        {
            coll.GetCollider().enabled = false;
        }

        Vector3 movePoint = transform.position + (usePoint * data.distance);
        if (caster.TryGetComponent<BoxCollider>(out var boxCollider))
        {
            var yPos = boxCollider.bounds.min.y;
            movePoint.y = yPos;
        }

        UseEffect(gameObject);
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

        animator.SetTrigger(fireComplateTriggerName);
        usedIdleEffect = UseTrapEffect(idleParticle, transform);

        // 착지 애니메이션 대기 후 센서 활성화
        yield return new WaitForSeconds(waitSensorAnimationDelay);
        SetSensorsColliderEnable(true);

        // 센서 감지 대기
        yield return new WaitUntil(() => isSensing);
        if (sensingAudio != null)
        {
            SoundManager.Instance.PlayOneShot(sensingAudio);
        }

        if (usedIdleEffect != null)
        {
            usedIdleEffect.Stop();
            EffectManager.Instance.ReleaseEffect(usedIdleEffect);
            usedIdleEffect = null;
        }
        
        UseTrapEffect(sensingParticle, transform);

        animator.SetTrigger(detonateTriggerName);
        isSensing = false;

        // 터지기 전 대기
        yield return new WaitForSeconds(burstDelay);
        Burst();

        // 터진 후 대기
        yield return new WaitForSeconds(burstDuration);
        SetSensorsColliderEnable(false);
        isSensing = false;
        animator.SetTrigger(resetTriggerName);
        Release();
    }

    private void SetSensorsColliderEnable(bool set)
    {
        foreach (var sensor in sensors)
        {
            sensor.SetColliderEnable(set);
        }
    }

    public void Sensing() { isSensing = true; }

    private void OnDestroy()
    {
        if (usedIdleEffect != null)
        {
            usedIdleEffect.Stop();
            EffectManager.Instance.ReleaseEffect(usedIdleEffect);
            usedIdleEffect = null;
        }
    }
}
