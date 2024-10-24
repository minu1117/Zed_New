using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public struct UseSkillData
{
    public IObjectPool<Skill> skillPool;    // 스킬의 오브젝트 풀 (Release를 하기 위함)
    public Skill skill;                     // 사용할 스킬
    public ZedSkillType skillType;          // 애니메이션 타입
    public GameObject target;               // 타겟
    public bool isUpper;
}

// 플레이어 전용 스킬
public class ZedShadow : ShotSkill
{
    private readonly float moveTime = 0.5f;             // 목표 지점까지 이동하는 시간
    private int objectID;                               // 그림자 스킬 고유 ID

    public bool isReady = false;                        // 스킬 사용 시작 가능 여부
    public Transform shotStartTransform;                // 스킬 발사 위치
    public SkinnedMeshRenderer meshRenderer;
    public List<TrailRenderer> weapontrailRenderers;

    private NavMeshAgent agent;
    private Rigidbody rb;
    private Vector3 usePoint;                           // 이동 목표 지점
    private Vector3 lookAtPoint;

    private Dictionary<string, List<UseSkillData>> useSkills;   // 플레이어가 사용 시 복제하여 사용할 스킬들
    private CharacterAnimationController animationController;
    [SerializeField] private GameObject particleFollowObj;      // 이동 중(시전 중) 나올 파티클

