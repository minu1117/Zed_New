using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MultipleShadow : Skill
{
    public int shadowCount;                 // 생성할 그림자 개수
    public ZedShadow shadowSkill;           // 그림자 스킬
    public float interval;                  // 간격
    public float individualWaitDelay;       // 생성 간격 (시간 간격)
    private WaitForSeconds waitDelay;
    private IObjectPool<Skill> shadowPool;

    public override void Awake()
    {
        base.Awake();
        data.duration = shadowSkill.data.duration + 1f;         // 지속 시간 설정 (그림자 스킬보다 1초 많게 -> 그림자가 모두 사라지기 전에 오브젝트가 꺼지면 오동작 할 수 있기 때문)
        waitDelay = new WaitForSeconds(individualWaitDelay);    // 생성 간격 (시간 간격) 캐싱

        shadowPool = new ObjectPool<Skill>
                    (
                        CreateShadow,
                        GetShadow,
                        ReleaseShadow,
                        DestroyShadow,
                        maxSize : shadowCount
                    );
    }

    // 스킬 사용
    public override void Use(GameObject character)
    {
        base.Use(character);
        StartCoroutine(CoSpawnMultipleShadow(character));   // 다중 그림자 생성 코루틴 실행
    }

    // 다중 그림자 생성 코루틴
    private IEnumerator CoSpawnMultipleShadow(GameObject character)
    {
        List<ZedShadow> shadows = new List<ZedShadow>();    // 그림자 목록
        for (int i = 0; i < shadowCount; i++)               // 생성할 그림자 개수만큼 순회
        {
            var shadow = shadowPool.Get();                  // 오브젝트 풀에서 그림자 가져오기
            shadows.Add(shadow as ZedShadow);               // 그림자 목록에 추가
        }

        yield return new WaitForSeconds(shadowSkill.data.useDelay);     // 시전 대기 시간만큼 대기

        // character 오브젝트에서 Zed 컴포넌트(플레이어) 추출을 성공했을 경우
        if (character.TryGetComponent(out Zed zed))
        {
            var lookPos = zed.transform.position;

            Vector3 startPosition = character.gameObject.transform.position;    // 시작 위치
            Vector3 startDirection = character.gameObject.transform.forward;    // 시작 방향

            for (int i = 0; i < shadows.Count; i++)                             // 그림자 목록 순회
            {
                float angle = (360f / shadowCount) * i;                         // 각도 계산
                Vector3 rotatedDirection = Quaternion.AngleAxis(angle, Vector3.up) * startDirection;    // 회전 방향 계산
                Vector3 point = startPosition + rotatedDirection * interval;    // 생성 지점 계산

                shadows[i].SetPosition(startPosition);                          // 그림자 스킬 위치 지정
                shadows[i].SetPoint(point);                                     // 이동할 위치 지정
                zed.AddShadow(shadows[i]);                                      // 플레이어에 그림자 스킬 추가
            }

            foreach (var shadow in shadows) // 그림자 목록 순회
            {
                shadow.SetLookAtPoint(lookPos);
                shadow.SetActive(true);     // 그림자 활성화
                shadow.FreeUse(character);      // 그림자 스킬 사용

                yield return waitDelay;     // 생성 간격만큼 대기
            }
        }

        yield return new WaitForSeconds(data.duration); // 지속 시간만큼 대기

        Release();  // 오브젝트 풀에 반납
    }

    // 오브젝트 풀의 Create
    private Skill CreateShadow()
    {
        var shadow = Instantiate(shadowSkill, gameObject.transform);    // 그림자 생성
        int id = ++SkillExcutor.shadowID;                               // 그림자의 ID 값
        shadow.SetID(id);                                               // ID 지정
        shadow.SetPool(shadowPool);

        return shadow;
    }

    // 오브젝트 풀의 Get
    private void GetShadow(Skill shadow)
    {
        shadow.gameObject.SetActive(false);
    }

    // 오브젝트 풀의 Release
    private void ReleaseShadow(Skill shadow)
    {
        shadow.gameObject.SetActive(false);
    }

    // 오브젝트 풀의 Destroy
    private void DestroyShadow(Skill shadow)
    {
        Destroy(shadow.gameObject);
    }
}
