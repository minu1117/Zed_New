using DG.Tweening;
using UnityEngine;

public class Shen : BossEnemy
{
    [SerializeField] private Weapon duskSword;
    [SerializeField] private GameObject duskSwordHand;
    [SerializeField] private Shen_DuskSword_Dummy duskSwordDummy;
    [SerializeField] private SkillButtonData duskSwordSkill;
    [SerializeField] private float duskSwordMoveDuration;
    private Shen_DuskSword_Dummy createdDuskswordDummy;

    [SerializeField] private LineRenderer duskSwordLineRenderer;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Init()
    {
        base.Init();

        AddPattern(duskSwordSkill, patternDict);
        SortPattern();

        createdDuskswordDummy = Instantiate(duskSwordDummy);
        createdDuskswordDummy.SetShen(this);
        createdDuskswordDummy.transform.position = new Vector3(shotStartTransform.position.x, createdDuskswordDummy.transform.position.y, shotStartTransform.position.z);
        duskSword.SetDamage(duskSwordSkill.skill.data.damage);
        duskSword.SetChamp(this);

        ResetEnemy();
    }

    public override void ResetEnemy()
    {
        createdDuskswordDummy.gameObject.SetActive(true);
        duskSword.OnFinished();
        duskSword.gameObject.SetActive(false);
    }

    public override void Update()
    {
        MoveAnimation();
        StateBehavior();
    }

    public override void OnDead()
    {
        duskSword.OnFinished();
        duskSword.gameObject.SetActive(false);
        createdDuskswordDummy.gameObject.SetActive(false);
        base.OnDead();
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

    public void CallDuskSword()
    {
        var pos = new Vector3(transform.position.x, createdDuskswordDummy.transform.position.y, transform.position.z);
        createdDuskswordDummy.transform.DOMove(pos, duskSwordMoveDuration)
                                       .OnComplete(OnNext);
    }

    private void OnNext()
    {
        animationController.StartNextMotion();
    }
}
