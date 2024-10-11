using UnityEngine;

public class SingletonChampion<T> : ChampBase where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    protected override void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }

            return;
        }

        instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}
