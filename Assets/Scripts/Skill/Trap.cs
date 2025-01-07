using System.Collections;
using UnityEngine;

public class Trap : Skill
{
    [SerializeField] protected float burstDuration;
    [SerializeField] protected Effect burstParticle;
    [SerializeField] protected Color burstColor;
    [SerializeField] protected Renderer meshRenderer;
    [SerializeField] protected AudioClip explosionSound;
    [SerializeField] protected AudioClip countdownSound;

    protected Color defaultColor;
    protected float timer = 0f;
    protected bool isColorAdded = false;

    public override void Awake()
    {
        base.Awake();
        if (meshRenderer != null)
        {
            defaultColor = meshRenderer.material.color;
        }
    }

    public override void Use(GameObject character)
    {
        if (!SubMP())
        {
            Release();
            return;
        }

        StartSound(data.useClips);      // 스킬 시전 사운드 재생
        StartSound(data.voiceClips);    // 시전 보이스 재생
        StartCoroutine(CoUse());
    }

    private IEnumerator CoUse()
    {
        SoundManager.Instance.PlayOneShot(countdownSound);

        foreach (var coll in colliders)
        {
            coll.GetCollider().enabled = false;
        }

        Vector3 movePoint = transform.position + (usePoint * data.distance);
        transform.position = movePoint;

        if (caster.TryGetComponent<BoxCollider>(out var boxCollider))
        {
            var yPos = boxCollider.bounds.min.y;
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        }

        UseEffect(gameObject);

        isCollide = true;
        isColorAdded = true;

        yield return waitduration;

        Burst();

        if (burstDuration > 0f)
            yield return new WaitForSeconds(burstDuration);

        timer = 0f;

        if (meshRenderer != null)
            meshRenderer.material.color = defaultColor;

        Release();
    }

    protected virtual void Update()
    {
        if (meshRenderer == null || !isColorAdded)
            return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / data.duration);
        meshRenderer.material.color = Color.Lerp(defaultColor, burstColor, t);
    }

    protected void Burst()
    {
        isCollide = false;
        foreach (var coll in colliders)
        {
            coll.GetCollider().enabled = true;
        }

        if (data.isCameraShake)
        {
            GameSceneManager.Instance.GetCameraChakeController().ShakeCamera();
        }

        isColorAdded = false;
        SoundManager.Instance.PlayOneShot(explosionSound);
        UseTrapEffect(burstParticle, transform);
    }

    protected Effect UseTrapEffect(Effect effectPrefab, Transform transform)
    {
        if (effectPrefab == null)
            return null;

        var effect = EffectManager.Instance.GetEffect(effectPrefab.name);
        effect.SetStartPos(transform.position);
        effect.SetForward(transform.forward);
        effect.Use();

        return effect;
    }
}
