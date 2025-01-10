using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundController : MonoBehaviour
{
    [SerializeField] private List<Button> buttons;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip pointEnterSound;
    private SoundManager soundManager;

    private void Start()
    {
        soundManager = SoundManager.Instance;

        foreach (var button in buttons)
        {
            AddPointerEnterSound(button);
            button.onClick.AddListener(() => StartSound(clickSound));
        }
    }

    private void AddPointerEnterSound(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };

        entry.callback.AddListener((eventData) => StartSound(pointEnterSound));
        trigger.triggers.Add(entry);
    }

    private void StartSound(AudioClip clip)
    {
        if (soundManager == null)
            return;

        if (clip == null)
            return;

        soundManager.PlayOneShot(clip);
    }
}
