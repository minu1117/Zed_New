using System.Collections.Generic;
using UnityEngine;

public class SkillSlotManager : Singleton<SkillSlotManager>
{
    [SerializeField] private List<SkillButton> buttons;     // 스킬 버튼들
    private Dictionary<string, SkillButton> buttonDict;     // 생성된 스킬 버튼 오브젝트들 저장용

    protected override void Awake()
    {
        base.Awake();
        if (buttons == null || buttons.Count <= 0)
            return;

        buttonDict = new();

        // 스킬 버튼 List 순회
        foreach (var button in buttons)
        {
            button.Init();  // 초기 설정 실행
            buttonDict.Add(EnumConverter.GetString(button.keycode), button);    // Dictionary에 추가
        }
    }

    public void SetImage(string keycode, Sprite sp)
    {
        if (!buttonDict.ContainsKey(keycode))
            return;

        buttonDict[keycode].SetSprite(sp);
    }

    public void CoolDown(float time)
    {

    }

    public ZedSkillType GetType(string key)
    {
        if (!buttonDict.ContainsKey(key))
            return ZedSkillType.None;

        return buttonDict[key].GetSkillType();
    }

    public Dictionary<string, SkillButton> GetSlotDict()
    {
        return buttonDict;
    }
}
