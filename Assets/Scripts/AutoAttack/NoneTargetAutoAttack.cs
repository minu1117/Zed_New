using UnityEngine;

public class NoneTargetAutoAttack : AutoAttack
{
    public override void Attack(GameObject character)
    {
        if (character.tag == EnumConverter.GetString(CharacterEnum.Player))
        {
            Vector3 point = Raycast.GetMousePointVec(); // 마우스 위치
            point.y = character.transform.position.y;   // point의 y값을 캐릭터의 y값으로 변경 (위, 아래로 이동하지 않게)
            character.transform.LookAt(point);          // character를 point 위치로 바라보기
        }
    }
}
