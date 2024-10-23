using UnityEngine;

public class ChildCollider : MonoBehaviour
{
    [SerializeField] private Skill parent;

    private void OnTriggerEnter(Collider other)
    {
        parent.Collide(other.gameObject);
    }
}
