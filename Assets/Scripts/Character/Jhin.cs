using UnityEngine;

public class Jhin : BossEnemy
{
    [SerializeField] private Transform R_Shot_Tr;
    [SerializeField] private Transform spine;
    [SerializeField] private string isLastAAParamName;

    private Jhin_Weapon jhin_Weapon;
    
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

    public override void Update()
    {
        MoveAnimation();
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
}
