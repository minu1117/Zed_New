using UnityEngine;

[CreateAssetMenu(fileName = "Skill Button Data", menuName = "Scriptable Objects/Boss Skill Button Data")]
public class BossSkillButtonData : EnemySkillButtonData
{
    [Range (0f, 100f)]
    public float healthPercentage;
}