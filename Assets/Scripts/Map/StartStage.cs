using UnityEngine;

public class StartStage : MonoBehaviour
{
    [SerializeField] private Map map;
    [SerializeField] private Light directionalLight;
    [SerializeField] private Transform startPos;

    public void Awake()
    {
        Zed.Instance.GetMoveController().GetAgent().Warp(startPos.transform.position);
        Zed.Instance.gameObject.transform.position = startPos.transform.position;
    }

    public void SetActiveLight(bool set)
    {
        map.SetActiveLight(set);
    }
}
