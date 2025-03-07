using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChampBase : MonoBehaviour
{
    public CharacterData data;                                      // 캐릭터 데이터
    public AutoAttack autoAttack;                                   // 평타 컴포넌트 (오브젝트에 붙어있음)
    public Transform shotStartTransform;                            // 스킬 발사 시작 위치
    public Transform groundTransform;                               // 바닥 위치
    public List<Weapon> weapons;                                    // 무기 List (인스펙터에서 담아둠)
    public SkillSlot slot;                                          // 스킬 슬롯
    public Effect deadEffect;                                       // 죽었을 때 나오는 이펙트
    public List<AudioClip> deadSounds;                              // 죽었을 때 나오는 사운드들
    private Dictionary<string, Weapon> weaponDict;                  // 무기들을 이름과 같이 담아두는 Dictionary
    protected CharacterAnimationController animationController;     // 애니메이션 컨트롤러
    protected StatusController hpController;                          // hp 컨트롤러
    protected StatusController mpController;                          // mp 컨트롤러
    protected NavMeshAgent agent;
    protected CharacterMoveController moveController;
    public float deadAnimDuration;                                  // 죽는 애니메이션 출력 시간
    protected BoxCollider coll;
    protected bool isDead = false;

    protected virtual void Awake()
    {
        hpController = GetComponent<StatusController>();
        animationController = GetComponent<CharacterAnimationController>();
        agent = GetComponent<NavMeshAgent>();
        moveController = GetComponent<CharacterMoveController>();
        coll = GetComponent<BoxCollider>();

        weaponDict = new(); // 무기 dictionary 초기화
        if (weapons != null && weapons.Count > 0)   // 무기 List에 무기가 있을 경우
        {
            // 무기 List 순회
            foreach (var weapon in weapons)
            {
                weapon.SetDamage(autoAttack.data.damage);   // 데미지 설정
                weapon.SetChamp(this);
                weaponDict.Add(weapon.name, weapon);        // 무기 dictionary에 이름과 함께 추가
            }
        }

        if (slot == null)
        {
            if (gameObject.TryGetComponent(out SkillSlot skillSlot))    // 오브젝트에서 스킬 슬롯 컴포넌트 추출 성공 시
            {
                slot = skillSlot;   // 스킬 슬롯 설정
                slot.Init();        // 스킬 슬롯 초기 설정
            }
        }
        else
        {
            slot.Init();
        }
    }

    // 평타 공격 완료 
    public void FinishedAttack()
    {
        if (weaponDict == null || weaponDict.Count == 0)    // 무기 dictionary가 비었을 경우 return
            return;

        // 무기 dictionry 순회
        foreach (var weapon in weaponDict)
        {
            weapon.Value.OnFinished();  // 무기 상태 초기화
        }
    }

    public void ActiveMove()
    {
        if (agent != null)
            agent.isStopped = false;

        if (moveController != null)
            moveController.isMoved = true;
    }

    // 평타 준비 상태 변경 
    protected void OnAutoAttack(string name)
    {
        weaponDict[name].OnReady(); // 무기를 준비 완료 상태로 변경
    }

    public StatusController GetStatusController(SliderMode mode)
    {
        return mode == SliderMode.HP ? hpController : mpController;
    }

    public void SetStatusController(SliderMode mode, StatusController controller)
    {
        switch (mode)
        {
            case SliderMode.HP:
                hpController = controller;
                break;
            case SliderMode.MP:
                mpController = controller;
                break;
        }
    }

    // 평타 실행
    public virtual void Attack()
    {
        if (animationController != null)                                // 애니메이션 컨트롤러가 있을 경우
            animationController.Attack(autoAttack.data.attackSpeed);    // 평타 애니메이션 실행

        if (agent != null)
            agent.isStopped = true;

        if (moveController != null)
            moveController.StopMove();

        autoAttack.Attack(gameObject);  // 평타 실행
    }

    protected void AutoAttack()
    {
        FinishedAttack();   // 무기 상태 초기화
        Attack();           // 평타 실행
    }

    // 스킬 사용 
    // 실행하는 곳에서도 사용하는 스킬을 알 수 있게 스킬을 return
    public virtual Skill UseSkill(string keycode, int enumIndex, string layerMask = "")
    {
        if (slot == null)   // 스킬 슬롯이 없을 경우 null return
            return null;

        var skillDict = slot.GetSlotDict();     // 스킬 Dictionary
        if (!skillDict.ContainsKey(keycode))    // 찾는 스킬이 없을 경우 null return
            return null;

        Skill skill = skillDict[keycode].StartSkill(gameObject, enumIndex, layerMask); // 스킬 사용
        return skill; // 사용한 스킬 return
    }

    // 데미지 받기
    public virtual void OnDamage(float damage)
    {
        if (hpController.isShield)
        {
            damage = hpController.HitShield(damage);
        }

        if (damage <= 0)
            return;

        float currentValue = hpController.GetCurrentValue();
        if (currentValue - damage >= 0)   // 현재 HP - 받는 데미지가 0 이상일 경우
        {
            hpController.SetCurrentValue(currentValue - damage);
        }
        else                                // 피해를 받았을 때 HP가 0 미만으로 내려가는 경우
        {
            hpController.SetCurrentValue(0);
        }

        if (hpController == null)           // HP 컨트롤러가 없을 경우 return
            return;

        hpController.SetCurrentValue();     // HP 컨트롤러에서 현재 HP 설정
        //if (data.currentHp <= 0)            // 체력이 0 이하일 경우
        if (hpController.GetCurrentValue() <= 0)            // 체력이 0 이하일 경우
        {
            StartCoroutine(OnDead());                       // 사망
        }
    }

    public bool CheckMP(float value)
    {
        if (mpController == null)
            return false;

        float currentValue = mpController.GetCurrentValue();
        if (currentValue <= 0)            // 마나가 0 이하일 경우
            return false;

        if (currentValue - value < 0)     // 현재 마나보다 소모량이 많을 경우
            return false;

        if (mpController == null)
            return false;

        return true;
    }

    public void SubMP(float value)
    {
        if (!CheckMP(value))
            return;

        mpController.SetCurrentValue(mpController.GetCurrentValue() - value);
    }

    public virtual IEnumerator OnDead()
    {
        isDead = true;
        if (coll != null)
        {
            coll.enabled = false;
        }

        if (moveController != null)
        {
            moveController.StopMove();
        }

        if (deadEffect != null)
        {
            EffectManager.Instance.UseEffect(deadEffect, gameObject.transform, true);
        }

        if (deadSounds != null && deadSounds.Count > 0)
        {
            SoundManager.Instance.StartSound(deadSounds);
        }

        if (animationController != null)
        {
            animationController.Dead();
            yield return new WaitForSeconds(deadAnimDuration);
        }

        Destroy(gameObject);
    }

    public void Restart()
    {
        isDead = false;
        if (coll != null)
        {
            coll.enabled = true;
        }

        hpController.SetMaxHp();
        mpController.SetMaxMp();

        moveController.StartMove();
        animationController.Restart();
    }

    public void OnShield(float shieldValue)
    {
        hpController.SetShield(shieldValue);
    }

    public void HitShield(float damage)
    {
        hpController.HitShield(damage);
    }

    public void DestroyShield()
    {
        hpController.DestroyShield();
    }

    public bool GetIsDead() { return isDead; }
    public CharacterAnimationController GetAnimationController() { return animationController; }
    public CharacterMoveController GetMoveController() { return moveController; }
    public SkillSlot GetSlot() { return slot; }

    /********************************************** Animation Event **********************************************/
    public void OnAllWeaponsAttackReady()
    {
        foreach (var weapon in weaponDict)
        {
            weapon.Value.OnReady();
        }
    }
}