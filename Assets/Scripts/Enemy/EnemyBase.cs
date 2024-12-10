using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System.Collections;

public enum EnemySkill
{
    None = -1,

    Skill_1,
    Skill_2,
    Skill_3,
    Skill_4,
    Skill_5,

    Count,
}

public abstract class EnemyBase : ChampBase
{
    protected enum AttackMode
    {
        Normal,
        Skill,
        Combo,
        Pattern,
    }

    protected enum State
    {
        Patrol,
        Chase,
        Attack,
    }

    public float recognitionRange;          // 타겟 인식 범위
    public float attackRange;               // 일반 공격 범위
    public float skillRange;                // 스킬 사용 범위
    public float loseTargetTime;            // 타겟 해제 시간
    public float patrolRange;               // 정찰 범위

    protected Rigidbody rb;
    protected GameObject target;
    protected GameObject player;
    private IObjectPool<EnemyBase> pool;

    protected State state;

    protected List<string> skillKeys;
    private Coroutine loseTargetCoroutine;
    private Coroutine patrolCoroutine;

    [SerializeField] private float addRunSpeed = 5f;
    protected float runSpeed;
    private string moveAnimControllParam = "Speed";

    [SerializeField] protected float waitSkillDelay;
    [SerializeField] protected float waitAttackDelay;
    [SerializeField] protected Effect spawnEffect;
    protected bool isAttack;

    protected Coroutine waitNextAttackCoroutine;
    protected AttackMode attackMode;
    protected EnemyGenerator enemyGenerator;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();

