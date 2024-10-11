using UnityEngine;

public abstract class AutoAttack : MonoBehaviour/*, IDamageable*/
{
    public AutoAttackData data;
    public abstract void Attack(GameObject character);
}
