using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Talon_Q : Skill
{
    public float meleeAttackRange;
    public AudioClip meleeUseClip;
    public AudioClip leapUseClip;
    public AudioClip leapAttackClip;
    private ChampBase target;
    private bool isLeap;

    private ChampBase talon;

    public override void Awake()
    {
        base.Awake();
        target = Zed.Instance.GetComponent<ChampBase>();
    }

    public override void Use(GameObject character)
    {
        if (!SubMP())
        {
            Release();
            return;
        }

        talon = character.GetComponent<ChampBase>();
        UseEffect(gameObject);          // 이펙트 사용
        StartSound(data.voiceClips);    // 시전 보이스 재생

        StartCoroutine(CoUse());  // 근접 스킬 사용 코루틴 시작
    }

    // 근접 스킬 사용 코루틴
    private IEnumerator CoUse()
    {
        var moveController = caster.GetComponent<CharacterMoveController>();
        Rigidbody rb = null;
        NavMeshAgent agent = null;

        if (moveController != null)
        {
            moveController.StopMove();                  // 시전자 이동 제한

            rb = moveController.GetRigidbody();
            rb.velocity = Vector3.zero;

            agent = moveController.GetAgent();
            agent.isStopped = true;
        }

        Vector3 targetPos = target.transform.position;
        if (Vector3.Distance(targetPos, caster.transform.position) > meleeAttackRange)
        {
            isLeap = true;
            SoundManager.Instance.PlayOneShot(leapUseClip);

            float startTime = Time.time;  // 이동 시작 시간
            float distanceToTarget = Vector3.Distance(transform.position, targetPos);
            while (distanceToTarget > meleeAttackRange)  // 목표 지점에 가까워질 때까지
            {
                targetPos = target.transform.position;
                distanceToTarget = Vector3.Distance(transform.position, targetPos);

                // 이동할 비율 계산 (시간에 비례)
                float distanceCovered = (Time.time - startTime) * data.speed;  // 이동한 거리
                float fractionOfJourney = distanceCovered / distanceToTarget;  // 여행 비율

                // 목표 지점으로 일정한 속도로 이동
                transform.position = Vector3.Lerp(transform.position, targetPos, fractionOfJourney);
                transform.LookAt(targetPos);

                caster.transform.position = Vector3.Lerp(caster.transform.position, targetPos, fractionOfJourney);
                caster.transform.LookAt(targetPos);

                yield return null;
            }
        }
        else
        {
            isLeap = false;
            caster.transform.LookAt(target.transform.position);
            SoundManager.Instance.PlayOneShot(meleeUseClip);
        }

        if (isLeap && caster.TryGetComponent<CharacterAnimationController>(out var animationController))
        {
            animationController.StartNextMotion();
        }

        yield return waitduration;  // 지속 시간만큼 대기

        if (moveController != null)
        {
            moveController.StartMove();
            
            if (rb != null)
                rb.velocity = Vector3.zero;

            if (agent != null)
            {
                agent.enabled = true;
                agent.Warp(agent.transform.position);
                if (agent.isActiveAndEnabled)
                {
                    agent.isStopped = false;
                }

                agent.SetDestination(target.transform.position);
            }
        }

        isLeap = false;
        Release();                  // 오브젝트 풀에 반납
    }

    // 데미지 처리
    public override IEnumerator DealDamage(ChampBase target, float damage, int hitRate)
    {
        int count = 0;
        while (count < hitRate)
        {
            target.OnDamage(damage);    // 타겟에 데미지 부여

            if (isLeap)
            {
                SoundManager.Instance.PlayOneShot(leapAttackClip);  // 사운드 매니저에서 타격 재생
            }

            UseHitEffect(target);
            count++;
            yield return hitInterval;
        }
    }

    private void Update()
    {
        transform.forward = talon.transform.forward;
        transform.position = talon.transform.position;
    }
}
