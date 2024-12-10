using UnityEngine;
using UnityEngine.UI;

public class SkillCoolDown : MonoBehaviour
{
    public Image cooldownImage;      // 쿨다운 UI 이미지
    private float cooldownTimer;
    private float cooldownTime;
    private bool isCooldown = false;

    private void Awake()
    {
        cooldownImage.fillAmount = 0f;
    }

    public void StartCoolDown(float duration)
    {
        cooldownTimer = duration;
        cooldownTime = duration;
        cooldownImage.fillAmount = 1f;
        isCooldown = true;
    }

    private void Update()
    {
        if (!isCooldown)
            return;

        // 쿨다운 중일 때 타이머 감소
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownImage.fillAmount = cooldownTimer / cooldownTime; // 비율 반전
        }
        else
        {
            cooldownImage.fillAmount = 0f; // 쿨다운 완료 시 0으로 고정
        }
    }
}
