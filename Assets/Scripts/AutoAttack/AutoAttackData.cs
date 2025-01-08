using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Auto Attack Data", menuName = "Scriptable Objects/Auto Attack Data")]
public class AutoAttackData : ScriptableObject
{
    public float damage;        // 데미지
    public float attackSpeed;   // 공격 속도
}
