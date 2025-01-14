using System.Collections.Generic;
using UnityEngine;

public class MapSelectPortal : Portal
{
    [SerializeField] private ModeSelectUI modeSelectUI;
    [SerializeField] private MapSelectUI mapSelectUI;

    protected void Start()
    {
        var loadData = Load();
        InitModeSelectUI(loadData);
        InitMapSelectUI(loadData);
    }

    private StageData Load()
    {
        return stageManager.Load();
    }

    private void InitModeSelectUI(StageData loadData)
    {
        modeSelectUI.AddOnClickBeginningStartButton(Teleport);
        modeSelectUI.AddOnClickBeginningStartButton(() => modeSelectUI.SetActiveUI(false));

        modeSelectUI.AddOnClickMapSelectButton(() => modeSelectUI.SetActiveUI(false));
        modeSelectUI.AddOnClickMapSelectButton(() => mapSelectUI.SetActiveMapSelectUI(true));

        modeSelectUI.AddOnClickContinueButton(() => stageManager.MoveSelectStage(loadData));
        modeSelectUI.AddOnClickContinueButton(() => SetCurrentStage(stageManager.GetCurrentStage(), stageManager.GetCurrentMap()));

        modeSelectUI.SetActiveUI(false);
    }

    private void InitMapSelectUI(StageData loadData)
    {
        mapSelectUI.AddOnClickExitButton(() => modeSelectUI.SetActiveUI(true));

        var allStage = stageManager.GetAllStages();
        List<string> stageNames = new();
        foreach (var stage in allStage)
        {
            stageNames.Add(stage.stageName);
        }

        mapSelectUI.CreateElements(allStage.Count, stageNames);

        for (int i = 0; i < allStage.Count; i++)
        {
            var stage = allStage[i];
            int mapCount = stage.GetAllMaps().Count;
            mapSelectUI.CreateButtons(i, mapCount);
        }

        SetupMapSelectButtons(loadData);
        mapSelectUI.SetActiveMapSelectUI(false);
    }

    private void SetupMapSelectButtons(StageData loadData)
    {
        if (stageManager == null)
            return;

        var allStages = stageManager.GetAllStages();
        if (allStages == null || allStages.Count <= 0)
            return;

        int index = 0;
        int saveMaxStage = loadData.currentStageNumber;
        int saveMaxFloor = loadData.currentMapNumber;
        for (int i = 0; i < allStages.Count; i++)
        {
            var stage = allStages[i];
            var maps = stage.GetAllMaps();
            if (maps == null || maps.Count <= 0)
                continue;

            int cpaturedI = i;
            for (int j = 0; j < maps.Count; j++)
            {
                int capturedJ = j;         // j 캡처
                int capturedIndex = index; // index 캡처
                mapSelectUI.AddButtonOnClick(capturedIndex, () => SetCurrentStage(stage, maps[capturedJ]));

                if (cpaturedI > saveMaxStage)       // 저장된 최고 스테이지보다 높을 경우
                {
                    mapSelectUI.LockButton(index);  // 잠금
                }
                else if (cpaturedI >= saveMaxStage && capturedJ > saveMaxFloor)     // 저장된 최고 스테이지와 같거나 높고, 저장된 최고 층수보다 높을 경우
                {
                    mapSelectUI.LockButton(index);  // 잠금
                }
                else                                // 나머지 경우 (최고 스테이지보다 낮거나 같고, 최고 층수보다 낮거나 같은 경우)
                {
                    mapSelectUI.UnlockButton(index);// 잠금 해제
                }

                index++;
            }
        }
    }

    private void SetCurrentStage(Stage stage, Map map)
    {
        stageManager.SetCurrentStage(stage);
        stageManager.SetCurrentMap(map);
        mapSelectUI.SetActiveMapSelectUI(false);
        modeSelectUI.SetActiveUI(false);
        TeleportSelectMap();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.tag != EnumConverter.GetString(CharacterEnum.Player))
            return;

        var loadData = Load();
        SetupMapSelectButtons(loadData);

        var moveController = Zed.Instance.GetMoveController();
        moveController.StopMove();
        modeSelectUI.SetActiveUI(true);
    }
}