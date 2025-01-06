using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct StageData
{
    public int currentStageNumber;
    public int currentMapNumber;
}

public class StageManager : MonoBehaviour
{
    [SerializeField] private StartStage startStage;
    [SerializeField] private SkyboxChanger skyboxChanger;

    [SerializeField] private List<Stage> stages;
    [SerializeField] private EndingCredit endingCredit;
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
    }

    private void Start()
    {
        // 맵의 Awake가 실행되지 않을 수 있어 Start에서 맵 비활성화
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

        // ending
        if (currentStage != null && currentStage.lastStage && currentStage.stageClear)
        {
            startStage.gameObject.SetActive(true);
            startStage.SetActiveLight(true);
            ChangeSunSource(startStage.GetDirectionalLight());
            skyboxChanger.ChangeToDaySkybox(Skybox.Defalut);
            startStage.Warp();

            if (GameSceneManager.Instance.data.ending)
                return;

            endingCredit.Ending();
            GameSceneManager.Instance.SetEnding(true);
            return;
        }

        if (currentStage == null)
        {
            startStage.SetActiveLight(false);
            startStage.gameObject.SetActive(false);
            startStage.StopBgm();
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

    public void MoveCurrentStage()
    {
        if (currentStage == null)
            return;

        SetActiveAllStages(false);
        startStage.SetActiveLight(false);
        startStage.gameObject.SetActive(false);
        startStage.StopBgm();

        SetActiveStage(currentStage, true);
        currentStage.MoveCurrentMap();
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
    public void SetActiveStartStage(bool set) { startStage.gameObject.SetActive(set); }
    public void StopStartStageBGM() { startStage.StopBgm(); }
    public void SetActiveStartStageLight(bool set) { startStage.SetActiveLight(set); }
    public void SetCurrentStage(Stage stage) { currentStage = stage; }
    public StartStage GetStartStage() { return startStage; }
    public void SetCurrentMap(Map map)
    {
        if (currentStage == null)
            return;

        currentStage.SetCurrentMap(map);
    }
    public Dictionary<int, Stage> GetStageDict() { return stageDict; }
    public List<Stage> GetAllStages() { return stages; }
    public Stage GetCurrentStage() { return currentStage; }
    public Map GetCurrentMap() { return currentStage.currentMap; }

    /********************************* Save & Load *********************************/

    public void Save()
    {
        if (currentStage == null)
            return;

        StageData data = new StageData();
        data.currentStageNumber = currentStage.stageNumber;
        data.currentMapNumber = currentStage.currentMap.floor;

        SaveLoadManager.Save(data, SaveLoadMode.Stage);
    }

    public StageData Load()
    {
        if (stageDict == null)
            return default;

        var loadData = SaveLoadManager.Load<StageData>(SaveLoadMode.Stage);
        return loadData;
    }

    public void MoveSelectStage(StageData data)
    {
        if (!stageDict.ContainsKey(data.currentStageNumber))
            return;

        currentStage = stageDict[data.currentStageNumber];
        var mapDict = currentStage.GetMapDict();

        if (mapDict == null)
            return;

        if (!mapDict.ContainsKey(data.currentMapNumber))
            return;

        currentStage.currentMap = mapDict[data.currentMapNumber];

    }

    private void SaveSequnce()
    {
        if (currentStage == null)
            return;

        if (currentStage.currentMap == null)
            return;

        var loadData = Load();

        // 이전보다 높거나 같은 스테이지일 경우
        if (loadData.currentStageNumber <= currentStage.stageNumber)
        {
            // 이전보다 높거나 같은 단계일 경우에만 저장
            // 같은 경우를 넣은 이유 : 넣지 않을 시 첫 저장이 저장 되지 않음 (Json 파일이 없을 때 파일 생성 X)
            if (loadData.currentMapNumber <= currentStage.currentMap.floor)
            {
                Save();
            }
        }
    }

    public void OnApplicationQuit()
    {
        SaveSequnce();
    }

    public void OnDestroy()
    {
        SaveSequnce();
    }
}
