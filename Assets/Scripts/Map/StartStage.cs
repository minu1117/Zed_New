using UnityEngine;

public class StartStage : MonoBehaviour
{
    [SerializeField] private Map map;
    [SerializeField] private Light directionalLight;

    public void SetActiveLight(bool set)
    {
        map.SetActiveLight(set);
    }
}
