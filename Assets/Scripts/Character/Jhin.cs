using System.Collections;
using UnityEngine;

public class Jhin : BossEnemy
{
    [SerializeField] private Transform R_Shot_Tr;
    [SerializeField] private Transform spine;
    [SerializeField] private string isLastAAParamName;
    [SerializeField] private float waitStartDelay;

    private Jhin_Weapon jhin_Weapon;
    private bool isStart = false;
    private Coroutine startActionCoroutine;
    
    protected override void Awake()
    {
        base.Awake();
        jhin_Weapon = weapons[0].GetComponent<Jhin_Weapon>();
    }

    public override void Init()
    {
        base.Init();
        ResetEnemy();
    }

    public override void ResetEnemy()
    {
        base.ResetEnemy();

        isStart = false;
        if (startActionCoroutine == null)
            startActionCoroutine = StartCoroutine(CoStartAction());
    }

    public override void Update()
    {
        MoveAnimation();

        if (!isStart)
            return;

        StateBehavior();
    }

    // 평타 실행
    public override void Attack()
    {
        bool isLast = false;
        if (jhin_Weapon != null)
        {
            isLast = jhin_Weapon.GetIsLast();
        }

        if (animationController != null)
        {
            if (isLast)
            {
                animationController.SetBool(isLastAAParamName, true);
            }
            else
            {
                animationController.SetBool(isLastAAParamName, false);
            }

            animationController.Attack(autoAttack.data.attackSpeed);    // 평타 애니메이션 실행
        } 

        if (agent != null)
            agent.isStopped = true;

        autoAttack.Attack(gameObject);  // 평타 실행
    }

    public Transform GetShotTR() { return R_Shot_Tr; }
    public Transform GetSpine() { return spine; }

    private IEnumerator CoStartAction()
    {
        if (player == null)
            yield break;

        var map = enemyGenerator.GetMap();
        if (map == null)
            yield break;

        var cm = map.GetCurtainMoveController();
        if (cm == null)
        {
            isStart = true;
            yield break;
        }

        var targetChampBase = player.GetComponent<ChampBase>();
        if (targetChampBase == null)
            yield break;

        var targetMoveController = targetChampBase.GetMoveController();
        if (targetMoveController == null)
            yield break;

        cm.Open();
        targetMoveController.StopMove();
        Zed.Instance.SetAttackUse(false);

        var defaultSpeed = agent.speed;
        agent.speed = defaultSpeed * 1.5f;
        agent.SetDestination(player.transform.position);

        yield return new WaitUntil(() => cm.GetIsTrigged());

        cm.SetIsTrigged(false);
        cm.Close();
        yield return new WaitForSeconds(waitStartDelay);

        agent.speed = defaultSpeed;
        targetMoveController.StartMove();
        Zed.Instance.SetAttackUse(true);
        isStart = true;
        startActionCoroutine = null;
    }

    public override IEnumerator OnDead()
    {
        OpenCurtain();
        StartCoroutine(base.OnDead());
        yield return null;
    }

    private void OpenCurtain()
    {
        var map = enemyGenerator.GetMap();
        if (map == null)
            return;

        var cm = map.GetCurtainMoveController();
        if (cm == null)
            return;

        cm.Open();
    }

    public void SetIsStart(bool set) { isStart = set; }
}
