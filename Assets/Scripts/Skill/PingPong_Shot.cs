using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PingPong_Shot : Skill
{
    public float waitPongDelay;     // 되돌아오기 전 대기 시간 (가만히 있는 시간)
    public float waitMaterialChangeDelay;   // 메테리얼 바꾼 후 대기 시간

    public RotateType pingRotateType = RotateType.None;
    public RotateType middleRotateType = RotateType.None;
    public RotateType pongRotateType = RotateType.None;

    public Material changeMaterial;     // 변경할 메테리얼
    private MeshRenderer meshRenderer;
    private Material defaultMaterial;   // 기본 메테리얼
    private RotateObject rotateObject;  // 오브젝트 회전 스크립트

    public override void Awake()
    {
        base.Awake();
        rotateObject = GetComponent<RotateObject>();
        meshRenderer = GetComponent<MeshRenderer>();
        defaultMaterial = meshRenderer.material;
    }

    public override void Use(GameObject character)
    {
        base.Use(character);

        ChampBase champBase = character.GetComponent<ChampBase>();
        Transform endTr = champBase.shotStartTransform == null ? champBase.transform : champBase.shotStartTransform;
        StartCoroutine(CoPingPong(endTr, usePoint));    // 날리기 코루틴 실행
    }

    // 날리기
    private IEnumerator CoPingPong(Transform endTr ,Vector3 startVec)
    {
        ChangeMaterial(defaultMaterial);
        ChangeRotate(pingRotateType);
        Vector3 totalMovement = transform.position + (startVec * data.distance); // 날아갈 거리 계산

        if (tweener == null)
        {
            tweener = transform.DOMove(totalMovement, data.duration)  // 지속 시간동안 totalMovement 까지 날아가기
                 .SetEase(Ease.Linear)
                 .SetAutoKill(false)
                 .OnComplete(() => ChangeRotate(middleRotateType));
        }
        else
        {
            RestartTween(transform.position, totalMovement);
        }

        yield return waitimmobilityTime;    // 사용 후 경직 시간동안 대기
        yield return new WaitForSeconds(waitPongDelay); // 되돌아오기 전 제자리에서 대기

        ChangeMaterial(changeMaterial);
        yield return new WaitForSeconds(waitMaterialChangeDelay); // 메테리얼 바꾼 후 대기

        ChangeRotate(pongRotateType);
        float startTime = Time.time;  // 이동 시작 시간
        float distanceToTarget = Vector3.Distance(transform.position, endTr.position);
        while (distanceToTarget > 1f)  // 목표 지점에 가까워질 때까지
        {
            distanceToTarget = Vector3.Distance(transform.position, endTr.position);

            // 이동할 비율 계산 (시간에 비례)
            float distanceCovered = (Time.time - startTime) * data.speed;  // 이동한 거리
            float fractionOfJourney = distanceCovered / distanceToTarget;  // 여행 비율

            // 목표 지점으로 일정한 속도로 이동
            transform.position = Vector3.Lerp(transform.position, endTr.position, fractionOfJourney);

            yield return null;
        }

        Release();
    }

    // 오브젝트 풀에 Release 하는 용도
    protected override void Release()
    {
        base.Release();
    }
    
    private void ChangeRotate(RotateType rotType)
    {
        if (rotateObject == null)
            return;

        rotateObject.SetRotateVector(rotType);

        if (rotType == RotateType.None)
            rotateObject.SetDefalutRotation();
    }

    private void ChangeMaterial(Material mat)
    {
        if (meshRenderer == null)
            return;

        meshRenderer.material = mat;
    }
}
