using System.Collections.Generic;
using UnityEngine;

public class SkillSlotManager : MonoBehaviour
{
    [SerializeField] private List<SkillButton> buttons;     // 스킬 버튼들
    [SerializeField] private List<SkillButtonData> playerSkillDatas;      // 플레이어의 모든 스킬
    private Dictionary<string, SkillButton> buttonDict;     // 생성된 스킬 버튼 오브젝트들 저장용
    public static SkillSlotManager Instance;
    private Dictionary<string, string> saveSkills;
    private Dictionary<string, SkillButtonData> skillButtonFindDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = GetComponent<SkillSlotManager>();
        }
        else
        {
            Destroy(gameObject); // 기존 인스턴스가 있으면 파괴
            return;
        }

        InitButtons();
        Load();
    }

    protected void OnDestroy()
    {
        // 씬이 바뀌면 파괴
        if (Instance != null)
        {
            Instance = null;
        }
        Save();
    }

    public void LoadButtonDatas(Dictionary<string, string> loadData)
    {
        skillButtonFindDict = new();
        foreach (var skillButtonData in playerSkillDatas)
        {
            skillButtonFindDict.Add(skillButtonData.skill.data.skillName, skillButtonData);
        }

        foreach (var data in loadData)
        {
            if (!buttonDict.ContainsKey(data.Key))
                continue;

            if (skillButtonFindDict.ContainsKey(data.Key))
                continue;

            var skillButtonData = skillButtonFindDict[data.Value];
            buttonDict[data.Key].SetSkill(skillButtonData);
        }
    }

    public void InitButtons()
    {
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

    public List<SkillButtonData> GetAllPlayerSkillButtonData()
    {
        return playerSkillDatas;
    }

    public void Save()
    {
        saveSkills = new();
        foreach (var item in buttonDict)
        {
            if (item.Value.data == null)
                continue;

            saveSkills.Add(item.Key, item.Value.data.skill.data.skillName);
        }

        SaveLoadManager.Save(saveSkills, SaveLoadMode.Skill);
    }

    public void Load()
    {
        var loadData = SaveLoadManager.LoadDictionary<string>(SaveLoadMode.Skill);
        if (loadData == null)
            return;

        LoadButtonDatas(loadData);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }
}
