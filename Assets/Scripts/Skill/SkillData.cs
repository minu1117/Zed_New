using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Objects/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("스킬 타입")]
    public SkillType type;

    [Header("스킬 이름")]
    public string skillName;

    [Header("스킬 설명")]
    public string skillDescription;

    [Header("스킬 레벨 (처음엔 0 고정)")]
    public int skillLevel;

    [Header("레벨 제한")]
    public int levelRestriction;

    [Header("데미지 (보호막일 경우 보호막 값)")]
    public float damage;

    [Header("타격 횟수")]
    public int hitRate = 1;

    [Header("타격 간격")]
    public float hitInterval;

    [Header("쿨타임")]
    public float coolDown;

    [Header("지속 시간")]
    public float duration;

    [Header("사용 전 대기 시간")]
    public float useDelay;

    [Header("사용 후 경직 시간")]
    public float afterUseStiffenTime;

    [Header("물체의 이동 속도")]
    public float speed;

    [Header("끝난 후 경직 시간")]
    public float immobilityTime;

    [Header("그림자 스킬일 경우 체크")]
    public bool isShadow = false;

    [Header("대쉬 대상 통과 여부")]
    public bool isDashPass;

    [Header("사거리")]
    public float distance;

    [Header("스킬 범위 인디케이터")]
    public SkillIndicator indicator;

    [Header("이펙트")]
    public Effect effect;

    [Header("시전 사운드")]
    public List<AudioClip> useClips;

    [Header("타격 사운드")]
    public List<AudioClip> attackClips;

    [Header("소멸 사운드")]
    public List<AudioClip> disappearClips;

    [Header("재시전 사운드")]
    public List<AudioClip> recastClips;

    [Header("사용 완료 후 사운드")]
    public List<AudioClip> complateClips;

    [Header("음성")]
    public List<AudioClip> voiceClips;

    [Header("셀프 타게팅일 경우 체크")]
    public bool isSelf = false;

    [Header("사용 전 경직 여부")]
    public bool isStiffen = false;
}
