using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [SerializeField] private StartStage startStage;
    [SerializeField] private SkyboxChanger skyboxChanger;

    [SerializeField] private List<Stage> stages;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration;
    private Dictionary<int, Stage> stageDict;
    private Stage currentStage;

    public bool isFade { get; set; } = false;

    private void Awake()
    {
        stageDict = new();
        foreach (var stage in stages)
        {
            stageDict.Add(stage.stageNumber, stage);
        }

        SetActiveAllStages(false);
    }

    public void NextStage()
    {
        SetActiveAllStages(false);

        bool stageClear = CheckStageClear(currentStage);
        if (stageClear)
        {
            SetStageClear(currentStage);
        }

        if (currentStage != null && currentStage.lastStage && currentStage.stageClear)
        {
            // ending
            return;
        }

        if (currentStage == null)
        {
            startStage.SetActiveLight(false);
            currentStage = stageDict[GetLowestStage()];
        }

        if (currentStage.stageClear)
        { 
            int stageNumber = ++currentStage.stageNumber;
            currentStage = stageDict[stageNumber];
        }


        SetActiveStage(currentStage, true);
        currentStage.NextMap();
        skyboxChanger.ChangeToDaySkybox(currentStage.skybox);
        ChangeSunSource(currentStage.currentMap.directionalLight);
    }

    private int GetLowestStage()
    {
        int lowest = stageDict.Keys.Min();
        return lowest;
    }

    private void SetActiveAllStages(bool active)
    {
        foreach (var stage in stageDict)
        {
            SetActiveStage(stage.Value, active);
        }
    }

    private void SetActiveStage(Stage stage, bool active)
    {
        stage.gameObject.SetActive(active);
    }

    private bool CheckStageClear(Stage stage)
    {
        if (stage == null || stage.currentMap == null || !stage.currentMap.lastFloor)
            return false;

        return true;
    }

    private void SetStageClear(Stage stage)
    {
        stage.StageClear();
    }

    public void ChangeSunSource(Light light)
    {
        skyboxChanger.ChangeSunSource(light);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            NextStage();
        }
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

    public IEnumerator FadeCanvasGroup(float start, float end)
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
}
