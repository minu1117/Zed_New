using UnityEngine;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private Button optionButton;

    private void Awake()
    {
        CustomSceneManager.Instance.SetOptionButton(optionButton);
    }
}
