using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    public List<EffectPool> effectPools;                    // 이펙트를 담고 있는 이펙트 오브젝트 풀
    private Dictionary<string, EffectPool> effectPoolDict;  // 이펙트 풀을 생성하여 저장할 Dictionary

    protected override void Awake()
    {
        base.Awake();
        CreateEffectPools();
    }

    // 이펙트 풀 생성
    private void CreateEffectPools()
    {
        effectPoolDict = new();
        foreach (var pool in effectPools)   // 이펙트 풀 List 순회
        {
            var effectPool = Instantiate(pool);                             // 이펙트 풀 오브젝트 생성
            effectPool.transform.SetParent(gameObject.transform);           // 오브젝트 부모 변경 (매니저 오브젝트 하위로)
            effectPoolDict.Add(effectPool.GetEffectName(), effectPool);     // Dictionary에 추가 -> (이펙트 이름, 이펙트 풀)
        }
    }

    // 이펙트 가져오기
    public Effect GetEffect(string effectName)
    {
        if (effectPoolDict == null || effectPoolDict.Count == 0)    // 이펙트를 저장한 Dictionary가 비었을 경우 return (이펙트 없음)
            return null;

        if (!effectPoolDict.ContainsKey(effectName))
            return null;

        var eft = effectPoolDict[effectName].Get(); // Dictionary에서 가져오기
        eft.ResetParticle();    // 이펙트 초기화
        return eft;
    }

    // 이펙트 오브젝트 풀에 Release 해주기
    public void ReleaseEffect(Effect eft)
    {
        string effectName = eft.name;       // 이펙트 이름
        if (effectName.EndsWith("(Clone)")) // 이름 뒤에 "(Clone)"이 붙었을 경우
        {
            effectName = effectName.Replace("(Clone)", "").Trim(); // "(Clone)" 제거 ((Clone)이 붙지 않은 상태로 저장되기 때문에 Dictionary에 (Clone)이 붙은 key는 없음)
        }

        var pool = effectPoolDict[effectName].GetPool();    // 이펙트 풀 가져오기
        pool.Release(eft);                                  // 이펙트 Release
    }

    // 이펙트 사용
    public Effect UseEffect(Effect effectPrefab, Transform transform)
    {
        if (effectPrefab == null)
            return null;

        var effect = GetEffect(effectPrefab.name);  // 이펙트 가져오기
        effect.SetStartPos(transform.position);     // 이펙트 위치 설정
        effect.SetForward(transform.forward);       // 이펙트의 Forward 설정
        effect.Use();                               // 이펙트 사용

        return effect;
    }

    // 이펙트 사용
    public Effect UseEffect(Effect eft, Transform tr, bool autoRelease, GameObject target = null)
    {
        if (eft == null)    // 데이터에 이펙트가 없을 시 return
            return null;

        eft = GetEffect(eft.name);
        eft.SetStartPos(tr.position);
        eft.SetForward(tr.forward);
        eft.SetAutoRelease(autoRelease);

        if (eft.TryGetComponent<TargetFollowEffect>(out var followEffect))   // 이펙트 오브젝트에서 TargetFollowEffect 컴포넌트 추출 성공 시
        {
            followEffect.SetTarget(target);
            followEffect.Use();
            return followEffect;
        }

        eft.Use();   // 이펙트 사용
        return eft;
    }
}
