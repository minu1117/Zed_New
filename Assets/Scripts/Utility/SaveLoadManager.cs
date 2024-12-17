using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public struct SerializableKeyValuePair<T>
{
    public T Key;
    public T Value;
}

public class SerializableKeyValuePairList<T>
{
    public List<SerializableKeyValuePair<T>> pairList;
}

public enum SaveLoadMode
{
    Skill,
    PlayerData,
}

public static class SaveLoadManager
{
    private static string skillSavePath = Application.persistentDataPath + "/CurrentSkill.json";
    private static string playerDataSavePath = Application.persistentDataPath + "/Player.json";

    public static void Save<T>(Dictionary<T, T> saveDict, SaveLoadMode mode)
    {
        var path = GetModePath(mode);
        var serializablePair = GetSerializablePairList(saveDict);
        string json = JsonUtility.ToJson(serializablePair, true);
        File.WriteAllText(path, json);
    }

    public static Dictionary<T, T> Load<T>(SaveLoadMode mode)
    {
        string path = GetModePath(mode);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var jsonParse = JsonUtility.FromJson<SerializableKeyValuePairList<T>>(json);

            Dictionary<T, T> returnDict = new();
            foreach (var item in jsonParse.pairList)
            {
                returnDict.Add(item.Key, item.Value);
            }

            return returnDict;
        }
        else
        {
            return null;
        }
    }

    private static SerializableKeyValuePairList<T> GetSerializablePairList<T>(Dictionary<T,T> dict)
    {
        SerializableKeyValuePairList<T> list = new();
        list.pairList = new();
        foreach (var data in dict)
        {
            SerializableKeyValuePair<T> pair;
            pair.Key = data.Key;
            pair.Value = data.Value;
            list.pairList.Add(pair);
        }

        return list;
    }

    private static string GetModePath(SaveLoadMode mode)
    {
        string path = string.Empty;
        switch (mode)
        {
            case SaveLoadMode.Skill:
                path = skillSavePath;
                break;
            case SaveLoadMode.PlayerData:
                path = playerDataSavePath;
                break;
        }

        return path;
    }
}
