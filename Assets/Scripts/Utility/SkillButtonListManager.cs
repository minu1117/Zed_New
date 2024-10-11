using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ListManagerSkillButtonData : ListManager<SkillButtonData>
{
    public List<CustomList<SkillButtonData>> GetListOfLists() { return listOfLists; }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ListManagerSkillButtonData))]
public class SkillButtonDataListManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var manager = (ListManagerSkillButtonData)target;

        if (GUILayout.Button("Add New List"))
        {
            var asd = manager.listOfLists;
            manager.listOfLists.Add(new CustomList<SkillButtonData>());
        }

        if (GUILayout.Button("Remove Last List"))
        {
            if (manager.listOfLists.Count > 0)
            {
                manager.listOfLists.RemoveAt(manager.listOfLists.Count - 1);
            }
        }

        base.OnInspectorGUI();
    }
}
#endif