        if (slot != null)
        {
            var skillSlot = slot.GetSlotDict();
            if (skillSlot != null && skillSlot.Count > 0)
            {
                skillKeys = new();  // 스킬 키 List 초기화
                foreach (var skillButton in skillSlot) // 스킬 슬롯 순회, 스킬 키 List에 key 추가
                {
                    skillKeys.Add(skillButton.Key);
                }
            }
        }
    }

    // 초기 설정
    public virtual void Init()
    {
        agent.isStopped = false;
        agent.speed = data.moveSpeed;
        runSpeed = data.moveSpeed + addRunSpeed;
        player = Zed.Instance.gameObject;
    }

    public virtual void ResetEnemy() 
    {
        if (coll != null)
        {
            coll.enabled = true;
        }

        animationController.Restart();
        agent.isStopped = false;
        agent.speed = data.moveSpeed;
        runSpeed = data.moveSpeed + addRunSpeed;
    }

    public virtual void Update()
    {
        MoveAnimation();
    }

    protected void MoveAnimation()
    {
        float speed = agent.velocity.magnitude;
        animationController.SetFloat(moveAnimControllParam, speed);
    }

    // 타겟 설정
    public void SetTarget(GameObject targetObj)
    {
        target = targetObj;
    }

    public override IEnumerator OnDead()
    {
        if (coll != null)
        {
            coll.enabled = false;
        }

        if (loseTargetCoroutine != null)
        {
            StopCoroutine(loseTargetCoroutine);
            loseTargetCoroutine = null;
        }

        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }

        if (waitNextAttackCoroutine != null)
        {
            StopCoroutine(waitNextAttackCoroutine);
            waitNextAttackCoroutine = null;
        }

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }

        if (animationController != null)
        {
            animationController.Dead();
            yield return new WaitForSeconds(deadAnimDuration);
        }

        if (deadEffect != null)
        {
            EffectManager.Instance.UseEffect(deadEffect, gameObject.transform, true, true);
        }

        // 오브젝트 풀이 설정된 경우
        if (pool != null)
        {
            if (slot != null)
            {
                var skillSlot = slot.GetSlotDict();
                if (skillSlot != null && skillSlot.Count > 0)
                {
                    // 모든 스킬을 사용 가능한 상태로 변경
                    foreach (var slot in skillSlot)
                    {
                        slot.Value.SetIsAvailable(true);
                    }
                }
            }

            if (enemyGenerator != null)
            {
                enemyGenerator.SubEnemyCount();
            }
            target = null;          // 타겟 해제
            pool.Release(this);     // 오브젝트 풀에 본인 반납
        }

        // 오브젝트 풀에 들어가지 않았을 경우
        else
            Destroy(gameObject);    // 그냥 삭제
    }

    // 오브젝트 풀 설정 (죽었을 때 반납용)
    public void SetPool(IObjectPool<EnemyBase> enemyPool)
    {
        pool = enemyPool;
    }

    protected void UseAutoAttack()
    {
        transform.LookAt(player.transform);
        AutoAttack();
    }

    protected void StopMove()
    {
        agent.isStopped = true;
        rb.velocity = Vector3.zero;
        agent.velocity = Vector3.zero;
    }

    protected void WaitNextAttack(float duration)
    {
        if (waitNextAttackCoroutine != null)
            return;

        waitNextAttackCoroutine = StartCoroutine(CoWaitNextAttack(duration));
    }

    protected IEnumerator CoWaitNextAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
        waitNextAttackCoroutine = null;
    }

    // 랜덤 스킬 실행
    protected void StartRandomSkill()
    {
        var count = skillKeys.Count;                            // 담아둔 key들의 개수
        var randomIndex = Random.Range(0, count);               // 랜덤 인덱스
        var key = skillKeys[randomIndex];                       // key를 담아둔 List에서 랜덤 인덱스의 key 가져오기

        var slotDict = slot.GetSlotDict();                      // 스킬 슬롯 dictionary
        var skillDistance = GetSkillDistance(slotDict, key);    // 실행할 스킬의 거리 가져오기
        var distance = GetDistance(target.transform.position);  // 타겟과의 거리

        // 처음 랜덤으로 가져온 스킬의 범위 검사
        // 타겟이 스킬 범위 안에 있을 경우
        if (distance <= skillDistance)
        {
            var slot = slotDict[key];
            var data = slot.GetData() as EnemySkillButtonData;
            if (!slot.GetIsAvailable())
                return;

            slot.StartSkill(gameObject, (int)data.type, EnumConverter.GetString(CharacterEnum.Player));    // 스킬 실행
        }

        // 랜덤으로 가져온 스킬의 범위에 타겟이 없을 경우
        else
        {
            // 스킬 슬롯을 순회하며 범위가 닿는 스킬 찾기
            foreach (var skillButton in slotDict)
            {
                skillDistance = GetSkillDistance(slotDict, skillButton.Key);
                if (distance <= skillDistance)  // 가져온 스킬의 범위가 닿을 경우 스킬 실행, 반복문 중단
                {
                    key = skillButton.Key;
                    var slot = slotDict[key];
                    if (!slot.GetIsAvailable())
                        return;

                    var data = slot.GetData() as EnemySkillButtonData;
                    slot.StartSkill(gameObject, (int)data.type, EnumConverter.GetString(CharacterEnum.Player));
                    break;
                }
            }
        }
    }

    // 공격
    protected void EnemyAttack()
    {
        if (waitNextAttackCoroutine != null)
            return;

        if (target == null)
        {
            state = State.Patrol;
            return;
        }

        attackMode = GetRandomAttackMode(AttackMode.Skill);
        if (attackMode == AttackMode.Combo || attackMode == AttackMode.Pattern || slot == null)
        {
            attackMode = AttackMode.Normal;
        }

        switch (attackMode)
        {
            case AttackMode.Normal:
                if (GetDistance(target.transform.position) <= attackRange)
                {
                    StopMove();
                    UseAutoAttack();
                    state = State.Chase;
                    WaitNextAttack(waitAttackDelay);
                }
                break;
            case AttackMode.Skill:
                if (GetDistance(target.transform.position) <= skillRange)   // 타겟이 스킬 범위 안에 있을 경우
                {
                    if (skillKeys == null || skillKeys.Count == 0)  // 스킬 슬롯이 비었을 경우 중지
                    {
                        state = State.Chase;
                        return;
                    }

                    StartRandomSkill(); // 가지고 있는 스킬 중 랜덤으로 골라 스킬 실행
                    WaitNextAttack(waitSkillDelay);
                }
                break;
            case AttackMode.Combo:
                break;
            case AttackMode.Pattern:
                break;
            default:
                break;
        }

        state = State.Chase;
    }

    // 가지고 있는 스킬의 거리 return
    private float GetSkillDistance(Dictionary<string, SkillExcutor> dict, string key)
    {
        return dict[key].GetData().skill.data.distance;
    }

    // 거리 재기
    protected float GetDistance(Vector3 targetPos)
    {
        return Vector3.Distance(transform.position, targetPos);
    }

    // 추적 행동 
    protected void Chase()
    {
        if (target == null)
            return;

        var targetPos = target.transform.position;
        agent.speed = runSpeed;
        agent.SetDestination(targetPos);

        if (GetDistance(targetPos) <= attackRange || GetDistance(targetPos) <= skillRange)
        {
            state = State.Attack;
        }

        if (GetDistance(targetPos) > recognitionRange)
        {
            LoseTarget();
        }
    }

    // 타겟 위치 재확인, 타겟 해제 
    protected void CheackLoseTarget()
    {
        if (target == null)
            return;

        if (GetDistance(target.transform.position) <= recognitionRange) // 타겟이 인식 범위 안에 있을 경우
            return;

        agent.speed = data.moveSpeed;
        target = null;          // 타겟 해제
        state = State.Patrol;   // 정찰 시작
    }

    // 타겟 해제 코루틴
    protected IEnumerator CoLoseTarget()
    {
        yield return new WaitForSeconds(loseTargetTime);    // 타겟 해제 타이머 시간동안 대기
        CheackLoseTarget(); // 타겟 위치 재확인
        loseTargetCoroutine = null;
    }

    // 타겟 해제 
    protected void LoseTarget()
    {
        if (target == null)
            return;

        if (GetDistance(target.transform.position) <= recognitionRange)   // 타겟 설정이 되어있고, 타겟이 인식 범위에 있을 경우
        {
            if (loseTargetCoroutine != null)    // 타겟 해제 코루틴이 실행 중일 때
            {
                // 정찰 코루틴 초기화, 타겟 해제 코루틴 정지 및 초기화, 정찰 활성화
                StopCoroutine(loseTargetCoroutine);
                patrolCoroutine = null;
                loseTargetCoroutine = null;
                //isPatrol = true;
                state = State.Patrol;
            }

            return;
        }

        loseTargetCoroutine = StartCoroutine(CoLoseTarget());   // 타겟 해제 코루틴 시작
    }

    // 정찰 행동 
    protected void Patrol()
    {
        agent.speed = data.moveSpeed;

        if (patrolCoroutine == null)                                    // 코루틴이 실행되지 않았을 경우
            patrolCoroutine = StartCoroutine(CoPatrol());               // 코루틴 시작

        if (player == null)
            return;

        if (GetDistance(player.transform.position) <= recognitionRange) // 플레이어가 인식 범위 안에 들어왔을 경우
        {
            if (patrolCoroutine != null)                                // 코루틴이 실행 중일 때 코루틴 중지
            {
                StopCoroutine(patrolCoroutine);
                patrolCoroutine = null;
            }

            target = player;        // 타겟 설정 (플레이어)
            state = State.Chase;    // 정찰 중지
        }
    }

    protected abstract void StateBehavior();

    public void SetPatrolState()
    {
        state = State.Patrol;
    }

    protected AttackMode GetRandomAttackMode(AttackMode max)
    {
        int randomAttackPattern = Random.Range((int)AttackMode.Normal, (int)max+1);
        return (AttackMode)randomAttackPattern;
    }

    protected AttackMode GetRandomAttackMode(AttackMode max, AttackMode exception)
    {
        AttackMode randomAttackPattern = (AttackMode)Random.Range((int)AttackMode.Normal, (int)max+1);
        do
        {
            randomAttackPattern = (AttackMode)Random.Range((int)AttackMode.Normal, (int)max+1);
        } while (randomAttackPattern == exception);

        return randomAttackPattern;
    }

    // 범위 내 랜덤 위치로 이동하는 코루틴
    protected IEnumerator CoPatrol()
    {
        var randomPos = Random.insideUnitSphere * patrolRange;          // patrolRange로 범위 조절
        randomPos = transform.position + randomPos;                     // 현재 위치를 기반으로 범위 설정
        randomPos.y = transform.position.y;                             // y값이 위, 아래로 크게 변동되지 않게 랜덤 위치값을 현재 캐릭터 위치의 y값으로 변경
        agent.SetDestination(randomPos);

        yield return new WaitUntil(() => GetDistance(randomPos) < 5f);  // 위치에 도착할 때 까지 대기
        yield return new WaitForSeconds(1f);                            // 도착 후 1초 대기

        patrolCoroutine = null;
    }

    protected void CreateNewSkill(SkillButtonData data)
    {
        var parent = slot.GetSlotObj();
        slot.CreateExcutor(parent, data);
    }

    protected void CreateNewSkills(List<SkillButtonData> datas)
    {
        var parent = slot.GetSlotObj();
        foreach (var skillData in datas)
        {
            slot.CreateExcutor(parent, skillData);
        }
    }

    public EnemyGenerator GetEnemyGenerator() { return enemyGenerator; }
    public void SetEnemyGenerator(EnemyGenerator eg) { enemyGenerator = eg; }

    // 애니메이션 이벤트
    public void SaveCurrentTargetPos()
    {
        foreach (var weapon in weapons)
        {
            var rangedWeapon = weapon as RangedWeapon;
            if (rangedWeapon != null)
            {
                rangedWeapon.SaveCurrentTargetPos();
            }
        }
    }

    public void UseSpawnEffect()
    {
        EffectManager.Instance.UseEffect(spawnEffect, gameObject.transform, true, true);
    }

    // 에디터 전용 코드
    // 인식 범위 기즈모로 그리기 (테스트용)
#if UNITY_EDITOR

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawCircle(gameObject.transform.position, recognitionRange);    // 플레이어 인식 범위
    }

    void DrawCircle(Vector3 position, float radius)
    {
        int segments = 100;
        float angle = 0f;

        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            angle += 2 * Mathf.PI / segments;

            float nextX = Mathf.Cos(angle) * radius;
            float nextZ = Mathf.Sin(angle) * radius;

            Gizmos.DrawLine(position + new Vector3(x, 0, z), position + new Vector3(nextX, 0, nextZ));
        }
    }

#endif
}
