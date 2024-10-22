using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class Skill : MonoBehaviour, IDamageable
{
    public SkillData data;                          // 스킬 데이터
    public bool isTargeting;                        // 타게팅 여부
    protected IObjectPool<Skill> pool;
    protected Vector3 startPos;                     // 스킬 시작 위치

    protected WaitForSeconds waitUseDelay;
    protected WaitForSeconds waitduration;
    protected WaitForSeconds waitimmobilityTime;

    protected GameObject caster;                    // 시전자
    protected Effect effect;                        // 이펙트
    protected Tweener tweener;

    public virtual void Awake()
    {
        waitUseDelay = new WaitForSeconds(data.useDelay);               // 시전 대기 시간 캐싱
        waitduration = new WaitForSeconds(data.duration);               // 지속 시간 캐싱
        waitimmobilityTime = new WaitForSeconds(data.immobilityTime);   // 스킬 종료 후 경직 시간 캐싱
    }

    // 스킬 사용
    public virtual void Use(GameObject character)
    {
        UseEffect(gameObject);          // 이펙트 사용
        StartSound(data.useClips);      // 스킬 시전 사운드 재생
        StartSound(data.voiceClips);    // 보이스 재생
    }

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
        if (clipList == null || clipList.Count == 0)           // 사운드가 없을 경우 return
            return;

        int index = GetRandomIndex(0, clipList.Count);         // 랜덤 인덱스 (사운드 클립들 중 하나를 재생하기 위함)
        SoundManager.Instance.PlayOneShot(clipList[index]);    // 사운드 매니저에서 시전 사운드들 중 랜덤 인덱스에 위치한 사운드 재생
    }

    public void SetPool(IObjectPool<Skill> pool) { this.pool = pool; }
    public void SetCaster(GameObject obj) { caster = obj; }
    public int GetRandomIndex(int min, int max) { return Random.Range(min, max); }

    // Collision, Trigger 둘 다 사용될 수 있으니 두 가지 메서드를 똑같이 구현
    // 오브젝트 충돌 시 처리될 작업 실행
    protected virtual void OnCollisionEnter(Collision collision)
    {
        Collide(collision.gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Collide(other.gameObject);
    }

    // 오브젝트 충돌 시 처리될 작업
    protected virtual void Collide(GameObject obj)
    {
        if (data.isShadow)  // 그림자 스킬일 경우 return (그림자 스킬 스크립트에서 따로 처리)
            return;

        if (gameObject.TryGetComponent(out ZedShadow shadow))   // 해당 스킬이 그림자 스킬일 경우
        {
            if (!shadow.isReady)    // 스킬 사용 준비가 되지 않았으면 return
                return;
        }

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
    public void DealDamage(ChampBase target, float damage)
    {
        target.OnDamage(damage);    // 타겟에 데미지 부여
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

        // 시전자가 몬스터이고, 타겟도 몬스터일 시 return (몬스터 팀킬 방지)
        string enemyEnumConvert = EnumConverter.GetString(CharacterEnum.Enemy);
        if (caster.tag == enemyEnumConvert && target.tag == enemyEnumConvert)
            return;

        if (target.TryGetComponent(out ChampBase champion))                 // 타겟에서 ChampBase 컴포넌트 추출 성공 시
        {
            DealDamage(champion, data.damage);                              // 타겟에게 데미지 부여

            if (data.attackClips == null || data.attackClips.Count == 0)    // 평타 사운드가 없을 경우 return
                return;

            int index = GetRandomIndex(0, data.attackClips.Count);          // 랜덤 인덱스 (평타 사운드 클립)
            SoundManager.Instance.PlayOneShot(data.attackClips[index]);     // 사운드 매니저에서 평타 사운드 재생
        }
    }

    protected virtual void Release()
    {
        if (pool == null)       // 오브젝트 풀이 설정되지 않았을 경우 return
        {
            tweener.Kill();
            return;
        }

        ReleaseEffect();        // 이펙트 반납
        StartDisappearSound();  // 시전 해제 사운드 재생
        caster = null;          // 시전자 초기화
        pool.Release(this);     // 스킬 반납
    }

    // 시전 해제 사운드 재생
    protected void StartDisappearSound()
    {
        if (data.disappearClips == null || data.disappearClips.Count <= 0)  // 시전 해제 사운드가 없을 경우 return
            return;

        int index = GetRandomIndex(0, data.disappearClips.Count);           // 랜덤 인덱스 (시전 해제 사운드 클립)
        SoundManager.Instance.PlayOneShot(data.disappearClips[index]);      // 사운드 매니저에서 시전 해제 사운드 재생
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
}
