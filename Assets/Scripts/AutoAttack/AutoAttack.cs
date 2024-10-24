using UnityEngine;

public abstract class AutoAttack : MonoBehaviour
{
    public AutoAttackData data;
    public abstract void Attack(GameObject character);
}
