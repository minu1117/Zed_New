using UnityEngine;

public class DialogueObject : MonoBehaviour
{
    public string eventName;            // 대화 이벤트 이름
    public TypingType typingType;       // 텍스트가 나올 때 어떻게 나올 지 선택
    private DialogueManager manager;    // 대화 매니저

    private void Awake()
    {
        manager = DialogueManager.Instance; // 대화 매니저 가져오기
    }

    // 충돌 처리
    private void OnTriggerEnter(Collider other)
    {
        // CharacterMoveController 컴포넌트 추출 성공 시
        if (other.TryGetComponent(out CharacterMoveController character))
        {
            character.StopMove();               // 캐릭터 멈추기
            manager.isTalking = true;           // 대화 상태 활성화
            manager.SetCharacter(character);    // 대화 중인 캐릭터 매니저에 넘겨주기
            manager.SetTypingType(typingType);
            manager.SetCurrentTalk(eventName);  // 이벤트 이름 넘겨주기
            manager.GetMessage(typingType);     // 대화 시작
        }
    }
}
