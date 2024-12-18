using TMPro;
using UnityEngine;

public class MapSelectElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageNameTmp;
    [SerializeField] private GameObject buttonParent;
    public void SetStageName(string stageName) { stageNameTmp.text = stageName; }
    public GameObject GetButtonParent() { return buttonParent; }
}
