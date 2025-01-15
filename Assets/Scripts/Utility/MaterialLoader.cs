using System.Collections.Generic;
using UnityEngine;

public class MaterialLoader : Singleton<MaterialLoader>
{
    [SerializeField] private List<Material> loadMatList;
    private Dictionary<string, Material> loadedMatDict;

    private void Start()
    {
        loadedMatDict = new();
        LoadMaterials();
    }

    private async void LoadMaterials()
    {
        //loadedMatDict = await AddressableManager.LoadAllMaterials(loadMatList);
        loadedMatDict = await AddressableManager.LoadAll(loadMatList);
    }

    public Material GetMatarial(string name)
    {
        var matName = name.Replace(" (Instance)", "");
        if (!loadedMatDict.ContainsKey(matName))
            return null;

        return loadedMatDict[matName];
    }
}
