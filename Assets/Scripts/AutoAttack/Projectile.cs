using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float duration;

    private RangedWeapon rangedWeapon;
    private BoxCollider coll;
    private IObjectPool<Projectile> releasePool;

    private bool isMoved = false;
    private float currentTime = 0f;
    private float damage = 0f;

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
            if (rangedWeapon.gameObject.activeSelf)
            {
                rangedWeapon.DealDamage(champion, damage);
            }
            Release();
        }
    }

    public void Use()
    {
        coll.enabled = true;
        currentTime = 0f;
        isMoved = true;
    }

    public void Update()
    {
        if (isMoved)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            currentTime += Time.deltaTime;
        }

        if (isMoved && currentTime >= duration)
        {
            currentTime = 0f;
            isMoved = false;
            Release();
        }
    }

    private void Release()
    {
        coll.enabled = false;
        if (releasePool != null)
        {
            releasePool.Release(this);
        }
    }

    public void Look(Transform tr)
    {
        Quaternion targetRotation = Quaternion.LookRotation(tr.forward);
        transform.rotation = targetRotation;
    }

    public void LookAt(Transform tr)
    {
        transform.LookAt(tr);
    }
    public void LookAt(Vector3 pos)
    {
        transform.LookAt(pos);
    }

    public void SetDamage(float dmg) { damage = dmg; }
    public void SetPosition(Vector3 pos) { transform.position = pos; }
    public void SetWeapon(RangedWeapon weapon) { rangedWeapon = weapon; }
    public void SetPool(IObjectPool<Projectile> pool) { releasePool = pool; }
}
