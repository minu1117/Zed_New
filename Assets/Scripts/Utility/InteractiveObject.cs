using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    [SerializeField] protected KeyCode interactionKey;
    [SerializeField] protected float interactionRange;
    [SerializeField] protected AudioClip interactionSound;
    [SerializeField] protected AudioClip escapeSound;
    protected Zed player;
    protected bool isInteractable;

    protected virtual void Start()
    {
        player = Zed.Instance;
    }

    protected virtual bool CheckDistance()
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
    protected void StartSound(AudioClip clip)
    {
        if (clip == null)
            return;

        SoundManager.Instance.PlayOneShot(clip);
    }
    public void SetIsInteract(bool set) { isInteractable = set; }
}
