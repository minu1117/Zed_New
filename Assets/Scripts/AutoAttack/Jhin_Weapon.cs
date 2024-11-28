using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Jhin_Weapon : RangedWeapon
{
    [SerializeField] protected Projectile finalShotObject;

    [SerializeField] protected List<AudioClip> twoVoiceClips;
    [SerializeField] protected List<AudioClip> threeVoiceClips;
    [SerializeField] protected List<AudioClip> fourVoiceClips;
    [SerializeField] protected List<AudioClip> fourSFXClips;

    protected IObjectPool<Projectile> finalShotPool;
    private int shootCount = 0;
    private int maxCount = 3;

    protected override void Awake()
    {
        base.Awake();
        if (finalShotObject == null)
            return;

        target = Zed.Instance;
        finalShotPool = new ObjectPool<Projectile>
                        (
                            () => CreateProjectile(finalShotObject, finalShotPool, data.damage * 2),
                            GetProjectile,
                            ReleaseProjectile,
                            DestroyProjectile,
                            maxSize: maxPoolSize
                        );
    }

    public override void Shot()
    {
        Projectile projectile = null;

        if (shootCount < maxCount)
        {
            projectile = projectilePool.Get();
            shootCount++;
        }
        else
        {
            projectile = finalShotPool.Get();
            shootCount = 0;
        }

        if (projectile != null)
        {
            projectile.SetPosition(shotTransform.position);
            ChangeProjectileRotation(projectile);
            projectile.Use(shotTransform);
        }
    }

    public override void OnReady()
    {
        isReady = true;

        List<AudioClip> voiceClips = null;
        List<AudioClip> useClips = data.useClips;
        switch (shootCount)
        {
            case 1:
                voiceClips = twoVoiceClips;
                break;
            case 2:
                voiceClips = threeVoiceClips;
                break;
            case 3:
                voiceClips = fourVoiceClips;
                useClips = fourSFXClips;
                break;
        }

        if (Exist(voiceClips))
        {
            int randomIndex = GetRandomIndex(voiceClips.Count);
            SoundManager.Instance.PlayOneShot(voiceClips[randomIndex]);
        }

        if (Exist(useClips))
        {
            int randomIndex = GetRandomIndex(useClips.Count);
            SoundManager.Instance.PlayOneShot(useClips[randomIndex]);
        }

        Shot();
    }

    public override void DealDamage(ChampBase target, float damage)
    {
        StartCoroutine(DealDamage(target, damage, 1));
    }

    private int GetRandomIndex(int max) { return Random.Range(0, max); }
    private bool Exist<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
            return false;

        return true;
    }

    public bool GetIsLast() { return maxCount == shootCount; }
    public int GetCurrentShootCount() { return shootCount; }
    public int GetMaxShootCount() { return maxCount; }
}
