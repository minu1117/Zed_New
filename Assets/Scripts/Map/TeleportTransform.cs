using UnityEngine;
using UnityEngine.AI;

public class TeleportTransform : MonoBehaviour
{
    [SerializeField] private Transform movedTr;
    private Transform tr;

    private void Awake()
    {
        tr = GetComponent<Transform>();
    }

    public void Teleport(NavMeshAgent agent)
    {
        agent.gameObject.transform.position = tr.position;
        agent.gameObject.transform.forward = tr.forward;
        agent.Warp(tr.position);
    }

    public Transform GetMovedTransform() { return movedTr; }
}
