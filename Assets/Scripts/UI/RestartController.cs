using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RestartController : MonoBehaviour
{
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private AudioClip clickSound;
    private bool isComplate;

    private void Awake()
    {
        restartButton.onClick.AddListener(() => StartCoroutine(Restart()));
    }

    public void SetActiveDefeatPanel(bool set) { defeatPanel.SetActive(set); }

    private IEnumerator Restart()
    {
        isComplate = false;
        SoundManager.Instance.PlayOneShot(clickSound);
        var sceneMgr = CustomSceneManager.Instance;
        sceneMgr.FadeIn();

        yield return new WaitUntil(() => !sceneMgr.isFade);

        SetActiveDefeatPanel(false);
        sceneMgr.FadeOut();
        isComplate = true;
    }

    public bool GetIsComplate() { return isComplate; }
}
