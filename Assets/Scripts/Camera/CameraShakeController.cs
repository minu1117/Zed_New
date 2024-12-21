using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float shakeDuration;
    public float amplitudeGain;
    public float frequencyGain;

    private CinemachineBasicMultiChannelPerlin perlin;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = 0;
        perlin.m_FrequencyGain = 0;
    }

    public void PowerfulShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(CoShake(shakeDuration*2, amplitudeGain*2, frequencyGain*2));
    }

    public void ShakeCamera()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(CoShake(shakeDuration, amplitudeGain, frequencyGain));
    }

    private IEnumerator CoShake(float duration, float amplitude, float frequency)
    {
        perlin.m_AmplitudeGain = amplitude;
        perlin.m_FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        perlin.m_AmplitudeGain = 0;
        perlin.m_FrequencyGain = 0;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            ShakeCamera();
    }
}
