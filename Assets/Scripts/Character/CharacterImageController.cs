using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharacterImageController : MonoBehaviour
{
    public Image image;             // 캐릭터 이미지
    public Color shadowColor;       // 그림자 졌을 때 이미지 색
    public Color originalColor;     // 원래 색

    public float jumpPower;         // 이미지가 점프하는 힘 (속도)
    public int jumpCount;           // 점프 회수
    public float jumpDuration;      // 점프 시간

    public float shakeDuration;     // 이미지가 흔들리는 시간
    public float shakeStrength;     // 흔들리는 힘 (속도)
    public int shakeVibrato;        // 흔들리는 회수
    private Sequence jumpSequence;
    private Tweener shakeTweener;

    // 이미지 컬러 조정
    public void AdjustImageColor(Image notTalkingCharacterImage)
    {
        SetShadowImage(notTalkingCharacterImage);       // 대화하고 있지 않은 캐릭터의 이미지 그림자 처리
        SetImageOriginalColor(image, originalColor);    // 대화 중인 캐릭터의 이미지 원래 색으로 변경
    }

    public void SetActive(bool active)
    {
        if (image.gameObject.activeSelf == active)
            return;

        image.gameObject.SetActive(active);
    }

    public void SetImage(Sprite sp)
    {
        if (sp == null)
            return;

        image.gameObject.SetActive(true);
        image.sprite = sp;
    }

    // 이미지 그림자 처리
    private void SetShadowImage(Image image)
    {
        if (image.gameObject.activeSelf == false)
            return;

        image.color = shadowColor;
    }

    private void SetImageOriginalColor(Image image, Color originalColor)
    {
        image.color = originalColor;
    }

    // 이미지 점프
    public void JumpVertically()
    {
        if (jumpSequence == null)
            jumpSequence = image.transform.DOJump(transform.position, jumpPower, jumpCount, jumpDuration);
        else
            jumpSequence.Restart();

    }

    // 이미지 흔들림
    public void Shake()
    {
        if (shakeTweener == null)
            shakeTweener = image.transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, 90, false, true);
        else
            shakeTweener.Restart();
    }
}
