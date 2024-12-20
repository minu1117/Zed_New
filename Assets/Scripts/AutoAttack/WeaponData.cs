using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Scriptable Objects/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public float damage;                    // 데미지

    [Header("시전 사운드")]
    public List<AudioClip> useClips;

    [Header("타격 사운드")]
    public List<AudioClip> attackClips;

    [Header("보이스")]
    public List<AudioClip> voiceClips;

    [Header("타격 이펙트")]
    public Effect hitEffect;
}
