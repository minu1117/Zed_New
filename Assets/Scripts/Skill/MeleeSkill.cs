using System.Collections;
using UnityEngine;

public class MeleeSkill : Skill
{
    public override void Use(GameObject character)
    {
        gameObject.transform.forward = usePoint;
        StartCoroutine(CoMelee(character));  // 근접 스킬 사용 코루틴 시작
    }

    // 근접 스킬 사용 코루틴
    private IEnumerator CoMelee(GameObject character)
    {
        isCollide = true;
        foreach (var coll in colliders)
        {
            coll.GetCollider().enabled = false;
        }

        yield return waitUseDelay;

        foreach (var coll in colliders)
        {
            coll.GetCollider().enabled = true;
        }
        isCollide = false;

        base.Use(character);

        yield return waitduration;  // 지속 시간만큼 대기
        Release();                  // 오브젝트 풀에 반납
    }
}
