using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MultipleSkill : Skill
{
    public int count;                       // 생성할 스킬 개수
    public Skill skillPrefab;               // 스킬
    public float individualWaitDelay;       // 시전 간격 (시간 간격)
    public float angle;                     // 각도
    public bool isGround;                   // 바닥에서 시전할 것인지 여부
    private WaitForSeconds waitDelay;
    private IObjectPool<Skill> skillPool;

    public override void Awake()
    {
        base.Awake();
        data.duration = skillPrefab.data.duration + 1f;         // 지속 시간 설정 (스킬보다 1초 많게 -> 스킬이 모두 사라지기 전에 오브젝트가 꺼지면 오동작 할 수 있기 때문)

        if (individualWaitDelay > 0f)
        {
            waitDelay = new WaitForSeconds(individualWaitDelay);    // 시전 간격 (시간 간격) 캐싱
        }

        skillPool = new ObjectPool<Skill>
                    (
                        CreateSkill,
                        GetSkill,
                        ReleaseSkill,
                        DestroySkill,
                        maxSize: count
                    );
    }

    // 스킬 사용
    public override void Use(GameObject character)
    {
        base.Use(character);
        StartCoroutine(CoUseMultipleSkill(character));
    }

    // 다중 그림자 생성 코루틴
    private IEnumerator CoUseMultipleSkill(GameObject character)
    {
        var champBase = character.GetComponent<ChampBase>();
        var zedShadow = character.GetComponent<ZedShadow>();

        List<Skill> skills = new();
        for (int i = 0; i < count; i++)
        {
            var skill = skillPool.Get();
            skills.Add(skill);
        }

        yield return new WaitForSeconds(skillPrefab.data.useDelay);     // 시전 대기 시간만큼 대기

        Vector3 startPosition = character.gameObject.transform.position;
        if (!isGround)
        {
            if (champBase != null)
            {
                startPosition = champBase.shotStartTransform.position;
            }
            else if (zedShadow != null)
            {
                startPosition = zedShadow.shotStartTransform.position;
            }
        }

        Vector3 startDirection = character.gameObject.transform.forward;    // 시작 방향

        for (int i = 0; i < skills.Count; i++)                              // 스킬 목록 순회
        {
            float angleAxis = (i - (count - 1) / 2f) * (angle / count);     // 각도 계산 (중심 기준)
            Quaternion rotation = Quaternion.AngleAxis(angleAxis, Vector3.up);
            Vector3 point = rotation * startDirection;

            skills[i].SetPosition(startPosition);           // 스킬 시작 위치 지정
            skills[i].SetPoint(point);                      // 스킬 이동 위치 지정
        }

        foreach (var skill in skills)   // 스킬 목록 순회
        {
            skill.SetActive(true);      // 스킬 활성화
            skill.SetCaster(caster);
            skill.Use(character);       // 스킬 사용

            if (waitDelay != null)
            {
                yield return waitDelay;     // 생성 간격만큼 대기
            }
        }

        yield return new WaitForSeconds(data.duration); // 지속 시간만큼 대기

        Release();  // 오브젝트 풀에 반납
    }

    // 오브젝트 풀의 Create
    private Skill CreateSkill()
    {
        var skill = Instantiate(skillPrefab, gameObject.transform);
        skill.SetPool(skillPool);

        return skill;
    }

    // 오브젝트 풀의 Get
    private void GetSkill(Skill skill)
    {
        skill.gameObject.SetActive(false);
    }

    // 오브젝트 풀의 Release
    private void ReleaseSkill(Skill skill)
    {
        skill.gameObject.SetActive(false);
    }

    // 오브젝트 풀의 Destroy
    private void DestroySkill(Skill skill)
    {
        Destroy(skill.gameObject);
    }
}
