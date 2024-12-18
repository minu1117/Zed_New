using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject elementParent;                  // 요소 부모 프리팹
    [SerializeField] private MapSelectElement elementPrefab;            // 버튼 부모 프리팹
    [SerializeField] private MapSelectButton mapSelectButtonPrefab;     // 맵 이동 버튼 프리팹

    private List<MapSelectElement> createdElements;                     // 생성된 요소들
    private List<MapSelectButton> createdMapSelectButtons;              // 생성한 맵 이동 버튼들

    public List<MapSelectButton> GetCreatedButtons() { return createdMapSelectButtons; }

    private void Awake()
    {
        AddOnClickExitButton(() => SetActiveMapSelectUI(false));
    }

    public void CreateElements(int count, List<string> stageNames)
    {
        createdElements = new();
        for (int i = 0; i < count; i++)
        {
            var element = Instantiate(elementPrefab, elementParent.transform);
            element.SetStageName(stageNames[i]);
            createdElements.Add(element);
        }
    }

    public void CreateButtons(int stageIndex, int count)
    {
        if (createdMapSelectButtons == null)
            createdMapSelectButtons = new();

        if (createdElements == null || createdElements.Count == 0)
            return;

        for (int i = 0; i < count; i++)
        {
            var button = Instantiate(mapSelectButtonPrefab, createdElements[stageIndex].GetButtonParent().transform);

            string buttonText = i == count - 1 ? "보스" : $"{i + 1}층";
            button.SetText(buttonText);

            createdMapSelectButtons.Add(button);
        }
    }

    public void LockButton(int index)
    {
        createdMapSelectButtons[index].Lock();
    }

    public void UnlockButton(int index)
    {
        createdMapSelectButtons[index].Unlock();
    }

    public void SetActiveMapSelectUI(bool active)
    {
        if (ui == null)
            return;

        ui.SetActive(active);
    }

    public void AddOnClickExitButton(UnityAction method)
    {
        exitButton.onClick.AddListener(method);
    }

    public void AddButtonOnClick(int index, UnityAction method)
    {
        createdMapSelectButtons[index].AddButtonOnClick(method);
    }
}
