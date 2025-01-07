using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Jhin_R : Skill
{
    [SerializeField] private Skill defalutShot;
    [SerializeField] private Skill fourShot;
    [SerializeField] private float nextShotDelay;
    [SerializeField] private float startAnimationDelay;

    [SerializeField] private string shootingTriggerName;
    [SerializeField] private string shootingCountParamName;
    [SerializeField] private Effect teleportEffect;

    private List<Skill> createdDefalutShots;
    private Skill createdFourShot;

    private WaitForSeconds waitNextShot;
    private int maxShotCount = 3;
    private float yRotation = 30f;

    private bool isShooting = false;
    private Zed zed;

    public override void Awake()
    {
        zed = Zed.Instance;

        base.Awake();
        waitNextShot = new WaitForSeconds(nextShotDelay);

        createdDefalutShots = new();
        for (int i = 0; i < maxShotCount; i++)
        {
            var skill = Instantiate(defalutShot, gameObject.transform);
            skill.SetActive(false);
            createdDefalutShots.Add(skill);
        }

        createdFourShot = Instantiate(fourShot, gameObject.transform);
        createdFourShot.SetActive(false);
    }

    public override void Use(GameObject character)
    {
        if (!SubMP())
        {
            Release();
            return;
        }
        StartCoroutine(CoCurtainCall(character));
    }

    private IEnumerator CoCurtainCall(GameObject character)
    {
        var tpEffect_1 = EffectManager.Instance.UseEffect(teleportEffect, transform);
        Vector3 startPos = caster.transform.position;

        Jhin jhin = caster.GetComponent<Jhin>();
        var jhinAnimator = jhin.GetComponent<CharacterAnimationController>();
        var agent = jhin.GetComponent<NavMeshAgent>();

        // Move
        Map map = null;
        if (jhin != null && agent != null)
        {
            var eg = jhin.GetEnemyGenerator();
            map = eg.GetMap();
            var teleportTr = map.GetRandomTeleportTransform();
            var movedTr = teleportTr.GetMovedTransform();

            teleportTr.Teleport(agent);
            agent.SetDestination(movedTr.transform.position);
        }

        yield return new WaitForSeconds(data.useDelay);     // 시전 대기 시간만큼 대기

        // 사운드
        StartSound(data.useClips);      // 스킬 시전 사운드 재생
        StartSound(data.voiceClips);    // 시전 보이스 재생
        
        if (map != null)
        {
            map.SetActiveVirtualCam(true);
        }

        if (jhinAnimator != null)
        {
            jhinAnimator.StartNextMotion();
        }

        // 허리 숙여주고
        if (jhin != null)
        {
            var spine = jhin.GetSpine();
            spine.localRotation = Quaternion.Euler(-yRotation, 0, 0);
        }

        // 총 꺼내는 모션 기다리고
        yield return new WaitForSeconds(startAnimationDelay);

        // 사격
        isShooting = true;
        for (int i = 0; i <= maxShotCount; i++)
        {
            var shot = i != maxShotCount ? createdDefalutShots[i] : createdFourShot;
            Vector3 point = zed.shotStartTransform.position;
            var startPosition = jhin.GetShotTR().position;

            shot.SetPosition(startPosition);           // 스킬 시작 위치 지정
            shot.SetPoint(point);                      // 스킬 이동 위치 지정
            shot.SetCaster(caster);
            shot.SetActive(true);
            shot.Use(character);

            if (jhinAnimator != null)
            {
                jhinAnimator.SetTrigger(shootingTriggerName);
                jhinAnimator.SetInteger(shootingCountParamName, i);
            }

            yield return waitNextShot;
        }

        isShooting = false;

        if (jhinAnimator != null)
        {
            jhinAnimator.StartNextMotion();
        }

        // 허리 펴주고
        if (jhin != null)
        {
            var spine = jhin.GetSpine();
            spine.localRotation = Quaternion.Euler(yRotation, 0, 0);
        }

        yield return waitNextShot;

        if (map != null)
        {
            map.SetActiveVirtualCam(false);
        }

        caster.transform.position = startPos;
        if (agent != null)
        {
            agent.Warp(startPos);
        }

        var tpEffect_2 = EffectManager.Instance.UseEffect(teleportEffect, transform);
        yield return new WaitForSeconds(tpEffect_2.GetDuration());

        if (tpEffect_1 != null)
        {
            tpEffect_1.Stop();
            EffectManager.Instance.ReleaseEffect(tpEffect_1);
        }

        if (tpEffect_2 != null)
        {
            tpEffect_1.Stop();
            EffectManager.Instance.ReleaseEffect(tpEffect_2);
        }

        Release();
    }

    private void Update()
    {
        if (zed != null && isShooting)
        {
            Vector3 point = zed.shotStartTransform.position;
            caster.transform.LookAt(point);
        }
    }
}
