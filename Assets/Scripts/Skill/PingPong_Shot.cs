using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PingPong_Shot : Skill
{
    public float waitReturnDelay;     // 되돌아오기 전 대기 시간 (가만히 있는 시간)
    public float waitMaterialChangeDelay;   // 메테리얼 바꾼 후 대기 시간

    public RotateType startRotateType = RotateType.None;
    public RotateType middleRotateType = RotateType.None;
    public RotateType returnRotateType = RotateType.None;

    public Material changeMaterial;     // 변경할 메테리얼
    private MeshRenderer meshRenderer;
    private Material defaultMaterial;   // 기본 메테리얼
    private RotateObject rotateObject;  // 오브젝트 회전 스크립트

    public bool changeReturnDefaultMat;   // 돌아올 때 기본 메테리얼로 변경할지 여부
    public bool returnLookAtTarget;     // 돌아올 때 타겟을 바라볼 지 여부
    public bool isSplitHit;             // 시작, 중간, 돌아오기 3가지를 분리해서 타격할지 여부

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
        StartCoroutine(CoPingPong(champBase, usePoint));
    }

    // 날리기
    private IEnumerator CoPingPong(ChampBase champBase, Vector3 startVec)
    {
        ChangeMaterial(defaultMaterial);
        ChangeRotate(startRotateType);
        Vector3 totalMovement = transform.position + (startVec * data.distance); // 날아갈 거리 계산

        if (tweener == null)
        {
            tweener = transform.DOMove(totalMovement, data.duration)  // 지속 시간동안 totalMovement 까지 날아가기
                 .SetEase(Ease.Linear)
                 .SetAutoKill(false)
                 .OnComplete(() =>
                 {
                     ChangeRotate(middleRotateType);

                     // 중간 대기 시간에 타격 판정이 일어나지 않게
                     if (isSplitHit)
                     {
                         isCollide = true;
                     }
                 });
        }
        else
        {
            RestartTween(transform.position, totalMovement);
        }

        yield return waitimmobilityTime;    // 사용 후 경직 시간동안 대기
        yield return new WaitForSeconds(waitReturnDelay); // 되돌아오기 전 제자리에서 대기

        ChangeMaterial(changeMaterial);
        yield return new WaitForSeconds(waitMaterialChangeDelay); // 메테리얼 바꾼 후 대기

        // 되돌아올 때 다시 타격 판정이 있게
        if (isSplitHit)
        {
            isCollide = false;
        }

        if (changeReturnDefaultMat)
        {
            ChangeMaterial(defaultMaterial);
        }

        bool isShotStartTr = false;
        Vector3 endPos = Vector3.zero;
        if (champBase.shotStartTransform == null)
        {
            endPos = champBase.transform.position;
        }
        else
        {
            endPos = champBase.shotStartTransform.position;
            isShotStartTr = true;
        }

        if (returnLookAtTarget)
        {
            ChangeRotate(RotateType.None);
            transform.LookAt(endPos);
        }
        else
        {
            ChangeRotate(returnRotateType);
        }

        float startTime = Time.time;  // 이동 시작 시간
        float distanceToTarget = Vector3.Distance(transform.position, endPos);
        while (distanceToTarget > 1f)  // 목표 지점에 가까워질 때까지
        {
            endPos = isShotStartTr ? champBase.shotStartTransform.position : champBase.transform.position;
            distanceToTarget = Vector3.Distance(transform.position, endPos);

            // 이동할 비율 계산 (시간에 비례)
            float distanceCovered = (Time.time - startTime) * data.speed;  // 이동한 거리
            float fractionOfJourney = distanceCovered / distanceToTarget;  // 여행 비율

            // 목표 지점으로 일정한 속도로 이동
            transform.position = Vector3.Lerp(transform.position, endPos, fractionOfJourney);

            if (returnLookAtTarget)
            {
                transform.LookAt(endPos);
            }

            yield return null;
        }

        Release();
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
