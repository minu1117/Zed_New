using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomList<T>
{
    public List<T> items = new List<T>();
}

public class ListManager<T> : MonoBehaviour
{
    public List<CustomList<T>> listOfLists = new List<CustomList<T>>();
}