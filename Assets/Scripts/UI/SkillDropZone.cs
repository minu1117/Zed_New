using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDropZone : MonoBehaviour, IDropHandler
{
    // 스킬이 해당 위치에 드롭되면 스킬 할당
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            var dragInfo = eventData.pointerDrag.GetComponent<DraggableSkill>();    // 드래그 데이터에서 드래그 스킬 추출
            if (dragInfo == null)                                                   // 드래그 스킬이 없을 시 return
                return;

            var skillButton = GetComponent<SkillButton>();  // 게임오브젝트에서 스킬 버튼 컴포넌트 추출
            if (skillButton == null)                        // 스킬 버튼 컴포넌트가 붙어있지 않을 시 return
                return;

            skillButton.SetData(dragInfo.skill);    // 스킬 버튼에 스킬 데이터 할당
            skillButton.Init();
        }
    }
}
