using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ShotSkill : Skill
{
    public TrailRenderer trailRenderer; // 따라다닐 TrailRenderer

    public override void Use(GameObject character)
    {
        StartSound(data.voiceClips);    // 시전 보이스 재생
        StartSound(data.useClips);      // 스킬 시전 사운드 재생
        SetActiveTrailRenderer(true);   // TrailRenderer 활성화
        StartCoroutine(CoShot(usePoint));    // 날리기 코루틴 실행
    }

    protected void SetActiveTrailRenderer(bool active)
    {
        if (trailRenderer == null)
            return;

        trailRenderer.Clear();
        trailRenderer.enabled = active;
    }

    // 날리기
    private IEnumerator CoShot(Vector3 startVec)
    {
        isCollide = true;
        foreach (var coll in colliders)
        {
            coll.GetCollider().enabled = false;
        }

        yield return waitUseDelay;

        if (data.isCameraShake)
        {
            GameSceneManager.Instance.GetCameraChakeController().ShakeCamera();
        }

        foreach (var coll in colliders)
        {
            coll.GetCollider().enabled = true;
        }

        isCollide = false;
        Vector3 totalMovement = transform.position + (startVec.normalized * data.duration * data.speed); // 날아갈 거리 계산
        UseEffect(gameObject);          // 이펙트 사용

        if (tweener == null)
        {
            tweener = transform.DOMove(totalMovement, data.duration)  // 지속 시간동안 totalMovement 까지 날아가기
                 .SetEase(Ease.Linear)
                 .SetAutoKill(false)
                 .OnComplete(() => Release());
        }
        else
        {
            RestartTween(transform.position, totalMovement);
        }

        yield return waitimmobilityTime;    // 사용 후 경직 시간동안 대기
    }

    //// 오브젝트 충돌 시 처리될 작업
    //public override void Collide(GameObject obj)
    //{
    //    if (data.isShadow)  // 그림자 스킬일 경우 return (그림자 스킬 스크립트에서 따로 처리)
    //        return;

    //    if (gameObject.TryGetComponent(out ZedShadow shadow))   // 해당 스킬이 그림자 스킬일 경우
    //    {
    //        if (!shadow.isReady)    // 스킬 사용 준비가 되지 않았으면 return
    //            return;
    //    }

    //    if (!isPiercing)
    //    {
    //        if (isCollide)
    //            return;
    //    }

    //    DealDamage(obj);    // 데미지 처리
    //}

    // 오브젝트 풀에 Release 하는 용도
    protected override void Release()
    {
        SetActiveTrailRenderer(false);  // TrailRenderer 비활성화
        base.Release();
    }
}
