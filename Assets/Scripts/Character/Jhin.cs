using UnityEngine;

public class Jhin : BossEnemy
{
    [SerializeField] private Transform R_Shot_Tr;
    [SerializeField] private Transform spine;
    
    protected override void Awake()
    {
        base.Awake();
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

    public Transform GetShotTR() { return R_Shot_Tr; }
    public Transform GetSpine() { return spine; }
}
