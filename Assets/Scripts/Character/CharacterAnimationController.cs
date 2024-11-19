using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AutoAttackEnum
{
    None = -1,

    Attack_1,
    Attack_2,
    Attack_3,
    Attack_4,
    Attack_5,

    Count,
}

public enum JumpTypeEnum
{
    Short = 0,
    Medium,
    Long,
}

public class CharacterAnimationController : MonoBehaviour
{
    public string skillTypeParamName;           // 스킬 타입 파라미터
    public string useSkillParamName;            // 스킬 실행 트리거
    public string autoAttackTriggerName;        // 평타 실행 트리거
    public string attackSpeedParamName;         // 평타 속도 파라미터
    public string attackTypeParamName;          // 평타 타입 파라미터
    public string nextMotionTriggerParamName;   // 다음 모션으로 연결할 때 쓰이는 트리거
    public string upperLayerParamName;          // 상체 레이어 사용 시 쓰는 파라미터
    public string JumpTriggerName;              // 점프 실행 트리거
    public string JumpTypeParamName;            // 점프 타입별 분기 파라미터
    public AutoAttackEnum maxAutoAttackEnum;    // 평타 종류 애니메이션 개수 (최대 개수)
    private Animator animator;
    private int currentLayerIndex;              // 현재 레이어 인덱스
    public int upperLayerIndex = 1;             // 상체 레이어 인덱스
    public int wholeBodyLayerIndex = 2;         // 전신 레이어 인덱스

    public float shortJumpThreshold;            // 짧은 점프 거리 기준

    public List<AudioClip> jumpSoundClips;
    public List<AudioClip> landingSoundClips;

    private NavMeshAgent agent;
    private bool isJumping = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // NavMeshAgent가 NavMeshLink를 통과 중인지 확인
        if (agent.isOnOffMeshLink && !isJumping)
        {
            StartCoroutine(Jump());
        }
    }

    private IEnumerator Jump()
    {
        isJumping = true;

        // 시작점과 끝점 데이터 가져오기
        OffMeshLinkData linkData = agent.currentOffMeshLinkData;
        Vector3 startPos = linkData.startPos;
        Vector3 endPos = linkData.endPos;

        // 수평 거리 계산
        Vector3 horizontalVector = new Vector3(endPos.x - startPos.x, 0, endPos.z - startPos.z);
        float horizontalDistance = horizontalVector.magnitude;

        // 높이 변화 계산
        float heightDifference = Mathf.Abs(endPos.y - startPos.y);

        // 방향 각도 계산 (수평 벡터의 각도)
        Vector3 direction = horizontalVector.normalized;
        float angle = Vector3.Angle(Vector3.forward, direction); // 예: Z축 기준

        int type = 0;
        // 조건을 기반으로 애니메이션 선택
        if (horizontalDistance < shortJumpThreshold && heightDifference < 1f && angle < 45f)
        {
            type = (int)JumpTypeEnum.Medium;
        }
        else
        {
            type = (int)JumpTypeEnum.Long;
        }

        SoundManager.Instance.StartSound(jumpSoundClips);
        SetTrigger(JumpTriggerName);
        SetInteger(JumpTypeParamName ,type);

        // 점프 애니메이션 재생 시간 대기
        float jumpDuration = horizontalDistance / agent.speed;
        yield return new WaitForSeconds(jumpDuration);

        if (agent.isOnOffMeshLink)
        {
            agent.CompleteOffMeshLink();
        }
        isJumping = false;
        SoundManager.Instance.StartSound(landingSoundClips);
    }

    // 이동 애니메이션
    public void UpdateMoveAnimation(Vector2 movement)
    {
        if (isJumping)
            return;

        SetFloat("Horizontal",  movement.x);
        SetFloat("Vertical", movement.y);
    }

    // 공격 애니메이션
    public void Attack(float attackSpeed)
    {
        SetInteger(attackTypeParamName, Random.Range(0, (int)maxAutoAttackEnum + 1));   // 랜덤으로 Attack Type 설정, 파라미터에 값 넘기기 (평타 애니메이션 종류)
        SetFloat(attackSpeedParamName, attackSpeed);    // 공격 속도로 애니메이션 속도 조절
        SetTrigger(autoAttackTriggerName);  // 평타 애니메이션 실행 (트리거 활성화)
    }

    // 다음 모션으로 연결
    public void StartNextMotion()
    {
        SetTrigger(nextMotionTriggerParamName); // 다음 모션 연결 트리거 활성화
    }

    // 스킬 사용 애니메이션
    public void UseSkill(int enumIndex, bool isUpper = false)
    {
        if (nextMotionTriggerParamName != string.Empty)
            animator.ResetTrigger(nextMotionTriggerParamName);      // 다음 모션 연결 트리거 초기화 (오동작 방지)

        currentLayerIndex = isUpper ? upperLayerIndex : wholeBodyLayerIndex;        // 사용할 애니메이션 레이어 변경
        SetBool(upperLayerParamName, isUpper);                                      // 상체 레이어 사용 여부 설정

        SetInteger(skillTypeParamName, enumIndex);                  // 스킬 타입 파라미터 값 설정
        SetTrigger(useSkillParamName);                              // 스킬 사용 트리거 활성화
    }

    public void SetTrigger(string triggerName)
    {
        if (triggerName == string.Empty)
            return;

        animator.SetTrigger(triggerName);
    }

    public void SetFloat(string name, float value)
    {
        if (name == string.Empty)
            return;

        animator.SetFloat(name, value);
    }

    public void SetBool(string name, bool value)
    {
        if (name == string.Empty)
            return;

        animator.SetBool(name, value);
    }

    public void SetInteger(string name, int value)
    {
        if (name == string.Empty)
            return;

        animator.SetInteger(name, value);
    }

    // 실행 중인 애니메이션의 총 길이 가져오기
    public float GetCurrentAnimLength()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(currentLayerIndex);
        return currentState.length;
    }

    public bool HasParameter(string parameterName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == parameterName)
                return true;
        }
        return false;
    }

    public Animator GetAnimator() { return animator; }
}
