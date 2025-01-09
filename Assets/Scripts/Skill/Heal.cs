using System.Collections;
using UnityEngine;

public class Heal : BuffSkill, ITargetable
{
    private ChampBase target;

    public override void Use(GameObject character)
    {
        base.Use(character);
        StartCoroutine(CoHeal());  // 근접 스킬 사용 코루틴 시작
    }

    private IEnumerator CoHeal()
    {
        if (target == null)
            yield break;

        int count = 0;
        var hpController = target.GetStatusController(SliderMode.HP);
        while (count < data.hitRate)
        {
            hpController.Heal(data.damage);
            SoundManager.Instance.StartSound(data.attackClips);
            count++;
            yield return hitInterval;
        }

        StartSound(data.complateClips);

        yield return waitduration;  // 지속 시간만큼 대기
        Release();                  // 오브젝트 풀에 반납
    }

    public void SetTarget(GameObject target)
    {
        this.target = target.GetComponent<ChampBase>();
    }
}
