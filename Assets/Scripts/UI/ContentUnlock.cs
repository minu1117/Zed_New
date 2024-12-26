using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentUnlock : MonoBehaviour
{
    [SerializeField] private Image bossRaidUnlockPanel;
    [SerializeField] private Image chellengeUnlockPanel;

    [SerializeField] private TextMeshProUGUI bossRaidUnlockText;
    [SerializeField] private TextMeshProUGUI chellengeUnlockText;

    [SerializeField] private float waitTime;
    [SerializeField] private float fadeTime;
    private WaitForSeconds waitForDeltaTime;

    private void Awake()
    {
        Color color1 = bossRaidUnlockPanel.color;
        color1.a = 0f;
        bossRaidUnlockPanel.color = color1;

        Color color2 = chellengeUnlockPanel.color;
        color2.a = 0f;
        bossRaidUnlockPanel.color = color2;

        waitForDeltaTime = new WaitForSeconds(Time.deltaTime);
    }

    public void FadeOut(float startAlpha)
    {
        StartCoroutine(CoFadeOut(bossRaidUnlockPanel, bossRaidUnlockText, startAlpha));
        StartCoroutine(CoFadeOut(chellengeUnlockPanel, chellengeUnlockText, startAlpha));
    }

    private IEnumerator CoFadeOut(Image img, TextMeshProUGUI text, float startAlpha)
    {
        Color imgColor = img.color;
        Color textColor = text.color;

        imgColor.a = startAlpha;
        textColor.a = startAlpha;

        img.color = imgColor;
        text.color = textColor;

        yield return new WaitForSeconds(waitTime);

        float startTime = 0f;
        while (startTime < fadeTime)
        {
            startTime += Time.deltaTime;

            imgColor.a = Mathf.Lerp(startAlpha, 0f, startTime / fadeTime);
            textColor.a = Mathf.Lerp(startAlpha, 0f, startTime / fadeTime);

            img.color = imgColor;
            text.color = textColor;
            yield return waitForDeltaTime;
        }

        imgColor.a = 0f;
        textColor.a = 0f;

        img.color = imgColor;
        text.color = textColor;
    }
}
