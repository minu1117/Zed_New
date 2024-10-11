using UnityEngine;
using UnityEngine.Pool;

public class EffectPool : MonoBehaviour
{
    [SerializeField] private Effect effect;     // 사용할 이펙트
    [SerializeField] private int maxPoolSize;   // 최대 오브젝트 풀 사이즈
    private IObjectPool<Effect> pool;

    private void Awake()
    {
        pool = new ObjectPool<Effect>
                (
                    CreateEffect,
                    GetEffect,
                    ReleaseEffect,
                    DestroyEffect,
                    maxSize: maxPoolSize
                );
    }

    public IObjectPool<Effect> GetPool()
    {
        return pool;
    }

    public string GetEffectName()
    {
        return effect.name;
    }

    // 이펙트 가져오기
    public Effect Get()
    {
        if (pool == null)   // 오브젝트 풀이 없을 경우 return (이펙트가 생성되지 않았다는 것)
            return null;

        return pool.Get();
    }

    // 오브젝트 풀의 Create
    private Effect CreateEffect()
    {
        var useEffect = Instantiate(effect, transform);
        return useEffect;
    }

    // 오브젝트 풀의 Get
    private void GetEffect(Effect eft)
    {
        eft.gameObject.SetActive(true);
    }

    // 오브젝트 풀의 Release
    private void ReleaseEffect(Effect eft)
    {
        eft.Stop();
    }

    // 오브젝트 풀의 Destroy
    private void DestroyEffect(Effect eft)
    {
        Destroy(eft.gameObject);
    }
}
