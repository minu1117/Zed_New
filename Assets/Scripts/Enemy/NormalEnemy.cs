public class NormalEnemy : EnemyBase
{
    public override void Update()
    {
        base.Update();
        StateBehavior();
    }

    protected override void StateBehavior()
    {
        if (isDead)
            return;

        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                EnemyAttack();
                break;
        }
    }
}
