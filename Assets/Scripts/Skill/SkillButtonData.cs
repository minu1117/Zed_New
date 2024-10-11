using UnityEngine;

[CreateAssetMenu(fileName = "Skill Button Data", menuName = "Scriptable Objects/Skill Button Data")]
public class SkillButtonData : ScriptableObject
{
    public string keycode;      // 입력 키
    public Skill skill;         // 스킬
    public int maxPoolSize;     // 오브젝트 풀 최대 사이즈
    public Sprite sp;           // 이미지
}
