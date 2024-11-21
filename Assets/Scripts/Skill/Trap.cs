using System.Collections;
using UnityEngine;

public class Trap : Skill
{
    [SerializeField] private float burstDuration;
    [SerializeField] private Effect burstParticle;
    [SerializeField] private Color burstColor;

    private Renderer meshRenderer;
    private Color defaultColor;
    private float timer = 0f;
    private bool isColorAdded = false;

    public override void Awake()
    {
        base.Awake();
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            defaultColor = meshRenderer.material.color;
        }
    }

    public override void Use(GameObject character)
    {
        base.Use(character);
        StartCoroutine(CoUse());
    }

    private IEnumerator CoUse()
    {
        Vector3 movePoint = transform.position + (usePoint * data.distance);
        transform.position = movePoint;

        isCollide = true;
        isColorAdded = true;

        yield return waitduration;

        isColorAdded = false;

        // Burst
        isCollide = false;

        if (burstParticle != null)
        {
            var effect = EffectManager.Instance.GetEffect(burstParticle.name);
            effect.SetStartPos(transform.position);
            effect.SetForward(transform.forward);
            effect.Use();
        }

        yield return new WaitForSeconds(burstDuration);

        timer = 0f;
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
