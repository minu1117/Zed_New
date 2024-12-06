using Cinemachine;
using System.Collections;
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
    public EnemyGeneratorController enemyGeneratorController;
    public Portal portal;
    public CurtainMoveController curtainMoveController;

    private void Awake()
    {
        if (virtualCamera != null)
            SetActiveVirtualCam(false);

        if (enemyGeneratorController != null)
            enemyGeneratorController.SetMap(this);
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

    private IEnumerator CoCheakClear()
    {
        yield return new WaitUntil(() => GetIsClear());

        enemyGeneratorController.ResetEnemyCount();
        isClear = true;
        portal.Open();
    }

    private bool GetIsClear()
    {
        if (!enemyGeneratorController.GetIsCreated())
            return false;

        if (enemyGeneratorController.GetEnemyCount() > 0)
            return false;

        return true;
    }

    public void SetEnemyGeneratorIsCreated(bool set)
    {
        if (enemyGeneratorController == null)
            return;

        enemyGeneratorController.SetIsCreated(set);
    }

    public void SetEnemyGeneratorColliderEnable(bool set)
    {
        if (enemyGeneratorController == null)
            return;

        enemyGeneratorController.SetColliderEnable(set);
    }

    public void StartCheakMapClear()
    {
        if (enemyGeneratorController == null)
        {
            isClear = true;
            portal.Open();
            return;
        }

        StartCoroutine(CoCheakClear());
    }

    public void ResetPortal()
    {
        portal.ResetDoor();
    }

    public void SetActiveVirtualCam(bool set) { virtualCamera.gameObject.SetActive(set); }
    public CurtainMoveController GetCurtainMoveController() { return curtainMoveController; }
}
