using UnityEngine;

public class TargetingSkill : Skill, ITargetable
{
    public bool isChase;         // 대상한테 붙은 후 사용 여부
    protected GameObject target;

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public override void Use(GameObject character)
    {
        base.Use(character);
    }
}
