using System.Collections.Generic;
using UnityEngine;

public class EnemyGeneratorController : MonoBehaviour
{
    [SerializeField] protected List<EnemyGenerator> generators;
    protected Map map;
    protected int enemyCount = 0;
    protected bool isCreated = false;
    protected BoxCollider coll;

    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
        SetColliderEnable(true);
        SetIsCreated(false);

        foreach (var generator in generators)
        {
            generator.SetGeneratorController(this);
        }
    }

    public void SetMap(Map map)
    {
        this.map = map;
        foreach (var generator in generators)
        {
            generator.SetMap(map);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (GetIsCreated())
            return;

        if (other.tag == EnumConverter.GetString(CharacterEnum.Player))
        {
            SpawnAllEnemy();
            SetColliderEnable(false);
        }
    }

    protected void SpawnAllEnemy()
    {
        foreach (var generator in generators)
        {
            generator.SpawnAllEnemy();
        }

        SetIsCreated(true);
    }

    public bool GetIsCreated() { return isCreated; }
    public void SetIsCreated(bool set) { isCreated = set; }
    public void AddEnemyCount(int add) { enemyCount += add; }
    public void SubEnemyCount() { enemyCount--; }
    public int GetEnemyCount() { return enemyCount; }
    public void ResetEnemyCount() { enemyCount = 0; }
    public void SetColliderEnable(bool set) { coll.enabled = set; }
}
