using UnityEngine;

public class TrapSensorCollider : MonoBehaviour
{
    [SerializeField] private AnimationTrap trap;
    private BoxCollider coll;

    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != EnumConverter.GetString(CharacterEnum.Player))
            return;

        trap.Sensing();
    }

    public void SetColliderEnable(bool set) { coll.enabled = set; }
}
