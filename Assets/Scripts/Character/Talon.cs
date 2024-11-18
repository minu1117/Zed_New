using UnityEngine;

public class Talon : BossEnemy
{
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
}
