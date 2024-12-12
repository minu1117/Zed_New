using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    [SerializeField] protected KeyCode interactionKey;
    [SerializeField] protected float interactionRange;
    protected Zed player;

    protected virtual void Awake()
    {
        player = Zed.Instance;
    }

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

    protected abstract void Interaction();
}
