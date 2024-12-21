using System.Collections.Generic;
using UnityEngine;

public class DraggableSkillCreatorManager : MonoBehaviour
{
    [SerializeField] private List<DraggableSkillCreator> draggableSkillCreators;
    [SerializeField] private List<DraggableSkill> allDraggableSkills;
    private Dictionary<string, DraggableSkill> allDraggableSkillDict;
    private ListWrapper<string> saveDraggableSkills { get; set; }

    private void Awake()
    {
        allDraggableSkillDict = new();
        foreach (var draggableSkill in allDraggableSkills)
        {
            allDraggableSkillDict.Add(draggableSkill.skill.skill.data.skillName, draggableSkill);
        }
    }

    private void Start()
    {
        Load();
    }

    public void AddDraggableSkill(DraggableSkill skill)
    {
        var emptyCreator = GetEmptyCreator();
        if (emptyCreator == null)
            return;

        if (Exist(skill))
            return;

        emptyCreator.SetDraggableSkill(skill);
    }

    private DraggableSkillCreator GetEmptyCreator()
    {
        foreach (var creator in draggableSkillCreators)
        {
            if (creator.draggableSkill == null)
                return creator;
        }

        return null;
    }

    private bool Exist(DraggableSkill skill)
    {
        foreach (var creator in draggableSkillCreators)
        {
            if (creator.draggableSkill == null)
                continue;

            if (creator.draggableSkill.GetSkillName() == skill.GetSkillName())
                return true;
        }

        return false;
    }

    private void Load()
    {
        var loadData = SaveLoadManager.Load<ListWrapper<string>>(SaveLoadMode.SkillTree);
        if (loadData == null || loadData.list == null || loadData.list.Count == 0)
            return;

        foreach (var skillName in loadData.list)
        {
            if (!allDraggableSkillDict.ContainsKey(skillName))
                continue;

            AddDraggableSkill(allDraggableSkillDict[skillName]);
        }
    }

    private void Save()
    {
        saveDraggableSkills = new()
        {
            list = new()
        };

        foreach (var creator in draggableSkillCreators)
        {
            var draggableSkill = creator.draggableSkill;
            if (draggableSkill == null)
                continue;

            var skillButtonData = draggableSkill.skill;
            if (skillButtonData == null)
                continue;

            var skill = skillButtonData.skill;
            if (skill == null)
                continue;

            saveDraggableSkills.list.Add(skill.data.skillName);
        }

        SaveLoadManager.Save(saveDraggableSkills, SaveLoadMode.SkillTree);
    }

    private void OnDestroy()
    {
        Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
