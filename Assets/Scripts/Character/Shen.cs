using UnityEngine;

public class Shen : BossEnemy
{
    [SerializeField] private Weapon duskSword;
    [SerializeField] private GameObject duskSwordHand;
    [SerializeField] private GameObject duskSwordDummy;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Init()
    {
        base.Init();
        duskSword.SetDamage(duskSword.data.damage);
        duskSword.SetChamp(this);
        duskSword.OnFinished();
        duskSword.gameObject.SetActive(false);
    }

    //protected override void AttackByMode()
    //{
    //    switch (attackMode)
    //    {
    //        case AttackMode.Normal:
    //            EnemyAttack();
    //            break;
    //        case AttackMode.Combo:
    //            UseComboSkill();
    //            break;
    //        case AttackMode.Pattern:
    //            UsePattern();
    //            break;
    //        default:
    //            break;
    //    }
    //}

    public override void Update()
    {
        MoveAnimation();
        StateBehavior();
    }

    /********************************************** Animation Event **********************************************/
    public void OnDuskSword()
    {
        duskSword.OnReady();
    }

    public void FinishedDuskSword()
    {
        duskSword.OnFinished();
    }

    public void ActivationDuskSword()
    {
        duskSword.gameObject.SetActive(true);
    }

    public void DeactivationDuskSword()
    {
        duskSword.gameObject.SetActive(false);
    }
}
