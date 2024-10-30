using System.Collections.Generic;
using UnityEngine;

public class SkillDropZoneManager : MonoBehaviour
{
    public List<SkillDropZone> dropZones;
    private Dictionary<string, SkillDropZone> dropZondDict;

    public void Start()
    {
        dropZondDict = new();
        foreach (var dropZone in dropZones)
        {
            dropZondDict.Add(dropZone.GetSkillName(), dropZone);
        }
    }

    public SkillDropZone GetDropZone(string keycode)
    {
        return dropZondDict[keycode];
    }

    public bool Exist(string keycode)
    {
        if (dropZondDict.ContainsKey(keycode))
            return true;

        return false;
    }

    public SkillDropZone GetDropZoneToSkillName(string skillName)
    {
        foreach (var pair in dropZondDict)
        {
            if (pair.Value.GetSkillName() == skillName)
            {
                return pair.Value;
            }
        }

        return null;
    }
}
