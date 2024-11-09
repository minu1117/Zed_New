using UnityEngine;

public class Map : MonoBehaviour
{
    public int floor;
    public bool lastFloor;
    public bool isClear;
    public Light directionalLight;
    public Transform startingPos;

    public void SetActiveLight(bool set)
    {
        if (directionalLight == null)
            return;

        directionalLight.gameObject.SetActive(set);
    }
}
