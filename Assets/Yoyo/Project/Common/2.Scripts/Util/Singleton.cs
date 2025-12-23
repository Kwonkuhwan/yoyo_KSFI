using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _shuttingDown = false;
    private new const string DontDestroyOnLoad = "DontDestroyOnLoad";

    public static T Instance
    {
        get
        {
            if (_shuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' is already destroyed. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";

                        DontDestroyOnLoad(singletonObject);
                    }
                }
                if (!IsDontDestroyOnLoad(_instance.gameObject))
                {
                    DontDestroyOnLoad(_instance);
                }
                // if (!_instance.gameObject.scene.name.Equals("DontDestroyOnLoad"))
                // {
                //     DontDestroyOnLoad(_instance);
                // }
                return _instance;
            }
        }
    }
    
    private static bool IsDontDestroyOnLoad(GameObject obj)
    {
        return obj.scene.name.Equals(DontDestroyOnLoad);
    }

    private void OnApplicationQuit()
    {
        _shuttingDown = true;
    }

    private void OnDestroy()
    {
        _shuttingDown = true;
    }
}
