using UnityEngine;

public class HealingSpot : InteractiveObject
{
    [SerializeField] private Effect effect;

    private void Update()
    {
        if (!CheckDistance())
            return;

        Interaction();
    }

    protected override void Interaction()
    {
        if (player == null)
            return;

        if (Input.GetKeyDown(interactionKey))
        {
            var hpController = player.GetStatusController(SliderMode.HP);
            if (hpController == null)
                return;

            StartSound(interactionSound);
            hpController.SetMaxHp();

            var tr = player.transform;
            EffectManager.Instance.UseEffect(effect, tr, true);
        }
    }
}
