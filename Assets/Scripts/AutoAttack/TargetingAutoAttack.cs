using UnityEngine;

public class TargetingAutoAttack : AutoAttack, ITargetable
{
    private GameObject target;  // 타겟

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    // 평타 실행
    public override void Attack(GameObject character)
    {
        if (target == null)     // 타겟이 없을 경우 return
            return;

        Vector3 point = target.transform.position;  // 타겟 위치 가져오기
        point.y = character.transform.position.y;   // 가져온 위치의 y값을 character의 y값으로 변경 (평타가 위, 아래로 이동하지 않게)
        character.transform.LookAt(point);          // character를 가져온 위치 값을 바라보게 설정
    }
}
