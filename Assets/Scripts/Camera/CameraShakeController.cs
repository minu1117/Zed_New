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
    private WaitForSeconds waitForSecnonds;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = 0;
        perlin.m_FrequencyGain = 0;

        waitForSecnonds = new WaitForSeconds(shakeDuration);
    }

    public void ShakeCamera()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(CoShake());
    }

    private IEnumerator CoShake()
    {
        perlin.m_AmplitudeGain = amplitudeGain;
        perlin.m_FrequencyGain = frequencyGain;

        yield return waitForSecnonds;

        perlin.m_AmplitudeGain = 0;
        perlin.m_FrequencyGain = 0;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            ShakeCamera();
    }
}
