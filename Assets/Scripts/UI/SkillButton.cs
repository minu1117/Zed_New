using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public KeyCode keycode;         // 스킬을 사용할 키
    public TextMeshProUGUI tmp;     // 스킬 키 텍스트
    private Image img;              // 스킬 이미지
    private SkillExcutor excutor;   // 스킬 생성기
    public SkillButtonData data;    // 스킬 데이터

    public void Init()
    {
        img = GetComponent<Image>();
        tmp.text = EnumConverter.GetString(keycode).ToUpper();
        SetSprite(data.sp);

        if (excutor == null)
            excutor = GetComponent<SkillExcutor>();

        excutor.Init(SkillSlotManager.Instance.gameObject, data);   // 스킬 생성기 초기 설정 실행
    }

    public void SetSprite(Sprite sp)
    {
        if (img == null)
            return;

        img.sprite = sp;
    }

    public void SetData(SkillButtonData data)
    {
        this.data = data;
    }

    public ZedSkillType GetSkillType()
    {
        var zedSkillData = data as ZedSkillButtonData;
        if (zedSkillData == null)
            return ZedSkillType.None;

        return zedSkillData.type;
    }

    public SkillExcutor GetExcutor()
    {
        return excutor;
    }
}
