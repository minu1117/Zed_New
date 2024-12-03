using UnityEngine;
using UnityEngine.Pool;

public class RangedWeapon : Weapon, IDamageable
{
    [SerializeField] protected Projectile projectileObject;
    [SerializeField] protected Transform shotTransform;
    [SerializeField] protected int maxPoolSize;
    protected IObjectPool<Projectile> projectilePool;
    protected GameObject projectileParent;
    protected ChampBase target;
    protected Vector3 targetPos;

    protected override void Awake()
    {
        if (projectileObject == null)
            return;

        if (gameObject.tag == EnumConverter.GetString(CharacterEnum.Enemy))
        {
            target = Zed.Instance;
        }

        projectileParent = new GameObject($"{name}_Projectiles");
        projectilePool = new ObjectPool<Projectile>
                        (
                            () => CreateProjectile(projectileObject, projectilePool, data.damage),
                            GetProjectile,
                            ReleaseProjectile,
                            DestroyProjectile,
                            maxSize: maxPoolSize
                        );

    }

    protected override void OnTriggerEnter(Collider other) { }

    public virtual void DealDamage(ChampBase target, float damage)
    {
        StartCoroutine(DealDamage(target, damage, 1));
    }

    // 무기 준비 완료
    public override void OnReady()
    {
        isReady = true;         // 준비 상태 변경 (완료)

        if (data.useClips == null || data.useClips.Count == 0)          // 시전 사운드가 없을 시 return
            return;

        int randomIndex = Random.Range(0, data.useClips.Count);         // 랜덤 인덱스 (평타 준비 완료 사운드)
        SoundManager.Instance.PlayOneShot(data.useClips[randomIndex]);  // 사운드 매니저에서 재생
        Shot();
    }

    public virtual void Shot()
    {
        var projectile = projectilePool.Get();
        projectile.SetPosition(shotTransform.position);

        ChangeProjectileRotation(projectile);
        projectile.Use();
    }

    protected Projectile CreateProjectile(Projectile projectileObj, IObjectPool<Projectile> pool, float damage)
    {
        if (data == null)
            return null;

        var projectile = Instantiate(projectileObj, projectileParent.transform);
        projectile.SetWeapon(this);
        projectile.SetDamage(damage);
        projectile.SetPool(pool);

        return projectile;
    }

    protected void GetProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(true);
    }

    protected void ReleaseProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    protected void DestroyProjectile(Projectile projectile)
    {
        Destroy(projectile.gameObject);
    }

    public void SetTarget(ChampBase champ) { target = champ; }
    public void SaveCurrentTargetPos()
    {
        if (target == null)
            return;

        targetPos = target.shotStartTransform.position;
    }

    protected void ChangeProjectileRotation(Projectile projectile)
    {
        if (target == null)
        {
            projectile.Look(shotTransform);
        }
        else
        {
            //projectile.LookAt(target.shotStartTransform);
            projectile.LookAt(targetPos);
        }
    }
}
