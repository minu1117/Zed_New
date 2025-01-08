using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Zed : SingletonChampion<Zed>
{
    public GameObject L_Hand_Blade;                     // 좌측 무기
    public GameObject R_Hand_Blade;                     // 우측 무기
    public Dictionary<int, ZedShadow> shadows = new();  // 그림자 스킬 목록
    public SkillSlotManager skillSlotMgr;              // 스킬 슬롯 매니저
    private List<KeyCode> keycodes;                     // 인풋 키 목록
    private bool attackUsing = true;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        keycodes = new();
        var slotDict = skillSlotMgr.GetSlotDict();  // 스킬 슬롯 가져오기

        // 스킬 슬롯 순회
        foreach (var item in slotDict)
        {
            KeyCode code = (KeyCode)System.Enum.Parse(typeof(KeyCode), item.Key, true); // KeyCode 추출
            keycodes.Add(code); // 인풋 키 List에 추출한 키 값 추가
        }
    }

    public void Update()
    {
        // 키코드 목록 순회하며 입력 체크
        foreach (var keycode in keycodes)
        {
            CheckUseSkill(keycode);
        }

        // 일반 공격 입력 체크
        CheckAutoAttack(MouseButton.Left);
    }

    // 스킬 사용 
    // null return == 스킬 사용 X
    public override Skill UseSkill(string keycode, int enumIndex, string layerMask = "")
    {
        if (skillSlotMgr == null)   // 스킬 슬롯 매니저가 없을 시 null return (스킬이 없다는 뜻)
            return null;

        var skillDict = skillSlotMgr.GetSlotDict(); // 스킬 슬롯 가져오기
        if (!skillDict.ContainsKey(keycode))        // 입력한 키가 없을 경우 null return
            return null;

        var skillData = skillDict[keycode].GetExcutor().GetData().skill.data;
        //if (!SubMP(skillData.cost))
        //    return null;
        if (!CheckMP(skillData.cost))
            return null;

        Skill skill = skillDict[keycode].GetExcutor().StartSkill(gameObject, enumIndex, layerMask);    // 사용할 스킬 가져오기

        if (skill != null)
        {
            SubMP(skillData.cost);
        }

        return skill;   // 사용한 스킬 return
    }

    // 평타 입력 체크, 평타 사용
    private void CheckAutoAttack(MouseButton mouseButton)
    {
        if (!attackUsing)
            return;

        if (!Input.GetMouseButtonDown((int)mouseButton))    // 키 입력이 없을 경우 return
            return;

        // 입력 시 마우스가 UI 위에 존재할 경우 return
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        AutoAttack();
    }

    // 스킬 키 입력 체크, 스킬 사용
    private void CheckUseSkill(KeyCode keyCode)
    {
        if (!Input.GetKeyDown(keyCode)) // 키 입력이 없을 경우 return
            return;

        if (!attackUsing)
            return;

        string keycodeStr = EnumConverter.GetString(keyCode);   // KeyCode를 string으로 변환
        ZedSkillType type = skillSlotMgr.GetType(keycodeStr);   // 스킬 타입 가져오기
        UseZedSkill(type, keycodeStr);                          // 스킬 사용
    }

    // 스킬 별로 분기하여 실행
    private void UseZedSkill(ZedSkillType type, string keycode)
    {
        if (type == ZedSkillType.LivingShadow)  // 그림자 스킬일 경우
        {
            UseShadowSkill(type, keycode);      // 그림자 스킬 사용
            return;
        }

        int typeToint = (int)type;

        Skill useSkill = UseSkill(keycode, typeToint, EnumConverter.GetString(CharacterEnum.Enemy));   // 스킬 사용
        if (useSkill == null)   // 사용한 스킬이 null을 반환할 시 return
            return;

        (GameObject, bool) target = (null, false);  // 타겟 정보
        if (useSkill.isTargeting)                   // 타게팅 스킬일 시
        {
            var hit = Raycast.GetHit(Input.mousePosition ,EnumConverter.GetString(CharacterEnum.Enemy));  // 현재 마우스 위치의 적 탐지, 타겟 정보 가져오기
            bool isHit = hit.collider != null;

            if (isHit)
                target = (hit.collider.gameObject, isHit);
        }

        Vector3 point = Raycast.GetMousePointVec();
        CopySkill(keycode, useSkill, type, skillSlotMgr.GetSlotDict()[keycode].GetExcutor(), point, target.Item1);   // 그림자 스킬에 사용한 스킬 전달
        skillSlotMgr.CoolDown(useSkill.data.coolDown);  // 쿨다운 시작
    }

    // 그림자 스킬 사용
    private void UseShadowSkill(ZedSkillType type, string key)
    {
        // 현재 마우스 위치에 그림자 스킬이 있는지 확인
        var hit = Raycast.GetHit(Input.mousePosition, EnumConverter.GetString(CharacterEnum.Shadow));

        if (hit.collider == null)   // 마우스 위치에 그림자 스킬이 없을 시
        {
            FinishedAttack();       // 무기 상태 초기화
            Skill useSkill = UseSkill(key, (int)type, EnumConverter.GetString(CharacterEnum.Enemy));   // 그림자 스킬 사용

            if (useSkill == null)
                return;

            skillSlotMgr.CoolDown(useSkill.data.coolDown);      // 쿨다운 시작
        }
        else  // 마우스 위치에 그림자 스킬이 있을 시
        {
            if (hit.collider.gameObject.TryGetComponent(out ZedShadow shadow))  // 오브젝트에서 그림자 스킬 컴포넌트 추출 성공 시
            {
                TeleportShadow(shadow); // 텔레포트 실행 (서로 자리 바꾸기)
            }
        }
    }

    // 그림자 목록에 그림자 추가
    public void AddShadow(ZedShadow shadow)
    {
        shadows.Add(shadow.GetID(), shadow);    // key : 그림자의 고유 ID, value : 그림자 스킬
    }

    // 그림자 삭제
    public void RemoveShadow(int id)
    {
        shadows.Remove(id);
    }

    // 그림자 목록 가져오기
    public Dictionary<int, ZedShadow> GetShadowDict()
    {
        return shadows;
    }    

    // 스킬 복제 사용 (그림자 스킬이)
    private void CopySkill(string skillKeyStr, Skill useSkill, ZedSkillType type, SkillExcutor excutor, Vector3 point, GameObject target = null)
    {
        if (useSkill == null)   // 사용할 스킬이 null일 경우 return (스킬이 사용되지 않았음)
            return;

        if (shadows.Count > 0)  // 가지고 있는 그림자 개수가 1개 이상일 경우
        {
            foreach (var shadow in shadows)         // 그림자 목록 순회
            {
                shadow.Value.SetCaster(gameObject); // 시전자 설정
                shadow.Value.AddSkill(skillKeyStr, useSkill, type, excutor, point, target); // 사용한 스킬 추가
            }
        }
    }

    // 위치 이동 (서로 자리 바꾸기)
    public void TeleportShadow(ZedShadow shadow)
    {
        var hit = Raycast.GetHit(Input.mousePosition, EnumConverter.GetString(CharacterEnum.Shadow));   // 마우스 위치에 그림자가 있는지 재확인
        if (hit.collider == null || !shadow.isReady)    // 오브젝트가 없을 시 또는 그림자가 목표 지점으로 이동 중일 경우 return
            return;

        shadow.Teleport(gameObject);    // 위치 이동
    }

    public void SetAttackUse(bool set) { attackUsing = set; }

    /********************************************** Animation Event **********************************************/
    // 우측 무기 준비 완료
    public void OnLeftAttack()
    {
        OnAutoAttack(R_Hand_Blade.name);
    }

    // 좌측 무기 준비 완료
    public void OnRightAttack()
    {
        OnAutoAttack(L_Hand_Blade.name);
    }
}
