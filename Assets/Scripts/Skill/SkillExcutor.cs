using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class SkillExcutor : MonoBehaviour
{
    [SerializeField] private SkillCoolDown cooldownImage;
    private IObjectPool<Skill> skillPool;
    private GameObject poolObject;          // 생성된 스킬들을 담아둘 부모 오브젝트
    private SkillButtonData data;           // 스킬 데이터
    public static int shadowID = 0;         // 플레이어의 그림자 스킬 전용 ID
    private Skill coolDownSkill;            // 쿨다운 중인 스킬
    private bool isAvailable = true;        // 스킬 사용 가능 여부
    protected SkillIndicator indicator;

    // 초기 설정
    public void Init(GameObject parentObj, SkillButtonData data)
    {
        if (data == null)   // 스킬 데이터를 받지 못할 경우 return
            return;

        this.data = data;   // 스킬 데이터 설정
        poolObject = new GameObject($"{data.skill.data.skillName}_Pool");   // 생성된 스킬들을 담아둘 부모 오브젝트 생성
        poolObject.transform.parent = parentObj.transform;                  // 스킬들의 부모 오브젝트를 parentObj의 하위로 이동

        if (data.maxPoolSize <= 0)  // 스킬 데이터의 최대 pool size가 0 이하일 경우 return (오브젝트 풀 생성 X)
            return;

        // 스킬 오브젝트 풀 생성
        skillPool = new ObjectPool<Skill>
                    (
                        CreateSkill,
                        GetSkill,
                        ReleaseSkill,
                        DestroySkill,
                        maxSize : data.maxPoolSize
                    );

        if (data.skill.data.indicator != null)
        {
            indicator = Instantiate(data.skill.data.indicator, gameObject.transform);
        }
    }

    public void SetParentInExcutor()
    {
        poolObject.transform.SetParent(gameObject.transform);
    }

    // 스킬 실행 
    // 실행한 스킬을 return (해당 스킬을 실행한 곳에서도 실행한 스킬의 정보를 알 수 있게)
    public Skill StartSkill(GameObject character, int enumIndex, string layerMask)
    {
        if (!isAvailable)   // 사용 불가 상태일 경우 null return
            return null;

        if (data == null)
            return null;

        Vector3 point = Vector3.zero;                                       // 실행된 스킬이 이동할 위치
        if (character.tag == EnumConverter.GetString(CharacterEnum.Enemy))  // 인자로 받은 character 오브젝트의 태그가 Enemy일 경우
            point = Zed.Instance.gameObject.transform.position;             // 플레이어의 위치로 이동
        else                                                                // 태그가 Enemy가 아닐 경우
            point = Raycast.GetMousePointVec();                             // 현재 마우스 위치로 이동 (플레이어가 발사하는 스킬)

        point.y = character.transform.position.y;   // 설정한 위치의 Y값을 character 오브젝트의 Y값으로 변경 (위, 아래로 이동하지 않게)

        var useSkill = skillPool.Get();             // 오브젝트 풀에서 스킬 가져오기
        useSkill.SetCaster(character);              // 시전자 설정
        if (useSkill.data.type == SkillType.Dash)   // 스킬 타입이 대쉬일 경우
        {
            var dashSkill = useSkill.GetComponent<DashSkill>(); // 가져온 스킬에서 대쉬 스킬 컴포넌트 추출
            if (dashSkill == null)      // 대쉬 스킬이 아닐 때
                return null;            // null return (잘못 설정된 스킬)

            if (character.TryGetComponent<CharacterAnimationController>(out var animationController))
            {
                animationController.UseSkill(enumIndex, data.isUpper);
            }

            isAvailable = false;            // 스킬 사용 불가 상태로 변경 (오동작 방지)
            dashSkill.SetPoint(point);      // 대쉬할 위치 설정
            dashSkill.Use(character);       // 대쉬 스킬 사용
            UseIndicator(character, useSkill.data.useDelay); // 스킬 범위 표시기 사용
            coolDownSkill = dashSkill;      // 쿨다운 스킬 설정
            StartCoroutine(CoCoolDown());   // 쿨다운 코루틴 시작
            return dashSkill;
        }

        if (useSkill.isTargeting)                   // 스킬이 타게팅 스킬일 경우
        {
            if (!useSkill.data.isSelf)
            {
                GameObject target = null;
                if (character.tag == EnumConverter.GetString(CharacterEnum.Player))
                {
                    var hit = Raycast.GetHit(Input.mousePosition, layerMask);  // 현재 마우스 위치의 적 탐지, 타겟 정보 가져오기
                    if (hit.collider != null)
                    {
                        target = hit.collider.gameObject;
                    }

                }
                else if (character.tag == EnumConverter.GetString(CharacterEnum.Enemy))
                {
                    target = Zed.Instance.gameObject;
                }

                if (!target)
                {
                    skillPool.Release(useSkill);
                    return null;
                }
                else
                {
                    var targeting = useSkill.GetComponent<ITargetable>();  // 가져온 스킬에서 타게팅 추출
                    targeting.SetTarget(target);                           // 타겟 설정
                }
            }  
            else
            {
                if (!character)
                {
                    skillPool.Release(useSkill);
                    return null;
                }
                else
                {
                    var targeting = useSkill.GetComponent<ITargetable>();   // 가져온 스킬에서 타게팅 추출
                    targeting.SetTarget(character);                         // 타겟 설정
                }
            }
        }

        StartCoroutine(WaitUseSkill(useSkill, character, point, enumIndex));   // 스킬 사용 대기 코루틴 시작
        return useSkill;    // 스킬 return
    }

    // 스킬 사용 대기 코루틴
    // 스킬 데이터의 대기 시간만큼 대기, 대기 완료 후 스킬 실행
    private IEnumerator WaitUseSkill(Skill useSkill, GameObject character, Vector3 lookAtPoint, int enumIndex)
    {
        isAvailable = false;                                        // 스킬 사용 불가 상태로 변경 (오동작 방지)
        useSkill.SetActive(false);                                  // 스킬 오브젝트의 active 꺼놓기 (켜져 있을 경우 보이기 때문)
        character.transform.LookAt(lookAtPoint);                    // 캐릭터를 lookAtPoint로 바라보게 설정

        UseIndicator(character, useSkill.data.useDelay); // 스킬 범위 표시기 사용

        var moveController = character.GetComponent<CharacterMoveController>();
        var agent = character.GetComponent<NavMeshAgent>();
        if (useSkill.data.isStiffen)              // 사용 전 경직이 활성화 되어 있을 경우
        {
            if (moveController != null)
            {
                moveController.StopMove();
            }
            else if (agent != null)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }
        }

        if (useSkill.data.useDelay > 0)
            yield return new WaitForSeconds(useSkill.data.useDelay);            // 스킬 데이터의 대기 시간만큼 대기

        if (character.TryGetComponent<CharacterAnimationController>(out var animationController))
        {
            // 탈론
            if (useSkill.TryGetComponent<Talon_Q>(out var talon_Q))
            {
                if (animationController.HasParameter("IsLeap"))
                {
                    var distance = Vector3.Distance(character.transform.position, Zed.Instance.transform.position);
                    if (distance > talon_Q.meleeAttackRange)
                    {
                        animationController.SetBool("IsLeap", true);
                    }
                    else
                    {
                        animationController.SetBool("IsLeap", false);
                    }
                }
            }
            else if(character.TryGetComponent<Gangplank>(out var gangplank))
            {
                var animQuarter = Random.Range(0, gangplank.spell_1_Animation_Quarter);
                animationController.SetInteger(animationController.spell_Anim_Quarter_ParamName, animQuarter);
            }

            animationController.UseSkill(enumIndex, data.isUpper);        // 애니메이션 출력
        }

        // 애니메이션 전환 후 평타 애니메이션의 애니메이션 이벤트가 발생하지 않게 하기 위함 (트랜지션 대기 1프레임)
        yield return null;
        if (character.TryGetComponent<ChampBase>(out var champ))
        {
            champ.FinishedAttack();
        }

        Vector3 startPosition = character.gameObject.transform.position;    // 시작 위치 설정 (character 오브젝트의 현재 위치)
        if (character.TryGetComponent(out ChampBase champion))              // character 오브젝트에서 ChampBase 컴포넌트 추출 성공 시
        {
            startPosition = champion.shotStartTransform.position;           // 스킬 시작 위치를 캐릭터에 설정된 발사 시작 위치로 변경
        }

        useSkill.SetActive(true);                           // 스킬 active 활성화
        useSkill.SetPosition(startPosition);                // 스킬 위치 설정
        useSkill.SetStartPos(startPosition);                // 스킬 시작 위치 설정
        useSkill.SetRotation(character.transform.rotation); // 스킬 회전 값 설정

        character.transform.LookAt(lookAtPoint);                    // 캐릭터를 lookAtPoint로 바라보게 설정

        // 그림자 스킬이고,
        // character 오브젝트에서 플레이어 컴포넌트 추출을 성공했고,
        // 스킬에서 그림자 스킬 컴포넌트 추출을 성공했을 경우
        if (data.skill.data.isShadow && character.TryGetComponent(out Zed zed) && useSkill.TryGetComponent(out ZedShadow shadow))
        {
            useSkill.SetPosition(character.transform.position); // 스킬 위치 설정 (캐릭터의 위치로)
            useSkill.SetPoint(Raycast.GetMousePointVec());

            if (shadow.GetID() <= 0)    // 그림자 ID가 0 이하일 경우
            {
                shadowID++;             // 그림자 ID ++
                shadow.SetID(shadowID); // 그림자 스킬 ID 설정
            }

            zed.AddShadow(shadow);      // 그림자 스킬을 플레이어에 추가
        }
        else
        {
            useSkill.SetPoint(character.transform.forward);
        }

        useSkill.Use(character);        // 스킬 실행
        coolDownSkill = useSkill;       // 쿨다운 스킬 설정
        StartCoroutine(CoCoolDown());   // 쿨다운 코루틴 실행

        if (moveController != null)
        {
            moveController.StopMove();
        }

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        yield return new WaitForSeconds(useSkill.data.afterUseStiffenTime);    // 스킬 데이터의 사용 후 경직 시간만큼 대기

        if (character.gameObject.activeSelf)
        {
            if (moveController != null)
                moveController.isMoved = true;
            if (agent != null && agent.enabled && agent.isOnNavMesh)
                agent.isStopped = false;
        }
    }

    // 쿨다운 코루틴
    private IEnumerator CoCoolDown()
    {
        if (cooldownImage != null)
        {
            cooldownImage.StartCoolDown(coolDownSkill.data.coolDown);
        }

        yield return new WaitForSeconds(coolDownSkill.data.coolDown);   // 스킬 데이터의 쿨다운 시간만큼 대기
        coolDownSkill = null;   // 쿨다운 코루틴 초기화
        isAvailable = true;     // 스킬 사용 가능 상태로 변경
    }

    protected void UseIndicator(GameObject character, float duration)
    {
        if (indicator == null)
            return;

        // 캐릭터의 forward로 Y와 Z 회전값 계산
        Quaternion targetRotation = Quaternion.LookRotation(character.transform.forward, Vector3.up);
        Vector3 targetEulerAngles = targetRotation.eulerAngles;

        // X 회전값 유지, Y와 Z는 캐릭터의 회전값 가져오기
        var eulerAngles = new Vector3(indicator.transform.eulerAngles.x, targetEulerAngles.y, targetEulerAngles.z);

        // 위치 가져오기
        var pos = character.transform.localPosition;

        indicator.SetPosition(pos);
        indicator.SetTarget(character);
        indicator.transform.eulerAngles = eulerAngles;
        indicator.duration = duration;
        indicator.Use();
    }

    // 오브젝트 풀의 Create 
    private Skill CreateSkill()
    {
        if (data == null)
            return null;

        var useSkill = Instantiate(data.skill, poolObject.transform);   // 스킬 생성
        useSkill.SetPool(skillPool);    // 스킬에 오브젝트 풀 설정 (release 시 설정한 풀에 반납)

        return useSkill;
    }

    // 오브젝트 풀의 Get 
    private void GetSkill(Skill skill)
    {
        skill.gameObject.SetActive(true);
    }

    // 오브젝트 풀의 Release 
    private void ReleaseSkill(Skill skill)
    {
        skill.gameObject.SetActive(false);
    }

    // 오브젝트 풀의 Destroy 
    private void DestroySkill(Skill skill)
    {
        skill.KillTween();
        Destroy(skill.gameObject);
    }

    public SkillButtonData GetData()
    {
        return data;
    }

    public void DestroyPool()
    {
        Destroy(poolObject);
        poolObject = null;
    }

    public bool GetIsAvailable() { return isAvailable; }
    public void SetIsAvailable(bool set) { isAvailable = set; }
    public IObjectPool<Skill> GetPool() { return skillPool; }
    public void ResetExcutor()
    {
        skillPool.Clear();

        if (poolObject != null)
            Destroy(poolObject.gameObject);

        if (indicator != null)
            Destroy(indicator.gameObject);

        data = null;
        coolDownSkill = null;
        isAvailable = true;
    }
}
