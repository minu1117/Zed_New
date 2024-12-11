using System.Collections.Generic;
using UnityEngine;

public class DraggableSkillCreatorManager : MonoBehaviour
{
    [SerializeField] private List<DraggableSkillCreator> draggableSkillCreators;

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
}
