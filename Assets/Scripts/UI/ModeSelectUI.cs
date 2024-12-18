using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModeSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private Button beginningStartButton;       // 처음부터
    [SerializeField] private Button continueButton;             // 이어하기
    [SerializeField] private Button mapSelectButton;            // 맵 선택

    public void SetActiveUI(bool set)
    { 
        ui.SetActive(set);
    }

    public void AddOnClickBeginningStartButton(UnityAction method)
    {
        beginningStartButton.onClick.AddListener(method);
    }

    public void AddOnClickContinueButton(UnityAction method)
    {
        continueButton.onClick.AddListener(method);
    }

    public void AddOnClickMapSelectButton(UnityAction method)
    {
        mapSelectButton.onClick.AddListener(method);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && ui.activeSelf)
        {
            var zed = Zed.Instance;
            var moveController = zed.GetMoveController();

            zed.SetAttackUse(true);
            moveController.StartMove();

            SetActiveUI(false);
        }
    }
}
