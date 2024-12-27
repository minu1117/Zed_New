using UnityEngine;

public class InteractiveEnemyGeneratorController : EnemyGeneratorController
{
    [SerializeField] private KeyCode interactionKey;
    [SerializeField] private float interactionRange;
    private Zed player;

    private void Awake()
    {
        player = Zed.Instance;
    }

    private void Update()
    {
        if (!CheackDistance())
            return;

        Interaction();
    }

    protected void Interaction()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            SpawnAllEnemy();
        }
    }
    protected override void OnTriggerEnter(Collider other) { }

    protected virtual bool CheackDistance()
    {
        if (player == null)
            return false;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= interactionRange)
        {
            return true;
        }

        return false;
    }
}
