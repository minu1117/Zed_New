using System.Collections;
using UnityEngine;

public class Trap : Skill
{
    [SerializeField] private float burstDuration;
    [SerializeField] private Effect burstParticle;
    [SerializeField] private Color burstColor;
    [SerializeField] private Renderer meshRenderer;
    [SerializeField] private AudioClip explosionSound;

    private Color defaultColor;
    private float timer = 0f;
    private bool isColorAdded = false;

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
        StartSound(data.useClips);      // 스킬 시전 사운드 재생
        StartSound(data.voiceClips);    // 시전 보이스 재생
        StartCoroutine(CoUse());
    }

    private IEnumerator CoUse()
    {
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

        // Burst
        isCollide = false;
        foreach (var coll in colliders)
        {
            coll.GetCollider().enabled = true;
        }

        isColorAdded = false;
        SoundManager.Instance.PlayOneShot(explosionSound);
        if (burstParticle != null)
        {
            var effect = EffectManager.Instance.GetEffect(burstParticle.name);
            effect.SetStartPos(transform.position);
            effect.SetForward(transform.forward);
            effect.Use();
        }

        if (burstDuration > 0f)
            yield return new WaitForSeconds(burstDuration);

        timer = 0f;

        if (meshRenderer != null)
            meshRenderer.material.color = defaultColor;

        Release();
    }

    private void Update()
    {
        if (meshRenderer == null || !isColorAdded)
            return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / data.duration);
        meshRenderer.material.color = Color.Lerp(defaultColor, burstColor, t);
    }
}
