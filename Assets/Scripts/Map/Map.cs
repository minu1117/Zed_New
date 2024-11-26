using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public int floor;
    public bool lastFloor;
    public bool isClear;
    public Light directionalLight;
    public Transform startingPos;
    public List<TeleportTransform> teleportTransforms;
    public CinemachineVirtualCamera virtualCamera;
    public EnemyGenerator enemyGenerator;

    private void Awake()
    {
        if (virtualCamera != null)
            SetActiveVirtualCam(false);

        if (enemyGenerator != null)
            enemyGenerator.SetMap(this);
    }

    public void SetActiveLight(bool set)
    {
        if (directionalLight == null)
            return;

        directionalLight.gameObject.SetActive(set);
    }

    public TeleportTransform GetRandomTeleportTransform()
    {
        int randomIndex = Random.Range(0, teleportTransforms.Count);
        return teleportTransforms[randomIndex];
    }

    public void SetActiveVirtualCam(bool set) { virtualCamera.gameObject.SetActive(set); }
}
