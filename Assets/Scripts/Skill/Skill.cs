using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System.Collections;

public class Skill : MonoBehaviour, IDamageable
{
    public SkillData data;                          // 스킬 데이터
    public bool isTargeting;                        // 타게팅 여부
    protected IObjectPool<Skill> pool;
    protected Vector3 startPos;                     // 스킬 시작 위치

    protected WaitForSeconds waitUseDelay;
    protected WaitForSeconds waitduration;
    protected WaitForSeconds waitimmobilityTime;
    protected WaitForSeconds hitInterval;

    protected GameObject caster;                    // 시전자
    protected Effect effect;                        // 이펙트
    protected Tweener tweener;

    protected SkillIndicator indicator;
    protected List<ChildCollider> colliders;
    public bool isCollide { get; set; } = false;
    protected Vector3 usePoint;

    public virtual void Awake()
    {
        colliders = new();

        var childColliders = GetComponentsInChildren<ChildCollider>();
        foreach (var coll in childColliders)
        {
            colliders.Add(coll);
        }

        waitUseDelay = new WaitForSeconds(data.useDelay);               // 시전 대기 시간 캐싱
        waitduration = new WaitForSeconds(data.duration);               // 지속 시간 캐싱
        waitimmobilityTime = new WaitForSeconds(data.immobilityTime);   // 스킬 종료 후 경직 시간 캐싱
        hitInterval = new WaitForSeconds(data.hitInterval);
    }

    // 스킬 사용
    public virtual void Use(GameObject character)
    {
        UseEffect(gameObject);          // 이펙트 사용
        StartSound(data.useClips);      // 스킬 시전 사운드 재생
        StartSound(data.voiceClips);    // 시전 보이스 재생
    }

    public Vector3 GetUsePoint() { return usePoint; }

    // 이펙트 Release
    protected void ReleaseEffect()
    {
        if (effect == null) // 이펙트가 없을 시 return
            return;

        EffectManager.Instance.ReleaseEffect(effect);   // 이펙트 매니저의 오브젝트 풀에 반납
        effect = null;
    }

    // 이펙트 사용
    protected void UseEffect(GameObject obj)
    {
        if (data.effect == null)    // 데이터에 이펙트가 없을 시 return
            return;

        effect = EffectManager.Instance.GetEffect(data.effect.name);    // 이펙트 매니저에서 이펙트 가져오기
        effect.SetStartPos(obj.transform.position);                     // 이펙트 시작 위치 지정
        effect.SetForward(obj.transform.forward);

        if (effect.TryGetComponent<TargetFollowEffect>(out var followEffect))   // 이펙트 오브젝트에서 TargetFollowEffect 컴포넌트 추출 성공 시
        {
            if (data.isSelf)
            {
                followEffect.SetTarget(caster);
            }
            else
            {
                followEffect.SetTarget(obj);    // 이펙트가 따라다닐 타겟 지정
            }
            effect = followEffect;          // 이펙트 할당
        }

        effect.Use();   // 이펙트 사용
    }

    protected void StartSound(List<AudioClip> clipList)
    {
        SoundManager.Instance.StartSound(clipList);
    }

    public void SetPool(IObjectPool<Skill> pool) { this.pool = pool; }
    public void SetCaster(GameObject obj) { caster = obj; }

    // 오브젝트 충돌 시 처리될 작업
    public virtual void Collide(GameObject obj)
    {
        if (data.isShadow)  // 그림자 스킬일 경우 return (그림자 스킬 스크립트에서 따로 처리)
            return;

        if (gameObject.TryGetComponent(out ZedShadow shadow))   // 해당 스킬이 그림자 스킬일 경우
        {
            if (!shadow.isReady)    // 스킬 사용 준비가 되지 않았으면 return
                return;
        }

        if (isCollide)
            return;

        DealDamage(obj);    // 데미지 처리
    }

    public void SetStartPos(Vector3 pos)
    {
        startPos = pos;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    // 데미지 처리
    public virtual IEnumerator DealDamage(ChampBase target, float damage, int hitRate)
    {
        int count = 0;
        bool isSound = data.attackClips != null && data.attackClips.Count > 0;
        while (count < hitRate)
        {
            target.OnDamage(damage);    // 타겟에 데미지 부여

            if (isSound)
            {
                SoundManager.Instance.StartSound(data.attackClips);  // 사운드 매니저에서 타격 재생
            }

            count++;
            yield return hitInterval;
        }
    }

    // 타겟에 데미지 부여
    private void DealDamage(GameObject target)
    {
        if (caster != null && ReferenceEquals(caster, target))  // 타겟과 시전자가 같으면 return (본인은 맞지 않게)
            return;

        if (target == null || caster == null)                   // 타겟 또는 시전자가 없으면 return
            return;

        // 타겟의 태그가 Shadow(그림자)일 경우 return (그림자 스킬은 맞지 않음)
        if (target.tag == EnumConverter.GetString(CharacterEnum.Shadow))
            return;

        // 그림자가 시전한 스킬이고, 타겟이 플레이어면 return (플레이어 그림자가 플레이어를 타격하는 현상 방지)
        if (caster.tag == EnumConverter.GetString(CharacterEnum.Shadow) && target.tag == EnumConverter.GetString(CharacterEnum.Player))
            return;

        // 시전자가 몬스터이고, 타겟도 몬스터일 시 return (몬스터 팀킬 방지)
        string enemyEnumConvert = EnumConverter.GetString(CharacterEnum.Enemy);
        if (caster.tag == enemyEnumConvert && target.tag == enemyEnumConvert)
            return;

        if (target.TryGetComponent(out ChampBase champion))                 // 타겟에서 ChampBase 컴포넌트 추출 성공 시
        {
            StartCoroutine(DealDamage(champion, data.damage, data.hitRate));                // 타겟에게 데미지 부여
            isCollide = true;                                               // 부딪힘 여부 활성화
        }
    }

    protected virtual void Release()
    {
        if (pool == null)       // 오브젝트 풀이 설정되지 않았을 경우 return
        {
            tweener.Kill();
            return;
        }

        isCollide = false;
        ReleaseEffect();        // 이펙트 반납
        StartDisappearSound();  // 시전 해제 사운드 재생
        StartSound(data.complateClips);
        StartSound(data.complateVoiceClips);
        caster = null;          // 시전자 초기화
        pool.Release(this);     // 스킬 반납
    }

    // 시전 해제 사운드 재생
    protected void StartDisappearSound()
    {
        SoundManager.Instance.StartSound(data.disappearClips);
    }

    protected void RestartTween(Vector3 startPos, Vector3 endPos)
    {
        tweener.ChangeStartValue(startPos);
        tweener.ChangeEndValue(endPos);
        tweener.Restart();
    }

    public void KillTween()
    {
        tweener.Kill();
    }

    public void SetPoint(Vector3 point) { usePoint = point; }
}
