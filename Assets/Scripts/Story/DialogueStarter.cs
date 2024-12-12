using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    public string csvFileName;          // 대화할 CSV 파일 이름
    public string eventName;            // 대화 이벤트 이름
    public TypingType typingType;       // 텍스트가 나올 때 어떻게 나올 지 선택
    public bool isRandom = false;       // 설정된 CSV에서 랜덤하게 대화를 가져올지 여부
    private DialogueManager manager;    // 대화 매니저

    private void Awake()
    {
        manager = DialogueManager.Instance;
    }

    public bool StartDialogue(GameObject other)
    {
        if (isRandom)
        {
            return RandomTalk(other);
        }
        else
        {
            return Talk(other);
        }
    }

    private bool Talk(GameObject other)
    {
        if (csvFileName == string.Empty)
            return false;

        if (eventName == string.Empty)
            return false;

        // CharacterMoveController 컴포넌트 추출 성공 시
        if (other.TryGetComponent(out CharacterMoveController character))
        {
            character.StopMove();               // 캐릭터 멈추기
            manager.isTalking = true;           // 대화 상태 활성화
            manager.SetCharacter(character);    // 대화 중인 캐릭터 매니저에 넘겨주기
            manager.SetTypingType(typingType);
            manager.SetCurrentTalk(csvFileName, eventName);  // 이벤트 이름 넘겨주기
            manager.GetMessage(typingType);     // 대화 시작
            return true;
        }

        return false;
    }

    private bool RandomTalk(GameObject other)
    {
        if (csvFileName == string.Empty)
            return false;

        // CharacterMoveController 컴포넌트 추출 성공 시
        if (other.TryGetComponent(out CharacterMoveController character))
        {
            var randomEvent = manager.GetRandomEvent(csvFileName);
            if (randomEvent == string.Empty)
                return false;

            character.StopMove();               // 캐릭터 멈추기
            manager.isTalking = true;           // 대화 상태 활성화
            manager.SetCharacter(character);    // 대화 중인 캐릭터 매니저에 넘겨주기
            manager.SetTypingType(typingType);
            manager.SetCurrentTalk(csvFileName, randomEvent);  // 이벤트 이름 넘겨주기
            manager.GetMessage(typingType);     // 대화 시작
            return true;
        }

        return false;
    }
}
