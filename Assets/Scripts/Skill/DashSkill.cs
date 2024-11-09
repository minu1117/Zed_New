using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DashSkill : Skill
{
    private BoxCollider coll;   // 대쉬 스킬의 Collider
    //private Vector3 movePoint;  // 이동할 위치

    public override void Use(GameObject character)
    {
        base.Use(character);

        if (coll == null)
            coll = GetComponent<BoxCollider>();

        if (usePoint == Vector3.zero)                  // 이동할 위치가 0,0,0일 경우
            usePoint = Raycast.GetMousePointVec();     // 현재 마우스 위치로 설정

        SetColliderSize();                              // Collider 사이즈 설정

        var moveController = character.GetComponent<CharacterMoveController>();           // 시전자의 CharacterMoveController
        var animationController = character.GetComponent<CharacterAnimationController>(); // 시전자의 애니메이션 컨트롤러
        StartCoroutine(CoDash(character, usePoint, animationController, moveController));     // 대쉬 스킬 사용 코루틴 실행
    }

    //public void SetMovePoint(Vector3 point)
    //{
    //    movePoint = point;
    //}

    private void Update()
    {
        FollowCaster();
    }

    // Collider 사이즈 설정
    private void SetColliderSize()
    {
        if (caster == null) // 시전자가 없을 시 return
            return;

        var casterCollider = caster.GetComponent<BoxCollider>();                    // 시전자의 Collider 가져오기
        coll.size = casterCollider.size;                                            // Collider Size를 시전자의 Collider Size로 변경 (시전자와 동일한 크기가 되게)
        coll.size = new Vector3(coll.size.x * 2, coll.size.y, coll.size.z * 2);     // 크기 조정 (더 크게)
        coll.center = casterCollider.center;                                        // Collider의 center를 시전자 Collider의 center값으로 변경 (시전자 Collider와 동일한 위치에 있게)
    }

    // 시전자 따라가기
    private void FollowCaster()
    {
        if (caster == null) // 시전자가 없을 경우 return
            return;

        gameObject.transform.position = caster.transform.position;  // 시전자 따라다니기
    }

    // 대쉬 스킬 사용 코루틴
    // obj = 사용자(시전자)
    private IEnumerator CoDash(GameObject obj, Vector3 point, CharacterAnimationController animationController, CharacterMoveController moveController)
    {
        /********************************************** 사용 대기 **********************************************/

        Rigidbody rb = null;
        NavMeshAgent agent = null;

        if (moveController != null)
        {
            moveController.StopMove();                  // 시전자 이동 제한
            rb = moveController.GetRigidbody();
            agent = moveController.GetAgent();
        }

        if (rb == null)
        {
            rb = obj.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
        }
        if (agent == null)
        {
            agent = obj.GetComponent<NavMeshAgent>();
            agent.isStopped = true;
        }

        coll.enabled = false;

        point.y = obj.transform.position.y;                                                 // 이동할 위치의 y값을 시전자의 y값으로 변경 (위, 아래로 돌진하지 않게)

        Vector3 LookAtDirection = (point == Vector3.zero) ? obj.transform.forward : point;  // 바라볼 방향 계산 -> 이동할 위치가 0,0,0일 시 forward 바라보기
        Vector3 dashDirection = (point - obj.transform.position).normalized;                // 대쉬 방향 계산 -> (이동할 위치 - 시전자 위치) 정규화
        obj.transform.LookAt(LookAtDirection);                                              // 시전자 회전 값 변경 (바라보기)

        yield return waitUseDelay;                      // 시전 대기 시간동안 대기


        /********************************************** 사용 중 **********************************************/

        if (agent != null && data.isDashPass)
        {
            agent.enabled = false;
        }

        coll.enabled = true;

        if (animationController != null)                // 시전자의 애니메이션 컨트롤러가 있을 시
            animationController.StartNextMotion();      // 다음 모션 재생 ([준비 -> 대쉬 -> 완료] 순으로 애니메이션을 재생하기 때문에 [준비] 모션에서 [대쉬] 모션으로 바꾸는 과정) 

        if (rb != null)                                 // 시전자의 rigidBody가 있을 시 (중간에 죽어서 사라질 수도 있기 때문에 계속 체크)
            rb.velocity = dashDirection * data.speed;   // rigidBody의 속도 값 변경 -> 대쉬 방향 * 대쉬 속도

        yield return waitduration;                      // 지속 시간만큼 대기


        /********************************************** 사용 완료 **********************************************/

        coll.enabled = false;

        if (rb != null)                                 // 시전자의 rigidBody가 있을 시 (중간에 죽어서 사라질 수도 있기 때문에 계속 체크)
            rb.velocity = Vector3.zero;                 // rigidBody의 속도 값 초기화

        yield return waitimmobilityTime;                // 끝난 후 경직 시간만큼 대기

        if (animationController != null)                // 시전자의 애니메이션 컨트롤러가 있을 시
            animationController.StartNextMotion();      // 다음 모션으로 변경 -> [완료] 모션

        StartSound(data.complateClips);

        usePoint = Vector3.zero;                        // 이동할 위치 초기화
        if (moveController != null)                     // 시전자의 CharacterMoveController가 있을 시
            moveController.isMoved = true;              // 시전자 이동 활성화

        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(agent.transform.position);
            if (agent.isActiveAndEnabled)
            {
                agent.isStopped = false;
            }
        }

        Release();  // 오브젝트 풀에 반납
    }
}
