using System.Collections;
using UnityEngine;

public class WideArea : Skill, ITargetable
{
    [SerializeField] private float interval;
    private WaitForSeconds waitHitInterval;
    private ChampBase target;

    public override void Awake()
    {
        base.Awake();
        waitHitInterval = new WaitForSeconds(interval);
    }

    public override void Use(GameObject character)
    {
        transform.position = target.transform.position;

        if (target.TryGetComponent<BoxCollider>(out var boxCollider))
        {
            transform.position = new Vector3(transform.position.x, boxCollider.bounds.min.y, transform.position.z);
        }

        base.Use(character);
        StartCoroutine(CoUse());  // 근접 스킬 사용 코루틴 시작
    }

    private IEnumerator CoUse()
    {
        if (target == null)
            yield break;

        float startTime = Time.time;
        while (Time.time - startTime < data.duration)
        {
            isCollide = false;
            foreach (var coll in colliders)
            {
                coll.GetCollider().enabled = true;
            }

            yield return waitHitInterval;

            isCollide = true;
            foreach (var coll in colliders)
            {
                coll.GetCollider().enabled = false;
            }

            yield return null;
            yield return null;
        }

        Release();
    }

    public void SetTarget(GameObject target)
    {
        this.target = target.GetComponent<ChampBase>();
    }
}
