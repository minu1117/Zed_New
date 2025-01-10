using DG.Tweening;
using UnityEngine;

public class Jhin_Q : TargetingSkill
{
    [SerializeField] private int bouncingCount;
    [SerializeField] private float jumpPower;
    [SerializeField] private SkillIndicator skillIndicator;
    [SerializeField] private float addStartDuration;

    private GameObject indicatorParent;
    private SkillIndicator createdIndicator;
    private int currentCount = 1;

    public override void Awake()
    {
        base.Awake();
        indicatorParent = new GameObject($"{data.skillName}_Indicator");
        createdIndicator = Instantiate(skillIndicator, indicatorParent.transform);
    }

    public override void Use(GameObject character)
    {
        if (target == null)
            return;

        base.Use(character);
        Bounce();
    }

    private void Bounce()
    {
        // 회전 애니메이션
        transform.DORotate(new Vector3(360, 0, 360), data.duration * 2, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear);

        float duration = currentCount == 1 ? data.duration + addStartDuration : data.duration;
        if (createdIndicator != null)
        {
            if (indicatorParent != null)
            {
                indicatorParent.SetActive(true);
            }

            UseIndicator(duration);
        }

        transform.DOJump(target.transform.position, jumpPower, 1, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (currentCount < bouncingCount)
                    {
                        currentCount++;
                        isCollide = false;
                        Bounce();
                    }
                    else
                    {
                        currentCount = 1;
                        Release();
                    }
                });
    }

    protected void UseIndicator(float duration)
    {
        // 위치 가져오기
        var pos = target.transform.position;
        pos.y += 0.1f;

        createdIndicator.SetPosition(pos);
        createdIndicator.duration = duration;
        createdIndicator.Use();
    }

    protected override void Release()
    {
        indicatorParent.SetActive(false);
        base.Release();
    }
}
