using UnityEngine;

[CreateAssetMenu(fileName = "Charactor Data", menuName = "Scriptable Objects/Charactor Data")]
public class CharacterData : ScriptableObject
{
    public float maxHp;             // 최대 체력
    public float currentHp;         // 현재 체력
    public float maxMp;             // 최대 마나
    public float currentMp;         // 현재 마나
    public int level;               // 레벨
    public string charactorName;    // 캐릭터 이름
    public float moveSpeed;         // 이동 속도
    public float jumpSpeed;         // 점프 속도
    public float gravity;           // 중력값

    // Json Save & Load
}
