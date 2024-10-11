using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public string skillTypeParamName;               // 스킬 타입 파라미터
    public string useSkillParamName;            // 스킬 실행 트리거
    public string autoAttackTriggerName;        // 평타 실행 트리거
    public string attackSpeedParamName;         // 평타 속도 파라미터
    public string attackTypeParamName;          // 평타 타입 파라미터
    public string nextMotionTriggerParamName;   // 다음 모션으로 연결할 때 쓰이는 트리거
    public string upperLayerParamName;          // 상체 레이어 사용 시 쓰는 파라미터
    public AutoAttackEnum maxAutoAttackEnum;    // 평타 종류 애니메이션 개수 (최대 개수)
    private Animator animator;
    private int currentLayerIndex;              // 현재 레이어 인덱스
    public int upperLayerIndex = 1;            // 상체 레이어 인덱스
    public int wholeBodyLayerIndex = 2;        // 전신 레이어 인덱스

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // 이동 애니메이션
    public void UpdateMoveAnimation(Vector2 movement)
    {
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

    public Animator GetAnimator() { return animator; }
}
