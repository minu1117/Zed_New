using System.Collections;
using UnityEngine;

public class ShieldBuffSkill : BuffSkill, ITargetable
{
    private ChampBase target;

    public override void Use(GameObject character)
    {
        base.Use(character);
        StartCoroutine(CoShield());  // 근접 스킬 사용 코루틴 시작
    }

    // 근접 스킬 사용 코루틴
    private IEnumerator CoShield()
    {
        if (target == null)
            yield break;

        target.OnShield(data.damage);

        yield return waitduration;  // 지속 시간만큼 대기

        var hpController = target.GetStatusController(SliderMode.HP);
        hpController.DeductedMaxShield(data.damage);

        var removeShieldValue = data.damage - hpController.shieldAccumulateDamage;
        target.HitShield(removeShieldValue);
        hpController.shieldAccumulateDamage = 0f;

        StartSound(data.complateClips);
        Release();                  // 오브젝트 풀에 반납
    }

    public void SetTarget(GameObject target)
    {
        this.target = target.GetComponent<ChampBase>();
    }
}
