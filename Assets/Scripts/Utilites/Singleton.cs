using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindAnyObjectByType<T>();

            if (instance == null)
            {
                var singletonObject = new GameObject(typeof(T).Name);
                instance = singletonObject.AddComponent<T>();
            }

            return instance;
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}