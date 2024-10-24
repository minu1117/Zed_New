using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteEnemy : EnemyBase
{
    [SerializeField] protected ListManagerSkillButtonData comboSkillLists;
    protected Dictionary<int, List<SkillButtonData>> comboDict;

    private float addWaitTime = 1f;
    protected bool availableCombo;
    [SerializeField] protected float waitComboDelay;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Init()
    {
        base.Init();
        attackMode = AttackMode.Normal;

        if (comboSkillLists == null || comboSkillLists.listOfLists.Count == 0)
        {
            waitComboDelay = 0f;
            return;
        }

        comboDict = new();
        AddComboSkill(comboSkillLists, comboDict);
    }

    public override void Update()
    {
        base.Update();
        StateBehavior();
    }

    protected override void StateBehavior()
    {
        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                AttackByMode();
                break;
        }
    }

    protected virtual void AttackByMode()
    {
        if (waitNextAttackCoroutine != null)
            return;

        attackMode = GetRandomAttackMode(AttackMode.Pattern);
        if (comboDict == null || comboDict.Count == 0)
        {
            attackMode = GetRandomAttackMode(AttackMode.Combo, AttackMode.Combo);
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
            default:
                break;
        }

        WaitNextAttack(duration);
    }

    protected void StartRandomCombo()
    {
        if (comboDict == null || comboDict.Count == 0)
            return;

        var count = comboDict.Count;
        var randomRange = Random.Range(0, count);
        var combo = comboDict[randomRange];
    }

    private IEnumerator CoUseCombo(List<SkillButtonData> datas)
    {
        var slotDict = slot.GetSlotDict();
        var playerTag = EnumConverter.GetString(CharacterEnum.Player);

        foreach (var data in datas)
        {
            var slot = slotDict[data.keycode];
            slot.SetIsAvailable(true);
            var duration = slot.GetData().skill.data.duration;

            var enemySkillButtonData = data as EnemySkillButtonData;
            slot.StartSkill(gameObject, (int)enemySkillButtonData.type, playerTag);

            //var enemySkillButtonData = data as EnemySkillButtonData;
            //animationController.UseSkill((int)enemySkillButtonData.type);
            yield return new WaitForSeconds(duration + addWaitTime);
        }
    }

    protected void AddComboSkill(ListManagerSkillButtonData skillList, Dictionary<int, List<SkillButtonData>> dict)
    {
        var listOfList = skillList.GetListOfLists();
        if (skillList == null || listOfList == null || listOfList.Count == 0)
            return;

        int comboName = 0;
        foreach (var list in listOfList)
        {
            var items = list.items;
            if (items == null || items.Count == 0)
                continue;

            CreateNewSkills(items);
            dict.Add(comboName++, items);
        }
    }
}