    public override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animationController = GetComponent<CharacterAnimationController>();
        useSkills = new();
    }

    public NavMeshAgent GetAgent() { return agent; }

    // 그림자 스킬 실행
    public override void Use(GameObject character)
    {
        if (!character.TryGetComponent(out Zed zed))    // character 오브젝트에서 플레이어 컴포넌트 추출 실패 시 (플레이어 전용 스킬)
            return;

        UseEffect(particleFollowObj);           // 이펙트 실행
        StartSound(data.useClips);              // 스킬 사용 사운드 재생
        meshRenderer.enabled = false;           // 이동 중 오브젝트가 보이지 않기 위해 비활성화
        SetActiveTrailRenderer(true);           // TrailRenderer 활성화
        SetActiveWeaponTrailRenderers(false);   // 무기 전용 TrailRenderer 활성화
        StartCoroutine(CoSpawnShadow(zed));     // 이동 목표 지점까지 이동하는 코루틴 실행
    }

    // 무기 전용 TrailRenderer 활성화 
    private void SetActiveWeaponTrailRenderers(bool active)
    {
        if (weapontrailRenderers == null || weapontrailRenderers.Count == 0)    // TrailRenderer가 없을 경우 return
            return;

        // TrailRenderer List 순회
        foreach (var trailRenderer in weapontrailRenderers)
        {
            trailRenderer.enabled = active; // 활성화
        }
    }

    // 이동 목표 지점까지 이동하는 코루틴
    // 이동 완료 후, 이동 중 플레이어가 사용한 모든 스킬 사용
    private IEnumerator CoSpawnShadow(Zed zed)
    {
        if (usePoint == null || usePoint == Vector3.zero)   // 이동 목표 지점이 null이거나 0,0,0일 경우
            usePoint = Raycast.GetMousePointVec();          // 목표 지점을 현재 마우스 위치로 설정

        agent.enabled = false;                              // 이동 중 벽에 부딪히지 않기 위해 비활성화
        transform.forward = new Vector3(usePoint.x, transform.position.y, usePoint.z);  // 바라보는 위치 설정

        yield return new WaitForSeconds(data.useDelay);     // 시전 딜레이 만큼 대기

        //if (tweener != null)
        //    tweener.Kill();

        if (tweener == null)
        {
            tweener = transform.DOMove(usePoint, moveTime)         // 목표 지점까지 moveTime 안에 도착
                    .SetEase(Ease.OutQuad)                     // 속도가 빠르게 시작, 점차 감소
                    .SetAutoKill(false)
                    .OnComplete(() => UseAllSkills());         // 이동 완료 후, 이동 중 플레이어가 사용한 모든 스킬 사용
        }
        else
        {
            RestartTween(transform.position, usePoint);
        }

        yield return new WaitForSeconds(data.duration);     // 지속시간 만큼 대기

        usePoint = Vector3.zero;                            // 목표 지점 초기화
        zed.RemoveShadow(objectID);                         // 플레이어에 담겨있는 그림자 스킬 제거 (본인)
        useSkills.Clear();                                  // 담아둔 모든 스킬 제거

        if (pool != null)                           // 그림자 스킬 오브젝트 풀이 있을 경우
        {
            isReady = false;                        // 준비 상태 초기화
            agent.enabled = true;                   // NavMeshAgent 활성화
            SetActiveWeaponTrailRenderers(false);   // 무기 TrailRenderer 비활성화
            Release();                              // 오브젝트 풀에 Release
        }
        else                                        // 오브젝트 풀이 없을 경우
        {
            StartDisappearSound();                  // 사라지는 사운드 재생
            Destroy(gameObject);                    // 삭제
        }
    }

    // 이동 완료 후 실행
    // 담아둔 모든 스킬 사용
    public void UseAllSkills()
    {
        if (lookAtPoint != Vector3.zero)
            transform.LookAt(lookAtPoint);

        ReleaseEffect();                        // 이동 중 나오는 파티클 Release
        SetActiveWeaponTrailRenderers(true);    // 무기 TrailRenderer 활성화

        meshRenderer.enabled = true;            // 이동이 완료되어 오브젝트가 보여야 하니 활성화
        isReady = true;                         // 스킬 사용 준비 완료
        usePoint = GetUsePoint();               // 현재 마우스 위치 저장

        if (useSkills.Count == 0)
            return;

        // 이동 중 담아둔 사용될 스킬들 순회
        foreach (var skillPairList in useSkills)
        {
            foreach (var skillObject in skillPairList.Value)
            {
                UseCopySkill(skillObject.skill, skillObject.skillPool, (int)skillObject.skillType, skillObject.isUpper, skillObject.target);  // 스킬 사용
                //StartAnimation(skillObject.skillType); // 애니메이션 실행
            }
        }

        useSkills.Clear();  // 사용 완료, 담아둔 모든 스킬 삭제
    }

    // 이동 중 사용된 스킬들 담기
    public void AddSkill(string name, Skill skill, ZedSkillType type, SkillExcutor excutor, GameObject target = null)
    {
        if (skill == null)  // 스킬이 없을 경우 return
            return;

        if (isReady)        // 스킬 사용 준비가 됐을 경우 스킬 바로 사용
        {
            usePoint = GetUsePoint();                   // 현재 마우스 위치 저장
            UseCopySkill(skill, excutor.GetPool(), (int)type, excutor.GetData().isUpper, target);     // 스킬 사용
            //StartAnimation(type);                       // 애니메이션 실행
            return;                                     // 사용 후 return
        }

        // 사용할 스킬 정보와 설정한 타겟 담기
        var skilldata = new UseSkillData
        {
            skill = skill,
            skillPool = excutor.GetPool(),
            skillType = type,
            target = target,
            isUpper = excutor.GetData().isUpper
        };

        // 대쉬 스킬일 경우
        if (skill.data.type == SkillType.Dash)
        {
            Vector3 point = Raycast.GetMousePointVec(); // 현재 마우스 위치 저장 (바닥 기준)
            var dash = skill.GetComponent<DashSkill>(); // 스킬에서 대쉬 스킬 컴포넌트 추출
            dash.SetCaster(gameObject);                 // 시전자 설정
            dash.SetPoint(point);                       // 시전 지점 설정
        }

        // 스킬들을 담아둔 곳에 같은 스킬이 없을 경우
        if (!useSkills.ContainsKey(name))
        {
            // 새로운 스킬 정보 컨테이너 생성, 사용할 스킬 담기
            List<UseSkillData> useSkillDatas = new List<UseSkillData>{skilldata};
            useSkills.Add(name, useSkillDatas);  // 새로 사용할 스킬 추가
        }
        // 같은 스킬이 있을 경우
        else
        {
            useSkills[name].Add(skilldata);  // 해당 컨테이너를 찾아 스킬 추가
        }
    }

    // 애니메이션 실행 
    private void StartAnimation(ZedSkillType type)
    {
        int typeToint = (int)type;
        bool isUpperLayer = typeToint != (int)ZedSkillType.ShadowRush ? true : false;
        animationController.UseSkill(typeToint, isUpperLayer);
    }

    // 스킬 사용 
    private void UseCopySkill(Skill skill, IObjectPool<Skill> skillPool, int enumIndex, bool isUpper, GameObject target = null)
    {
        StartCoroutine(CoUseCopySkill(skill, skillPool, enumIndex, isUpper, target));
    }

    // 스킬 사용 코루틴
    private IEnumerator CoUseCopySkill(Skill skill, IObjectPool<Skill> skillPool, int enumIndex, bool isUpper, GameObject target = null)
    {
        if (skill.data.isShadow)    // 그림자 스킬일 경우 중단
            yield break;

        yield return new WaitForSeconds(skill.data.useDelay);   // 스킬의 선 딜레이 만큼 대기

        transform.LookAt(usePoint);             // 시전 위치 바라보기

        if (animationController != null)
            animationController.UseSkill(enumIndex, isUpper);        // 애니메이션 출력

        var skillObject = skillPool.Get();      // 스킬 가져오기

        if (skillObject.isTargeting)            // 타게팅 스킬일 경우
        {
            if (target == null)                 // 타겟이 없을 경우
            {
                skillPool.Release(skillObject); // 스킬 제거 (Release)
            }
            else                                // 타게팅 스킬이 아닐 경우
            {
                var targetingSkill = skillObject.GetComponent<TargetingSkill>();    // 스킬에서 타게팅 스킬 컴포넌트 추출
                targetingSkill.SetTarget(target);                                   // 타겟 설정
            }
        }

        skillObject.SetCaster(gameObject);                      // 시전자 설정
        skillObject.SetPool(skillPool);                         // 오브젝트 풀 설정 (Release 하기 위함)
        skillObject.SetPosition(shotStartTransform.position);   // 스킬 위치 설정
        skillObject.SetStartPos(shotStartTransform.position);   // 시작 위치 설정
        skillObject.SetRotation(transform.rotation);            // 회전 값 설정

        skillObject.Use(gameObject);                            // 스킬 사용
    }

    // 순간이동
    // 인자로 받는 GameObject와 위치, 회전 값을 서로 변경
    public void Teleport(GameObject obj)
    {
        Vector3 position = obj.transform.position;
        Vector3 shadowPosition = transform.position;
        Quaternion rotation = obj.transform.rotation;
        Quaternion shadowRotation = transform.rotation;

        obj.transform.position = shadowPosition;
        transform.position = position;
        obj.transform.rotation = shadowRotation;
        transform.rotation = rotation;

        var recastClips = data.recastClips;                 // 재시전 사운드
        if (recastClips == null || recastClips.Count == 0)  // 재시전 사운드가 없을 경우 return
            return;

        SoundManager.Instance.PlayOneShot(recastClips[UnityEngine.Random.Range(0, recastClips.Count)]); // 재시전 사운드 실행
    }

    private Vector3 GetUsePoint()
    {
        Vector3 point = Raycast.GetMousePointVec();
        point.y = transform.position.y;
        return point;
    }

    public void SetLookAtPoint(Vector3 lookAtPoint) { this.lookAtPoint = lookAtPoint; }
    public void SetPoint(Vector3 point) { usePoint = point; }
    public void SetID(int id) { objectID = id; }
    public int GetID() { return objectID; }
}
