using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private List<Stage> stages;
    private Dictionary<int, Stage> stageDict;
    private Stage currentStage;

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
            currentStage = stageDict[GetLowestStage()];
        }

        if (currentStage.stageClear)
        { 
            int stageNumber = ++currentStage.stageNumber;
            currentStage = stageDict[stageNumber];
        }

        SetActiveStage(currentStage, true);
        currentStage.NextMap();
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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            NextStage();
        }
    }
}
