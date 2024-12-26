using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingCredit : MonoBehaviour
{
    [SerializeField] private ContentUnlock contentUnlockUI;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI endingText;
    [SerializeField] private float fadeTime;
    private WaitForSeconds waitForDeltaTime;
    private float waitEndingTime = 10f;

    private Coroutine coroutine;

    private void Awake()
    {
        Color color1 = endingText.color;
        color1.a = 0f;
        endingText.color = color1;

        waitForDeltaTime = new WaitForSeconds(Time.deltaTime);
        background.gameObject.SetActive(false);
        endingText.gameObject.SetActive(false);
    }

    public void Ending()
    {
        background.gameObject.SetActive(true);
        endingText.gameObject.SetActive(true);
        CustomSceneManager.Instance.FadeIn();
        coroutine = StartCoroutine(CoFadeInToOut(endingText));
    }

    private IEnumerator CoFadeInToOut(TextMeshProUGUI text)
    {
        Color color = text.color;
        color.a = 0;
        text.color = color;

        float startTime = 0f;
        while (startTime < fadeTime)
        {
            startTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, startTime / fadeTime);
            text.color = color;
            yield return waitForDeltaTime;
        }

        color.a = 1f;
        text.color = color;

        yield return new WaitForSeconds(waitEndingTime);

        coroutine = null;
        coroutine = StartCoroutine(CoFadeOut(text));
    }

    private IEnumerator CoFadeOut(TextMeshProUGUI text)
    {
        Color color = text.color;
        color.a = 1;
        text.color = color;

        float startTime = 0f;
        while (startTime < fadeTime)
        {
            startTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, startTime / fadeTime);
            text.color = color;
            yield return waitForDeltaTime;
        }

        color.a = 0f;
        text.color = color;

        background.gameObject.SetActive(false);
        endingText.gameObject.SetActive(false);
        CustomSceneManager.Instance.FadeOut();
        contentUnlockUI.FadeOut(1f);
    }

    private void Skip()
    {
        background.gameObject.SetActive(false);
        endingText.gameObject.SetActive(false);

        Color color = endingText.color;
        color.a = 0f;
        endingText.color = color;
        CustomSceneManager.Instance.FadeOut();

        contentUnlockUI.FadeOut(1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && coroutine != null)
        {
            Skip();
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}
