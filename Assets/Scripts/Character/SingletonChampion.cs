using UnityEngine;

public class SingletonChampion<T> : ChampBase where T : MonoBehaviour
{
    public static T Instance;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = GetComponent<T>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    protected void OnDestroy()
    {
        // 씬이 바뀌면 파괴
        if (Instance != null)
        {
            Instance = null;
        }
    }
}
