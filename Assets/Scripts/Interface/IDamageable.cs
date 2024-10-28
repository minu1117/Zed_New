using System.Collections;

public interface IDamageable
{
    public IEnumerator DealDamage(ChampBase target, float damage, int hitRate);
}
