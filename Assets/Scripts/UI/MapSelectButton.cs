using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapSelectButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private GameObject lockImage;

    public void SetText(string text) { tmp.text = text; }
    public void AddButtonOnClick(UnityAction method) { button.onClick.AddListener(method); }

    public void Lock()
    {
        button.interactable = false;
        lockImage.SetActive(true); 
    }

    public void Unlock()
    {
        button.interactable = true;
        lockImage.SetActive(false);
    }
}
