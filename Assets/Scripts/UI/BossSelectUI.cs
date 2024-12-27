using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BossSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject selectPanel;
    [SerializeField] private Button boss_1_button;
    [SerializeField] private Button boss_2_button;
    [SerializeField] private Button boss_3_button;
    [SerializeField] private Button boss_4_button;
    [SerializeField] private Button exitbutton;

    private void Awake()
    {
        AddListenerExitButton(() => SetActiveUI(false));
        AddListenerExitButton(() => Zed.Instance.GetMoveController().StartMove());
    }

    public void SetActiveUI(bool set)
    {
        selectPanel.SetActive(set);
    }

    public void AddListenerBoss_1(UnityAction method)
    {
        boss_1_button.onClick.AddListener(method);
    }
    public void AddListenerBoss_2(UnityAction method)
    {
        boss_2_button.onClick.AddListener(method);
    }
    public void AddListenerBoss_3(UnityAction method)
    {
        boss_3_button.onClick.AddListener(method);
    }
    public void AddListenerBoss_4(UnityAction method)
    {
        boss_4_button.onClick.AddListener(method);
    }
    public void AddListenerExitButton(UnityAction method)
    {
        exitbutton.onClick.AddListener(method);
    }
}