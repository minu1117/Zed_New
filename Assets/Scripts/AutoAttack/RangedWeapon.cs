using UnityEngine;
using UnityEngine.Pool;

public class RangedWeapon : Weapon, IDamageable
{
    [SerializeField] private Projectile projectileObject;
    [SerializeField] private Transform shotTransform;
    [SerializeField] private int maxPoolSize;
    private IObjectPool<Projectile> projectilePool;
    private GameObject projectileParent;

    protected override void Awake()
    {
        projectileParent = new GameObject($"{name}_Projectiles");
        projectilePool = new ObjectPool<Projectile>
                        (
                            CreateProjectile,
                            GetProjectile,
                            ReleaseProjectile,
                            DestroyProjectile,
                            maxSize: maxPoolSize
                        );

    }

    protected override void OnTriggerEnter(Collider other) { }

    public void DealDamage(ChampBase target)
    {
        DealDamage(target, data.damage, 1);
    }

    // 무기 준비 완료
    public override void OnReady()
    {
        isReady = true;         // 준비 상태 변경 (완료)

        if (data.useClips == null || data.useClips.Count == 0)          // 시전 사운드가 없을 시 return
            return;

        int randomIndex = Random.Range(0, data.useClips.Count);         // 랜덤 인덱스 (평타 준비 완료 사운드)
        SoundManager.Instance.PlayOneShot(data.useClips[randomIndex]);  // 사운드 매니저에서 재생
    }

    public void Shot()
    {
        var projectile = projectilePool.Get();
        projectile.Use(shotTransform);
    }

    private Projectile CreateProjectile()
    {
        if (data == null)
            return null;

        var projectile = Instantiate(projectileObject, projectileParent.transform);
        projectile.SetPool(projectilePool);

        return projectile;
    }

    private void GetProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(true);
    }

    private void ReleaseProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    private void DestroyProjectile(Projectile projectile)
    {
        Destroy(projectile.gameObject);
    }
}
