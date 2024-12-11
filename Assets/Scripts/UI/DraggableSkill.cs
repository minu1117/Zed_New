using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DraggableSkill : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerExitHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image img;
    private SortingGroup sortingGroup;

    public ZedSkillButtonData skill;    // 스킬 칸에 적용할 스킬

    private int minOrder = 1;
    private int maxOrder = 999;

    private bool isDrag;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        sortingGroup = GetComponent<SortingGroup>();
        img = GetComponent<Image>();

        if (skill != null)
            img.sprite = skill.sp;

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    // 드래그 시작 시 처음 한 번 실행되는 이벤트
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (skill == null)
            return;

        isDrag = true;                          // 드래그 상태 활성화
        canvasGroup.alpha = 0.6f;               // 알파 값 조정 (흐리게)
        canvasGroup.blocksRaycasts = false;     // 입력 비활성화 (오동작 방지)
        sortingGroup.sortingOrder = maxOrder;   // 맨 앞으로 보이게 설정
    }

    // 드래그 중 실행되는 이벤트
    public void OnDrag(PointerEventData eventData)
    {
        if (skill == null)
            return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    // 드래그를 끝낸 후 한 번 실행되는 이벤트
    public void OnEndDrag(PointerEventData eventData)
    {
        if (skill == null)
            return;

        OnReset(); // 리셋
    }

    // 마우스 포인트가 이미지를 벗어났을 때 실행되는 이벤트
    public void OnPointerExit(PointerEventData eventData)
    {
        if (skill == null)
            return;

        if (isDrag) // 드래그 중일 경우 return
            return;

        OnReset(); // 리셋
    }

    private void OnReset()
    {
        isDrag = false;                         // 드래그 상태 비활성화
        canvasGroup.alpha = 0f;                 // 알파 값 조정 (보이지 않게)
        sortingGroup.sortingOrder = minOrder;   // 맨 뒤로 보이게 설정
        canvasGroup.blocksRaycasts = true;      // 입력 활성화
        gameObject.SetActive(false);            // 오브젝트 비활성화
    }

    public Image GetImage()
    {
        return img;
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }

    public void SetCanvas(Canvas canvas)
    {
        this.canvas = canvas;
    }

    public string GetSkillName()
    {
        return skill.skill.data.skillName;
    }

    public Sprite GetSkillSprite()
    {
        return skill.sp;
    }
}
