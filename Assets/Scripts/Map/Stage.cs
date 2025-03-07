using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public string stageName;
    public int stageNumber;
    [SerializeField] private List<Map> maps;
    [SerializeField] private Dictionary<int, Map> mapDict;
    [SerializeField] private AudioClip normalBGM;
    [SerializeField] private AudioClip bossBGM;
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
            SoundManager.Instance.Play(normalBGM);
            SoundManager.Instance.SetLoop(normalBGM, true);
        }
        else
        {
            currentMap.SetActiveLight(false);

            int stageNumber = currentMap.floor + 1;
            currentMap = mapDict[stageNumber];

            if (currentMap != null && currentMap.lastFloor)
            {
                SoundManager.Instance.Stop(normalBGM);
                SoundManager.Instance.Play(bossBGM);
                SoundManager.Instance.SetLoop(bossBGM, true);
            }
        }

        ResetCurrentMap();
        TeleportPlayer();
    }

    public void MoveCurrentMap()
    {
        if (currentMap == null)
            return;

        if (!currentMap.lastFloor)
        {
            SoundManager.Instance.Play(normalBGM);
            SoundManager.Instance.SetLoop(normalBGM, true);
        }
        else
        {
            SoundManager.Instance.Play(bossBGM);
            SoundManager.Instance.SetLoop(bossBGM, true);
        }

        SetActiveAllMaps(false);
        ResetCurrentMap();
        TeleportPlayer();
    }

    private void TeleportPlayer()
    {
        var agent = Zed.Instance.GetMoveController().GetAgent();
        agent.Warp(currentMap.startingPos.position);
        Zed.Instance.transform.position = currentMap.startingPos.position;
    }

    private void ResetCurrentMap()
    {
        SetActiveMap(currentMap, true);
        currentMap.SetActiveLight(true);
        currentMap.SetEnemyGeneratorIsCreated(false);
        currentMap.SetEnemyGeneratorColliderEnable(true);
        currentMap.ResetPortal();
        currentMap.StartCheakMapClear();
    }

    public void StageClear()
    {
        SoundManager.Instance.Stop(bossBGM);
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

    public void SetCurrentMap(Map map) { currentMap = map; }
    public Dictionary<int, Map> GetMapDict() { return mapDict; }
    public List<Map> GetAllMaps() { return maps; }
}
