using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gangplank : BossEnemy
{
    public int spell_1_Animation_Quarter;

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
