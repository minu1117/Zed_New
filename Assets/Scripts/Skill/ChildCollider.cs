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
        var caster = parent.GetCaster();
        if (caster != null && other.tag == caster.tag)
            return;

        parent.Collide(other.gameObject);
    }

    public BoxCollider GetCollider() { return coll; }
}
