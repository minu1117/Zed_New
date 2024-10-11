using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSkillCreator : MonoBehaviour, IPointerEnterHandler
{
    public Canvas createdCanvas;                    // 드래그 될 스킬 오브젝트의 부모로 설정할 캔버스
    public DraggableSkill draggableSkill;           // 드래그 될 스킬
    private Image img;
    private DraggableSkill createdDraggableSkill;   // 생성된 드래그 스킬 오브젝트 저장용
    private RectTransform rectTransform;
    private Canvas parentCanvas;                    // 생성기의 부모 캔버스

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        img = GetComponent<Image>();
        parentCanvas = GetComponentInParent<Canvas>();

        Create();   // 드래그 될 스킬 오브젝트 생성
    }

    // 드래그 될 스킬을 현재 위치로 이동시키기
    private void MoveCurrentPos()
    {
        if (createdDraggableSkill == null)
            return;

        createdDraggableSkill.GetRectTransform().position = rectTransform.position;
    }

    // 드래그 될 스킬 생성
    private void Create()
    {
        createdDraggableSkill = Instantiate(draggableSkill);
        createdDraggableSkill.transform.SetParent(createdCanvas.transform);
        createdDraggableSkill.GetRectTransform().sizeDelta = rectTransform.sizeDelta;   // 똑같은 사이즈로 설정
        createdDraggableSkill.GetRectTransform().position = rectTransform.position;     // 위치 맞추기
        createdDraggableSkill.GetRectTransform().localScale = Vector3.one;              // 스케일 값 맞추기
        createdDraggableSkill.SetCanvas(parentCanvas);                                  // 부모 캔버스 설정

        img.sprite = createdDraggableSkill.GetImage().sprite;                           // 생성기의 이미지를 드래그 될 스킬의 이미지로 변경
    }

    // 마우스가 이미지 영역 안에 들어왔을 경우 실행
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 드래그 될 스킬이 활성화 되어 있을 경우 return (드래그 중이기 때문에)
        if (createdDraggableSkill.gameObject.activeSelf)
            return;

        MoveCurrentPos();   // 생성기 위치로 드래그 될 스킬 오브젝트 이동 (위치 다시 맞추기)
        createdDraggableSkill.gameObject.SetActive(true); // 드래그 될 스킬 활성화
    }
}
