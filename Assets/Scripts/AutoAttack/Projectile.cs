using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float duration;

    private RangedWeapon rangedWeapon;
    private BoxCollider coll;
    private IObjectPool<Projectile> releasePool;

    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == rangedWeapon.GetChamp().tag)  // 같은 태그일 시 return (팀킬 방지)
            return;

        if (other.TryGetComponent(out ChampBase champion))
        {
            rangedWeapon.DealDamage(champion);
            Release();
        }
    }

    public void Use(Transform tr)
    {
        StartCoroutine(CoUse(tr));
    }

    private IEnumerator CoUse(Transform tr)
    {
        var forward = tr.forward;
        float startTime = Time.time; // 시작 시간 기록

        while (Time.time - startTime < duration)
        {
            transform.position += forward * speed * (Time.time - startTime);
            yield return null;
        }

        Release();
    }

    private void Release()
    {
        coll.enabled = false;
        if (releasePool != null)
        {
            releasePool.Release(this);
        }
    }

    public void SetWeapon(RangedWeapon weapon) { rangedWeapon = weapon; }
    public void SetPool(IObjectPool<Projectile> pool) { releasePool = pool; }
}
