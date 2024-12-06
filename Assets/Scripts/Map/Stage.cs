using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public int stageNumber;
    [SerializeField] private List<Map> maps;
    [SerializeField] private Dictionary<int, Map> mapDict;
    public bool lastStage;
    public bool stageClear { get; set; } = false;
    public Map currentMap { get; set; }
    public Skybox skybox;

    private void Awake()
    {
        mapDict = new();
        foreach (var map in maps)
        {
            mapDict.Add(map.floor, map);
        }
    }

    public void NextMap()
    {
        if (stageClear)
            return;

        SetActiveAllMaps(false);

        if (currentMap == null)
        {
            currentMap = mapDict[GetLowestMap()];
        }
        else
        {
            currentMap.SetActiveLight(false);

            int stageNumber = currentMap.floor + 1;
            currentMap = mapDict[stageNumber];
        }

        SetActiveMap(currentMap, true);
        currentMap.SetActiveLight(true);
        currentMap.SetEnemyGeneratorIsCreated(false);
        currentMap.SetEnemyGeneratorColliderEnable(true);
        currentMap.ResetPortal();
        currentMap.StartCheakMapClear();

        var agent = Zed.Instance.GetMoveController().GetAgent();
        agent.Warp(currentMap.startingPos.position);
        Zed.Instance.transform.position = currentMap.startingPos.position;
    }

    public void StageClear()
    {
        stageClear = true;
        currentMap = null;
        SetActiveAllMaps(false);
    }

    private int GetLowestMap()
    {
        int lowest = mapDict.Keys.Min();
        return lowest;
    }

    private void SetActiveAllMaps(bool active)
    {
        foreach (var stage in mapDict)
        {
            SetActiveMap(stage.Value, active);
        }
    }

    private void SetActiveMap(Map map, bool active)
    {
        map.gameObject.SetActive(active);
    }
}
