using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : EliteEnemy
{
    [SerializeField] protected List<SkillButtonData> patternSkillList;
    protected Dictionary<float, List<SkillButtonData>> patternDict;
    private List<KeyValuePair<float, List<SkillButtonData>>> sortedPattern;
    private List<SkillButtonData> currentPattern;
    private float currentPatternHP;
    private float nextPatternHP;
    protected Coroutine usePatternWaitCoroutine;
    [SerializeField] protected float waitPatternDelay;
    [SerializeField] protected float resetDelay;
    protected bool isResetPattern;
    protected bool isPatternUsing;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Init()
    {
        base.Init();
        recognitionRange = Mathf.Infinity;
        patternDict = new();
        sortedPattern = new();

        if (patternSkillList == null || patternSkillList.Count == 0)
            return;

        AddPattern(patternSkillList, patternDict);
        SortPattern();
        SetCurrentPattern(sortedPattern[0].Value, sortedPattern[0].Key);
        nextPatternHP = data.maxHp;
    }

    public override void ResetEnemy()
    {
        base.ResetEnemy();
        nextPatternHP = data.maxHp;
        isPatternUsing = false;
    }

    public override void Update()
    {
        MoveAnimation();
        StateBehavior();
    }

    protected override void StateBehavior()
    {
        if (isPatternUsing)
            return;

        if (state == State.Patrol)
        {
            state = State.Chase;
            target = player;
        }

        switch (state)
        {
            case State.Patrol:
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                AttackByMode();
                break;
            default:
                break;
        }
    }

    protected override void AttackByMode()
    {
        if (waitNextAttackCoroutine != null)
            return;

        if (isResetPattern)
        {
            attackMode = AttackMode.Pattern;
        }
        else
        {
            if (comboDict == null || comboDict.Count == 0)
            {
                attackMode = GetRandomAttackMode(AttackMode.Pattern, AttackMode.Combo);
            }
            else
            {
                attackMode = GetRandomAttackMode(AttackMode.Pattern);
            }
        }

        float duration = 0f;
        switch (attackMode)
        {
            case AttackMode.Normal:
                duration = waitAttackDelay;
                EnemyAttack();
                break;
            case AttackMode.Skill:
                duration = waitSkillDelay;
                EnemyAttack();
                break;
            case AttackMode.Combo:
                duration = waitComboDelay;
                StartRandomCombo();
                break;
            case AttackMode.Pattern:
                duration = waitPatternDelay;

                var excutor = GetRandomPatternSkillExcutor();
                if (excutor == null)
                    break;

                var delay = excutor.GetData().skill.data.useDelay;
                UsePattern(delay, excutor);
                break;
            default:
                break;
        }

        WaitNextAttack(duration);
    }

    protected void UsePattern(float waitDelay, SkillExcutor excutor)
    {
        usePatternWaitCoroutine = StartCoroutine(CoWaitPattern(waitDelay, excutor));
    }

    protected IEnumerator CoWaitPattern(float waitDelay, SkillExcutor excutor)
    {
        isPatternUsing = true;
        if (waitNextAttackCoroutine != null)
        {
            StopCoroutine(waitNextAttackCoroutine);
            waitNextAttackCoroutine = null;
        }

        float prevtTime = Time.time;
        float skillDistance = excutor.GetData().skill.data.distance;

        agent.speed = runSpeed;
        while (GetDistance(player.transform.position) >= skillDistance)
        {
            agent.SetDestination(player.transform.position);
            yield return null;
        }
        agent.speed = data.moveSpeed;

        float chaseTime = Time.time - prevtTime;
        WaitNextAttack(waitPatternDelay);

        if (waitDelay - chaseTime > 0)
            yield return new WaitForSeconds(waitDelay - chaseTime);

        UsePattern(excutor);
        usePatternWaitCoroutine = null;
        isPatternUsing = false;
    }

    protected void UseRandomPattern()
    {
        if (currentPattern == null || currentPattern.Count == 0)
            return;

        var playerTag = EnumConverter.GetString(CharacterEnum.Player);

        var excutor = GetRandomPatternSkillExcutor();
        if (excutor == null)
            return;

        var data = excutor.GetData();
        excutor.SetIsAvailable(true);
        excutor.StartSkill(gameObject, playerTag);

        var enemySkillButtonData = data as EnemySkillButtonData;
        animationController.UseSkill((int)enemySkillButtonData.type, enemySkillButtonData.isUpper);
    }

    protected void UsePattern(SkillExcutor excutor)
    {
        var playerTag = EnumConverter.GetString(CharacterEnum.Player);

        var data = excutor.GetData();
        excutor.SetIsAvailable(true);
        excutor.StartSkill(gameObject, playerTag);

        var enemySkillButtonData = data as EnemySkillButtonData;
        animationController.UseSkill((int)enemySkillButtonData.type, enemySkillButtonData.isUpper);
    }

    protected SkillExcutor GetRandomPatternSkillExcutor()
    {
        if (currentPattern == null)
            return null;

        int randomIndex = UnityEngine.Random.Range(0, currentPattern.Count);
        var data = currentPattern[randomIndex];

        return slot.GetSlotDict()[data.keycode];
    }

    protected void AddPattern(SkillButtonData skillData, Dictionary<float, List<SkillButtonData>> dict)
    {
        CreateNewSkill(skillData);
        var bossSkill = skillData as BossSkillButtonData;
        if (bossSkill == null)
            return;

        float percent = bossSkill.healthPercentage / 100f;
        float patternHP = data.maxHp * percent;

        if (dict.ContainsKey(patternHP))
        {
            dict[patternHP].Add(bossSkill);
        }
        else
        {
            var list = new List<SkillButtonData> { bossSkill };
            dict.Add(patternHP, list);
        }
    }

    protected void AddPattern(List<SkillButtonData> skillList, Dictionary<float, List<SkillButtonData>> dict)
    {
        CreateNewSkills(skillList);
        foreach (var skillData in skillList)
        {
            var bossSkill = skillData as BossSkillButtonData;
            if (bossSkill == null)
                continue;

            float percent = bossSkill.healthPercentage / 100f;
            float patternHP = data.maxHp * percent;

            if (dict.ContainsKey(patternHP))
            {
                dict[patternHP].Add(bossSkill);
            }
            else
            {
                var list = new List<SkillButtonData> { bossSkill };
                dict.Add(patternHP, list);
            }
        }
    }

    protected void SortPattern()
    {
        foreach (var item in patternDict)
        {
            sortedPattern.Add(item);
        }

        sortedPattern.Sort((KeyValuePair<float, List<SkillButtonData>> a, KeyValuePair<float, List<SkillButtonData>> b) => { return b.Key.CompareTo(a.Key); });
    }

    private void SetCurrentPattern(List<SkillButtonData> pattern, float patternHP)
    {
        currentPattern = pattern;
        currentPatternHP = patternHP;
    }

    private void AddCurrentPattern(List<SkillButtonData> pattern, float patternHP)
    {
        foreach (var skill in pattern)
        {
            currentPattern.Add(skill);
        }
        currentPatternHP = patternHP;
    }

    private void DecidePattern()
    {
        float currentHP = data.currentHp;
        if (nextPatternHP < currentHP || currentHP <= 0)
            return;

        for (int i = 0; i < sortedPattern.Count; i++)
        {
            var key = sortedPattern[i].Key;
            if (key >= currentHP && key < currentPatternHP)
            {
                var value = sortedPattern[i].Value;
                //SetCurrentPattern(value, key);
                AddCurrentPattern(value, key);

                if (i + 1 < sortedPattern.Count)
                {
                    nextPatternHP = sortedPattern[i + 1].Key;
                }
                else
                {
                    nextPatternHP = key;
                }

                ResetUsePattern();
                break;
            }
        }
    }

    protected void ResetUsePattern()
    {
        if (usePatternWaitCoroutine != null)
            StopCoroutine(usePatternWaitCoroutine);

        isResetPattern = true;
        usePatternWaitCoroutine = null;
        waitNextAttackCoroutine = null;
        WaitNextAttack(resetDelay);
    }

    public override void OnDamage(float damage)
    {
        base.OnDamage(damage);
        DecidePattern();
    }

    public override void OnDead()
    {
        if (usePatternWaitCoroutine != null)
        {
            StopCoroutine(usePatternWaitCoroutine);
            usePatternWaitCoroutine = null;
        }

        isResetPattern = false;
        base.OnDead();
    }
}
