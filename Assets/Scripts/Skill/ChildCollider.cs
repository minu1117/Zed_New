using UnityEngine;

public class ChildCollider : MonoBehaviour
{
    [SerializeField] private Skill parent;
    private BoxCollider coll;

    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        parent.Collide(other.gameObject);
    }

    public BoxCollider GetCollider() { return coll; }
}
