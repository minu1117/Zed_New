using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDropZone : MonoBehaviour, IDropHandler
{
    private SkillDropZoneManager mgr;
    private SkillButton skillButton;

    private void Awake()
    {
        mgr = GetComponentInParent<SkillDropZoneManager>();
        skillButton = GetComponent<SkillButton>();
    }

    // 스킬이 해당 위치에 드롭되면 스킬 할당
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            var dragInfo = eventData.pointerDrag.GetComponent<DraggableSkill>();    // 드래그 데이터에서 드래그 스킬 추출
            if (dragInfo == null)                                                   // 드래그 스킬이 없을 시 return
                return;

            if (skillButton == null)                // 스킬 버튼 컴포넌트가 붙어있지 않을 시 return
                return;

            string skillName = dragInfo.GetSkillName();
            var dropZone = mgr.GetDropZoneToSkillName(skillName);

            if (dropZone != null)
            {
                dropZone.ResetDropZone();
            }

            skillButton.SetSkill(dragInfo.skill);
        }
    }

    public string GetKeyCode()
    {
        if (skillButton == null)
            return string.Empty;

        return skillButton.GetKeyCode();
    }

    public void ResetDropZone()
    {
        skillButton.ResetButton();
    }

    public string GetSkillName()
    {
        if (skillButton.data == null)
            return "-";

        return skillButton.data.skill.data.skillName;
    }

    public SkillButtonData GetSkillData() { return skillButton.data; }
}
