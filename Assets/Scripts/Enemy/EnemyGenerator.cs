using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyGenerator : MonoBehaviour
{
    public List<EnemyBase> enemies;                     // 생성할 몬스터 List
    public int poolSize;
    public float spawnRange;
    public int spawnCount;
    private EnemyGeneratorController enemyGeneratorController;
    private List<GameObject> poolObjects;               // 생성된 몬스터를 종류마다 담아둘 부모 오브젝트의 List
    private List<IObjectPool<EnemyBase>> enemyPools;    // 몬스터 종류 별 오브젝트 풀 List
    private int createIndex = 0;                        // 생성 시 설정될 풀의 Index

    private Dictionary<string, GameObject> enemySkillDict;
    private List<EnemyBase> cratedEnemies;

    private Map map;

    public void Start()
    {
        enemyPools = new();
        poolObjects = new();
        enemySkillDict = new();
        cratedEnemies = new();

        // 생성될 몬스터 List 순회
        for (int i = 0; i < enemies.Count; i++)
        {
            var name = enemies[i].data.charactorName;

            var poolObj = new GameObject($"{name}_Pool");    // 몬스터 오브젝트를 담아둘 부모 오브젝트 생성
            poolObj.gameObject.transform.SetParent(transform);
            poolObj.transform.position = transform.position;                          // 부모 오브젝트를 생성기의 위치로 이동 (몬스터가 이상한 위치에서 생성되지 않기 위함)
            poolObjects.Add(poolObj);

            IObjectPool<EnemyBase> pool;        // 오브젝트 풀 생성
            pool = new ObjectPool<EnemyBase>    // 오브젝트 풀 초기화
            (
                CreateEnemy,
                GetEnemy,
                ReleaseEnemy,
                DestroyEnemy,
                maxSize : poolSize
            );

            enemyPools.Add(pool);   // 몬스터 풀 List에 오브젝트 풀 추가

            if (enemies[i].GetSlot() == null)
                continue;

            var skillParentObj = new GameObject($"{name}_Skill");
            skillParentObj.transform.SetParent(poolObj.transform);
            enemySkillDict.Add(name, skillParentObj);
        }

        if (enemyGeneratorController == null)
        {
            enemyGeneratorController = GetComponentInParent<EnemyGeneratorController>();
        }
    }

    public void SpawnAllEnemy()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            for (int j = 0; j < spawnCount; j++)
            {
                SpawnEnemy(i);
            }
        }
    }

    private void SpawnEnemy(int index)
    {
        createIndex = index;
        enemyPools[createIndex].Get();
        enemyGeneratorController.AddEnemyCount(1);
    }

    // 오브젝트 풀의 Create 
    private EnemyBase CreateEnemy()
    {
        var enemyobj = Instantiate(enemies[createIndex].gameObject, poolObjects[createIndex].transform);    // 몬스터 생성
        enemyobj.transform.position = GetRandomPos(transform.position); // 스폰 범위 내 랜덤 생성

        var enemy = enemyobj.GetComponent<EnemyBase>();                      // 생성한 몬스터에서 EnemyBase 컴포넌트 가져오기
        var hpController = enemy.GetStatusController(SliderMode.HP);         // 몬스터의 HP Controller 가져오기

        enemy.InitStatusController();
        hpController.SetMaxValue();             // 몬스터 최대 HP, MP로 설정

        enemy.SetEnemyGenerator(this);
        enemy.Init();                           // 몬스터 초기 설정 실행

        var slot = enemy.GetSlot();
        if (slot != null)
        {
            slot.SetSlotParent(enemySkillDict[enemies[createIndex].data.charactorName]);
            var slotDict = enemy.GetSlot().GetSlotDict();
            foreach (var excutor in slotDict.Values)
            {
                excutor.SetParentInExcutor();
            }
        }

        enemy.SetPool(enemyPools[createIndex]); // 몬스터에 오브젝트 풀 설정 (Release용)
        cratedEnemies.Add(enemy);
        return enemy;
    }

    protected Vector3 GetRandomPos(Vector3 point)
    {
        var randomPoint = point + Random.insideUnitSphere * spawnRange;
        randomPoint.y = point.y;

        return randomPoint;
    }

    // 오브젝트 풀의 Get 
    private void GetEnemy(EnemyBase enemy)
    {
        enemy.transform.position = GetRandomPos(transform.position);
        enemy.ResetEnemy();
        enemy.SetPatrolState();
        var hpController = enemy.GetStatusController(SliderMode.HP);     // HP Controller 가져오기
        hpController.SetMaxValue();                     // 최대 HP, MP로 설정
        enemy.gameObject.SetActive(true);               // 몬스터 오브젝트 활성화
        enemy.UseSpawnEffect();
    }

    // 오브젝트 풀의 Release
    public void ReleaseEnemy(EnemyBase enemy)
    {
        if (!enemy.GetIsDead())
        {
            StartCoroutine(enemy.OnDead());
        }

        for (int i = 0; i < cratedEnemies.Count; i++)
        {
            var createdEnemy = cratedEnemies[i];
            if (ReferenceEquals(createdEnemy, enemy))
            {
                cratedEnemies[i] = null;
            }
        }
        cratedEnemies.RemoveAll(obj => obj == null);

        enemy.gameObject.SetActive(false);
    }

    // 오브젝트 풀의 Destroy 
    private void DestroyEnemy(EnemyBase enemy)
    {
        Destroy(enemy.gameObject);
    }

    public void ReleaseAll()
    {
        if (cratedEnemies == null || cratedEnemies.Count == 0)
            return;

        for (int i = cratedEnemies.Count-1; i >= 0; i--)
        {
            if (cratedEnemies[i] == null)
                continue;

            ReleaseEnemy(cratedEnemies[i]);
        }

        cratedEnemies.Clear();
    }

    public void SetGeneratorController(EnemyGeneratorController controller) { enemyGeneratorController = controller; }
    public void SetMap(Map map) { this.map = map; }
    public Map GetMap() { return map; }
    public void SubEnemyCount() { enemyGeneratorController.SubEnemyCount(); }
    public List<IObjectPool<EnemyBase>> GetEnemyPools() { return enemyPools; }
}
