using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomSceneManager : Singleton<CustomSceneManager>
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration;
    [SerializeField] private string gameSceneName;
    [SerializeField] private string titleSceneName;
    public bool isFade { get; set; } = false;

    public void LoadGameScene()
    {
        LoadScene(gameSceneName);
    }

    public void LoadTitleScene()
    {
        LoadScene(titleSceneName);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(CoLoadSceneAsync(sceneName));
    }
    public IEnumerator CoLoadSceneAsync(string sceneName)
    {
        FadeIn();
        yield return new WaitUntil(() => !isFade);

        progressSlider.value = 0f;
        progressText.text = "0%";

        progressSlider.gameObject.SetActive(true);
        progressText.gameObject.SetActive(true);
        var async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
        {
            progressSlider.value = async.progress;
            progressText.text = $"{async.progress * 100}%";
            yield return null;
        }

        progressSlider.gameObject.SetActive(false);
        progressText.gameObject.SetActive(false);
        FadeOut();
    }

    // 투명 -> 불투명
    public void FadeIn()
    {
        isFade = true;
        StartCoroutine(FadeCanvasGroup(0, 1));
    }

    // 불투명 -> 투명
    public void FadeOut()
    {
        StartCoroutine(FadeCanvasGroup(1, 0));
    }

    private IEnumerator FadeCanvasGroup(float start, float end)
    {
        float currentTime = Time.time;
        Color color = fadeImage.color;

        while (Time.time - currentTime < fadeDuration)
        {
            color.a = Mathf.Lerp(start, end, (Time.time - currentTime) / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = end;
        fadeImage.color = color;

        if (end >= 1f)
        {
            isFade = false;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            LoadScene(gameSceneName);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            LoadScene(titleSceneName);
        }
    }
}