using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillAdder : InteractiveObject
{
    [SerializeField] private DraggableSkillCreatorManager skillCreatorManager;
    [SerializeField] private DraggableSkill skill;
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private Image image;
    [SerializeField] private Effect effect;

    [SerializeField] private TextMeshProUGUI keyTmp;
    [SerializeField] private DialogueStarter dialogueStarter;
    private bool isAdded = false;

    protected override void Awake()
    {
        base.Awake();
        keyTmp.text = EnumConverter.GetString(interactionKey);

        if (skill != null)
        {
            tmp.text = skill.GetSkillName();
            image.sprite = skill.GetSkillSprite();
        }
    }

    public void Update()
    {
        Interaction();
    }

    protected override bool CheackDistance()
    {
        var check = base.CheackDistance();
        if (isAdded)
        {
            check = false;
        }

        return check;
    }

    protected override void Interaction()
    {
        if (!isInteractable)
            return;

        if (!CheackDistance())
            return;

        if (Input.GetKeyDown(interactionKey))
        {
            if (dialogueStarter != null)
                dialogueStarter.StartDialogue(player.gameObject);

            StartSound(interactionSound);
            EffectManager.Instance.UseEffect(effect, transform, true, true);
            skillCreatorManager.AddDraggableSkill(skill);
            isAdded = true;
        }
    }

    public bool GetIsAdded() { return isAdded; }
}
