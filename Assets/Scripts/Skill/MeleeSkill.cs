using System.Collections;
using UnityEngine;

public class MeleeSkill : Skill
{
    public override void Use(GameObject character)
    {
        base.Use(character);

        var characterMoveController = character.GetComponent<CharacterMoveController>();
        StartCoroutine(CoMelee());  // 근접 스킬 사용 코루틴 시작
    }

    // 근접 스킬 사용 코루틴
    private IEnumerator CoMelee(CharacterMoveController moveController = null)
    {
        //if (moveController != null)
        //    moveController.isMoved = false;

        //yield return waitUseDelay;

        //if (moveController != null)
        //    moveController.isMoved = true;

        yield return waitduration;  // 지속 시간만큼 대기
        StartSound(data.complateClips);
        Release();                  // 오브젝트 풀에 반납
    }
}
