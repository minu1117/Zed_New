using System.Collections;
using UnityEngine;

public class TargetingShotSkill : TargetingSkill
{
    public bool isPenetrate;        // 관통 여부
    private bool isOtherCollide = false;

    public override void Use(GameObject character)
    {
        if (character.tag == EnumConverter.GetString(CharacterEnum.Enemy))
            target = Zed.Instance.gameObject;

        if (target == null) // 타게팅 스킬이기 때문에 타겟이 없을 경우 Release, return
        {
            Release();
            return;
        }

        base.Use(character);
        StartCoroutine(CoShot());
    }

    //protected override void OnTriggerEnter(Collider other)
    //{
    //    if (other == null)
    //        return;

    //    Collide(other.gameObject);
    //}

    // 충돌 처리
    public override void Collide(GameObject obj)
    {
        if (obj == null)
            return;

        if (target == null || obj == null)  // 타겟이 없거나 부딪힌 대상이 없을 경우 return
            return;

        // 타겟에 부딪혔을 경우 타겟에 데미지 부여
        if (ReferenceEquals(target, obj))
        {
            //isCollide = true;
            if (isOtherCollide)
                isCollide = false;

            base.Collide(target);
        }

        // 관통하는 스킬일 경우 다른 대상에게 부딪혀도 데미지 부여
        else if (isPenetrate && !ReferenceEquals(target, obj))
        {
            isCollide = false;
            isOtherCollide = true;
            base.Collide(obj);
        }
    }

    // 날리기 코루틴
    private IEnumerator CoShot()
    {
        float duration = data.duration;
        Vector3 usePos = startPos;

        // 이동 거리 미리 계산 (적이 있는 방향으로)
        Vector3 totalMovement = usePos + (GetDir(usePos) * duration * data.speed);
        totalMovement.y = usePos.y;

        // 지속 시간만큼 날아가기 (타겟을 향해)
        for (var timePassed = 0f; timePassed < data.duration; timePassed += Time.deltaTime)
        {
            // 타겟에 부딪히지 않았을 경우 타겟을 향해 이동 거리 및 방향 재계산
            if (!isCollide)
            {
                Vector3 dir = GetDir(usePos);
                totalMovement = usePos + (dir * duration * data.speed);
                totalMovement.y = usePos.y;
            }

            var factor = timePassed / data.duration;
            transform.position = Vector3.Lerp(usePos, totalMovement, factor);
            yield return null;
        }

        Release();
    }

    // 방향 구하기
    private Vector3 GetDir(Vector3 usePosition)
    {
        Vector3 dir = target.transform.position - usePosition;
        dir.y = usePosition.y;
        dir.Normalize();

        return dir;
    }

    protected override void Release()
    {
        target = null;
        isCollide = false;
        base.Release();
    }
}
