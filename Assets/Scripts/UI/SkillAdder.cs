using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillAdder : MonoBehaviour
{
    [SerializeField] private DraggableSkillCreatorManager skillCreatorManager;
    [SerializeField] private DraggableSkill skill;
    [SerializeField] private KeyCode interactionKey;
    [SerializeField] private float interactionRange;

    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private Image image;
    [SerializeField] private AudioClip sound;
    [SerializeField] private Effect effect;

    [SerializeField] private TextMeshProUGUI keyTmp;

    private Zed player;
    private bool isAdded = false;

    private void Awake()
    {
        player = Zed.Instance;
        keyTmp.text = EnumConverter.GetString(interactionKey);

        if (skill != null)
        {
            tmp.text = skill.GetSkillName();
            image.sprite = skill.GetSkillSprite();
        }
    }

    public void Update()
    {
        if (!CheackDistance())
            return;

        Interaction();
    }

    private bool CheackDistance()
    {
        if (player == null)
            return false;

        if (isAdded)
            return false;

        float distance = Vector3.Distance(transform.position, player.gameObject.transform.position);
        if (distance <= interactionRange)
        {
            return true;
        }

        return false;
    }

    private void Interaction()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            SoundManager.Instance.PlayOneShot(sound);
            EffectManager.Instance.UseEffect(effect, transform, true, true);
            skillCreatorManager.AddDraggableSkill(skill);
            isAdded = true;
        }
    }

    public bool GetIsAdded() { return isAdded; }
}
